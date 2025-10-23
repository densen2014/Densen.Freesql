// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using FreeSql;
using FreeSql.DataAnnotations;
using FreeSql.Internal.Model;
using System.Linq.Expressions;
using System.Reflection;
using Console = System.Console;

namespace Densen.DataAcces.FreeSql;

/// <summary>
/// FreeSql ORM 查询工具类
/// </summary>
public static partial class FreeSqlUtil
{

    /// <summary>
    /// 获得查询子句
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="option"></param>
    /// <param name="fsql"></param>
    /// <param name="WhereCascade"></param>
    /// <param name="IncludeByPropertyNames"></param>
    /// <param name="LeftJoinString"></param>
    /// <param name="WhereCascadeOr"></param>
    /// <param name="WhereLamda"></param>
    /// <returns></returns>
    public static ISelect<TModel> BuildWhere<TModel>(
                        QueryPageOptions option,
                        IFreeSql fsql,
                        DynamicFilterInfo? WhereCascade = null,
                        List<string>? IncludeByPropertyNames = null,
                        string? LeftJoinString = null,
                        List<string>? WhereCascadeOr = null,
                        Expression<Func<TModel, bool>>? WhereLamda = null) where TModel : class
    {
        var dynamicFilterInfo = MakeDynamicFilterInfo(option, WhereCascade, WhereCascadeOr);
        var fsql_select = fsql.Select<TModel>();

        if (LeftJoinString != null)
        {
            fsql_select = fsql_select.LeftJoin(LeftJoinString);
        }
        if (WhereLamda != null)
        {
            fsql_select = fsql_select.Where(WhereLamda);
        }
        if (IncludeByPropertyNames != null)
        {
            foreach (var item in IncludeByPropertyNames)
            {
                if (item.IndexOf(",") != -1 && item.Split(",").Length > 2)
                {
                    var t1s = item.Split(",");
                    fsql_select = fsql_select.IncludeByPropertyName(t1s[0], then => then.IncludeByPropertyName(t1s[1], then => then.IncludeByPropertyName(t1s[2])));
                }
                else if (item.IndexOf(",") != -1 && item.Split(",").Length > 1)
                {
                    var t1s = item.Split(",");
                    fsql_select = fsql_select.IncludeByPropertyName(t1s[0], then => then.IncludeByPropertyName(t1s[1]));
                }
                else
                {
                    fsql_select = fsql_select.IncludeByPropertyName(item);
                }
            }
        }

        if (dynamicFilterInfo != null)
        {
            fsql_select = fsql_select.WhereDynamicFilter(dynamicFilterInfo);
        }

        return fsql_select;
    }

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="option">查询条件</param>
    /// <param name="optionsLast">(已弃用)缓存查询条件</param>
    /// <param name="TotalCount"></param> 
    /// <param name="fsql"></param>
    /// <param name="WhereCascade">附加查询条件使用and结合</param>
    /// <param name="IncludeByPropertyNames">附加IncludeByPropertyName查询条件.<para></para>
    /// 其中单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. <para></para>
    /// 单项第二个逗号隔开可第三层then附加</param>
    /// <param name="LeftJoinString">左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")</param>
    /// <param name="OrderByPropertyName">强制排序,但是手动排序优先, ["len(CustomerID)", "CustomerID"]</param>
    /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
    /// <param name="forceAllItems">附加查询条件使用or结合</param>
    /// <param name="WhereLamda">查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")</param>
    /// <param name="asTable">使用分表,走WithTempQuery方法查询</param>
    /// <param name="timeout">命令超时设置(秒),默认-1使用数据库默认超时</param>
    public static QueryData<TModel> Fetch<TModel>(
                            out string? message,
                            QueryPageOptions option,
                            QueryPageOptions? optionsLast,
                            long? TotalCount,
                            IFreeSql fsql,
                            DynamicFilterInfo? WhereCascade = null,
                            List<string>? IncludeByPropertyNames = null,
                            string? LeftJoinString = null,
                            List<string>? OrderByPropertyName = null,
                            List<string>? WhereCascadeOr = null,
                            bool forceAllItems = false,
                            Expression<Func<TModel, bool>>? WhereLamda = null,
                            bool AsTable = false,
                            int timeout = -1) where TModel : class
    {
        message = null;
        var items = new List<TModel>();

        if (forceAllItems)
        {
            option.IsPage = false;
            option.IsVirtualScroll = false;
        }

        //optionsLast = optionsLast ?? option;

        try
        {
            var isSearch = option.Searches.Any() || option.CustomerSearches.Any() || option.AdvanceSearches.Any();

            var fsql_select = BuildWhere(option, fsql, WhereCascade, IncludeByPropertyNames, LeftJoinString, WhereCascadeOr, WhereLamda);

            //使用分表,走WithTempQuery方法查询 https://github.com/dotnetcore/FreeSql/discussions/1066?WT.mc_id=DT-MVP-5005078#discussioncomment-8846214
            if (AsTable)
            {
                fsql_select = fsql_select.WithTempQuery(a => CreateInstance<TModel>());
            }

            //处理排序字段不存在错误捕获
            try
            {
                fsql_select = fsql_select.OrderByPropertyNameIf(option.SortOrder != SortOrder.Unset, option.SortName, option.SortOrder == SortOrder.Asc);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            #region "处理手动排序条件"
            if (OrderByPropertyName != null)
            {
                foreach (var itemtemp in OrderByPropertyName)
                {
                    try
                    {
                        var item = itemtemp;
                        var isAscending = true;
                        if (item.EndsWith(" desc") && !item.StartsWith("len("))
                        {
                            isAscending = false;
                            item = item.Replace(" desc", "");
                        }
                        if (item.StartsWith("len("))
                        {
                            fsql_select = fsql_select.OrderBy(item);
                        }
                        else if(item.StartsWith('('))
                        {
                            fsql_select = fsql_select.OrderBy(item.TrimStart('(').TrimEnd(')'));
                        }
                        else if (item.Contains("case when"))
                        {
                            fsql_select = fsql_select.OrderBy(item);
                        }
                        else if (option.SortOrder == SortOrder.Unset)
                        {
                            fsql_select = fsql_select.OrderByPropertyName(item, isAscending);
                        }
                        else if (option.SortOrder != SortOrder.Unset && option.SortName != null && !item.Equals(option.SortName))
                        {
                            //过滤预设排序名称和点击排序名称一致的情况
                            fsql_select = fsql_select.OrderByPropertyName(item, isAscending);
                        }
                    }
                    catch
                    {
                        fsql_select = fsql_select.OrderBy(itemtemp);
                    }
                }
            }
            #endregion

            //分页
            long count = 0;

            if (timeout > -1)
            {
                fsql_select = fsql_select.CommandTimeout(timeout);
            }
            fsql_select = fsql_select.Count(out count);


            //判断是否分页
            if (option.IsPage)
            {
                fsql_select = fsql_select.Page(option.PageIndex, option.PageItems);
            }
            else if (option.IsVirtualScroll)
            {
                fsql_select = fsql_select.Skip(option.StartIndex).Take(option.PageItems);
            }
            items = fsql_select.ToList();

            TotalCount = count;

            if (TotalCount == 0)
            {
                option.PageIndex = 1;
            }

            //}
        }
        catch (Exception e)
        {
            Console.WriteLine("FreeSqlDataService Error: " + e.Message);
            items = [];
            TotalCount = 0;
        }
        var ret = new QueryData<TModel>()
        {
            TotalCount = (int)(TotalCount ?? 0),
            Items = items,
            IsSorted = option.SortOrder != SortOrder.Unset,
            IsFiltered = option.Filters.Any(),
            IsAdvanceSearch = option.AdvanceSearches.Any(),
            IsSearch = option.Searches.Any() || option.CustomerSearches.Any()
        };

        return ret;
    }

    /// <summary>
    ///  DateTime.MinValue 在orm中转换有点问题,先自定义一个
    /// </summary>
    private static DateTime dateTimeMinValue = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

    #region "生成Where子句的DynamicFilterInfo对象"
    /// <summary>
    /// 生成Where子句的DynamicFilterInfo对象
    /// </summary>
    /// <param name="option"></param>
    /// <param name="WhereCascade">强制and的条件</param>
    /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
    /// <returns></returns>
    public static DynamicFilterInfo? MakeDynamicFilterInfo(QueryPageOptions option,
                                                     DynamicFilterInfo? WhereCascade = null,
                                                     List<string>? WhereCascadeOr = null)
    {
        var filters = new List<DynamicFilterInfo>();
        object? searchModel = option.SearchModel;
        if (searchModel == null && WhereCascade == null && WhereCascadeOr == null)
        {
            return null;
        }

        if (searchModel != null)
        {
            #region "处理搜索模型"
            Type type = searchModel.GetType();

            var instance = Activator.CreateInstance(type);

            var properties = type.GetProperties().Where(a =>
                                                    (a.PropertyType.IsClass || a.PropertyType == typeof(string) || a.PropertyType == typeof(DateTime) || a.PropertyType == typeof(DateTime?) || a.PropertyType.IsNumberType())
                                                    && a.GetCustomAttribute<ColumnAttribute>()?.IsIgnore != true
                                                    && a.GetCustomAttribute<ColumnAttribute>()?.MapType != typeof(int)
                                                    ).ToList();

            //TODO : 支持更多类型, 目前只支持 string , Number , DateTime 类型
            //搜索所有 string 类型和 Number 类型的字段
            //过滤FreeSql特性 : 1. 忽略特性, 2.MapType = typeof(int)
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                // 检查是否为复杂类型（排除基础类型和字符串）
                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    // 递归处理嵌套对象, 目前只处理第二层
                    var nestedValue = property.GetValue(searchModel);
                    if (nestedValue != null)
                    {
                        var nestedType = nestedValue.GetType();
                        var nestedProperties = nestedType.GetProperties().Where(a =>
                                        (a.PropertyType == typeof(string) || a.PropertyType == typeof(DateTime) || a.PropertyType == typeof(DateTime?) || a.PropertyType.IsNumberType())
                                        && a.GetCustomAttribute<AutoGenerateColumnAttribute>()?.Searchable == true
                                        && a.GetCustomAttribute<ColumnAttribute>()?.IsIgnore != true
                                        && a.GetCustomAttribute<ColumnAttribute>()?.MapType != typeof(int)
                                        ).ToList();
                        foreach (var nestedProperty in nestedProperties)
                        {
                            var flowControl = GeneratePropertyFilters(nestedValue, nestedValue, nestedProperty);
                            if (!flowControl)
                            {
                                continue;
                            }
                        }
                    }


                }
                else
                {
                    var flowControl = GeneratePropertyFilters(searchModel, instance, property);
                    if (!flowControl)
                    {
                        continue;
                    }
                }
            }

            // 处理表格过滤条件
            if (option.Filters.Any())
            {
                filters.AddRange(option.Filters.Select(i => i.ToDynamicFilter()));
            }
            #endregion
        }

        DynamicFilterInfo dyfilter = new DynamicFilterInfo();

        if (filters.Any(a => a.Field != null || a.Filters != null))
        {
            dyfilter = new DynamicFilterInfo()
            {
                Logic = string.IsNullOrEmpty(option.SearchText) ? DynamicFilterLogic.And : DynamicFilterLogic.Or,
                Filters = filters
            };

            //生成带预设条件的复合查询
            if (WhereCascade != null)
            {
                var filtersWhereCascade = new List<DynamicFilterInfo>
                {
                    WhereCascade,
                    dyfilter
                };
                DynamicFilterInfo dyfilterWhereCascade = new DynamicFilterInfo()
                {
                    Logic = DynamicFilterLogic.And,
                    Filters = filtersWhereCascade
                };
                return dyfilterWhereCascade;
            }

            //生成带预设条件的复合查询 Or
            if (WhereCascadeOr != null)
            {
                var filtersWhereCascade = new List<DynamicFilterInfo>();
                foreach (var item in WhereCascadeOr)
                {
                    filtersWhereCascade.Add(new DynamicFilterInfo()
                    {
                        Field = item,
                        Operator = DynamicFilterOperator.Contains,
                        Value = option.SearchText
                    });
                    //fsql_select = fsql_select.Where($"{item} like '%{options.SearchText}%'");
                }
                filtersWhereCascade.Add(dyfilter);
                DynamicFilterInfo dyfilterWhereCascadeOr = new DynamicFilterInfo()
                {
                    Logic = DynamicFilterLogic.Or,
                    Filters = filtersWhereCascade
                };
                return dyfilterWhereCascadeOr;
            }

            return dyfilter;

        }
        else if (WhereCascade != null)
        {
            return WhereCascade;
        }

        return null;


        bool GeneratePropertyFilters(object searchModel, object? instance, PropertyInfo property)
        {
            // 处理基础类型
            if (string.IsNullOrEmpty(option.SearchText))
            {
                var searchValue = property.GetValue(searchModel)?.ToString();

                // 处理高级搜索
                if (searchValue != null && (option.AdvanceSearches.Any() || option.CustomerSearches.Any()))
                {
                    var propertyValue = property.GetValue(searchModel);
                    var instanceValue = property.GetValue(instance);

                    //跟默认值比较,如果不一致,则加入搜索条件
                    if (!propertyValue!.Equals(instanceValue))
                    {
                        //临时处理日期类型,默认值只比较日期部分,因为实例化时间部分不一致
                        var isDateTime = property.PropertyType.IsDateTimeType();
                        if (isDateTime)
                        {
                            var propertyValueDatetime = Convert.ToDateTime(propertyValue);
                            var instanceValueDatetime = Convert.ToDateTime(instanceValue);
                            var span = instanceValueDatetime.Subtract(propertyValueDatetime);
                            //如果值为dateTimeMinValue,不加入搜索条件.
                            //如果时间差小于5秒,则不加入搜索条件并重置为dateTimeMinValue,因为可能是第一次渲染的
                            if (propertyValueDatetime == dateTimeMinValue)
                            {
                                return false;
                            }
                            else if (span.TotalSeconds < 5)
                            {
                                property.SetValue(searchModel, dateTimeMinValue);
                                Console.WriteLine($"时间差:{span.TotalSeconds},重置{property.Name}为{DateTime.MinValue}");
                                return false;
                            }
                        }
                        var fiter = property.GenFilter(searchValue);
                        if (fiter != null)
                        {
                            filters.Add(fiter);
                        }
                    }
                }
            }
            else
            {
                // 处理模糊搜索
                var fiter = property.GenFilter(option.SearchText);
                if (fiter != null)
                {
                    filters.Add(fiter);
                }
            }

            return true;
        }
    }


    private static DynamicFilterInfo? GenFilter(this PropertyInfo propertyinfo, string SearchText)
    {
        var isNumber = propertyinfo.PropertyType.IsNumberType();
        var isDateTime = propertyinfo.PropertyType.IsDateTimeType();
        if (isNumber && !SearchText.IsNumeric())
        {
            return null;
        }

        if (isDateTime && !SearchText.IsDatetime())
        {
            return null;
        }

        object val;
        try
        {
            if (propertyinfo.PropertyType.IsIntegerType())
            {
                if (SearchText.IsInt())
                {
                    val = Convert.ToInt32(SearchText);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyinfo.PropertyType.IsNumberType())
            {
                val = Convert.ToDecimal(SearchText);
            }
            else if (isDateTime)
            {
                val = Convert.ToDateTime(SearchText);


                //if (val.ToDate() < DateTime.MinValue || val.ToDate() > DateTime.MaxValue) 这个方式处理 1253-3-3 会报错
                //mssql【datetime】数据类型：最大是9999年12 月31日，最小是1753年1月1日
                if (val.ToDate() < new DateTime(1753, 1, 1) || val.ToDate() > new DateTime(9999, 12, 31))
                {
                    return null;
                }
                else if (SearchText.Length >= 12)
                {
                    SearchText = $"{val:yyyy-MM-dd HH}";
                }
                else if (SearchText.Length >= 10 || val.ToDate().Day > 1)
                {
                    SearchText = $"{val:yyyy-MM-dd}";
                }
                else if (SearchText.Length >= 6)
                {
                    //FreeSql: DateRange requires that the Value [1] format must be: yyyy, yyyy-MM, yyyy-MM-dd, yyyyy-MM-dd HH, yyyy, yyyy-MM-dd HH:mm”
                    SearchText = $"{val:yyyy-MM}";
                }
                else if (SearchText.Length == 4)
                {
                }
                else
                {
                    return null;
                }
            }
            else
            {
                val = SearchText;
            }
        }
        catch (Exception)
        {
            val = SearchText;
            isNumber = false;
        }

        return (new DynamicFilterInfo()
        {
            Field = propertyinfo.Name,
            Operator = isNumber ? DynamicFilterOperator.Equal : isDateTime ? DynamicFilterOperator.DateRange : DynamicFilterOperator.Contains,
            Value = isDateTime ? (new string[] { SearchText, SearchText }) : val,
        });
    }

    private static bool IsNumeric(this string? text) => double.TryParse(text, out _);
    private static bool IsInt(this string? text) => int.TryParse(text, out _);
    private static bool IsDatetime(this string? text) => DateTime.TryParse(text, out _);
    private static bool IsDateTimeType(this Type type) => type == typeof(DateTime) || type == typeof(DateTime?);

    /// <summary>
    /// String转Decimal
    /// </summary>
    /// <param name="t"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private static decimal ToDecimal(this string t, decimal defaultValue = 0m)
    {
        try
        {
            var x = t.IsNumeric() ? Convert.ToDecimal(t) : defaultValue;
            return x;
        }
        catch
        {
        }
        return defaultValue;
    }

    private static double ToDouble(this string t, double defaultValue = 0d)
    {
        try
        {
            var x = t.IsNumeric() ? Convert.ToDouble(t) : defaultValue;
            return x;
        }
        catch
        {
        }
        return defaultValue;
    }
    #endregion

    #region "删除方法"
    public static async Task<int> Delete<TModel>(this IFreeSql fsql, IEnumerable<TModel> models) where TModel : class
    {
        return await fsql.Delete<TModel>(models).ExecuteAffrowsAsync();
    }
    #endregion

    #region "保存方法"
    public static async Task<TModel?> Save<TModel>(this IFreeSql fsql, TModel model, bool EnableCascadeSave, TModel? ItemCache, string? SaveManyChildsPropertyName) where TModel : class
    {
        var repo = fsql.GetRepository<TModel>();

        //一对一(OneToOne)、一对多(OneToMany)、多对多(ManyToMany) 级联保存功能
        repo.DbContextOptions.EnableCascadeSave = EnableCascadeSave;

        if (ItemCache != null)
        {
            //多主键实体,保存前先删除
            var keys = fsql.CodeFirst.GetTableByEntity(typeof(TModel));
            if (keys.Primarys.Any() && keys.Primarys.Length > 1)
            {
                await fsql.Delete<TModel>(ItemCache).ExecuteAffrowsAsync();
            }
        }

        var res = await repo.InsertOrUpdateAsync(model);

        if (!string.IsNullOrEmpty(SaveManyChildsPropertyName))
        {
            try
            {
                //联级保存
                repo.SaveMany(model, SaveManyChildsPropertyName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FreeSqlDataService联级保存 error , {ex.Message}");
            }
        }

        return res;
    }

    #endregion

    /// <summary>
    /// 创建实例
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TModel CreateInstance<TModel>()
    {
        try
        {
            return Activator.CreateInstance<TModel>();
        }
        catch (Exception)
        {
            throw new InvalidOperationException($"{typeof(TModel)} missing new() method. {typeof(TModel)} 未提供无参构造函数 new()");
        }
    }
}

