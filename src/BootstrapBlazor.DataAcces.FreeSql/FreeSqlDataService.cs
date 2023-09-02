// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using FreeSql.Internal.Model;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Console = System.Console;

namespace Densen.DataAcces.FreeSql;

/// <summary>
/// FreeSql ORM 的 IDataService 数据注入服务接口实现
/// </summary>
public class FreeSqlDataService<TModel> : DataServiceBase<TModel> where TModel : class, new()
{
    [NotNull]
    public IFreeSql? fsql;

    /// <summary>
    /// 构造函数
    /// </summary>
    public FreeSqlDataService(IFreeSql fsql)
    {
        this.fsql = fsql;
    }

    /// <summary>
    /// 删除方法
    /// </summary>
    /// <param name="models"></param>
    /// <returns></returns>
    public override async Task<bool> DeleteAsync(IEnumerable<TModel> models)
    {
        // 通过模型获取主键列数据
        // 支持批量删除
        await fsql.Delete<TModel>(models).ExecuteAffrowsAsync();
        TotalCount = null;
        return true;
    }

    /// <summary>
    /// 指定数据库连接字符串 [预留功能]
    /// </summary>
    public string? ibstring { get; set; }

    /// <summary>
    /// 级联保存字段名
    /// </summary>
    public string? SaveManyChildsPropertyName { get; set; }

    /// <summary>
    /// 是否开启 一对一(OneToOne)、一对多(OneToMany)、多对多(ManyToMany) 级联保存功能<para></para>
    /// <para></para>
    /// 【一对一】模型下，保存时级联保存 OneToOne 属性。
    /// <para></para>
    /// 【一对多】模型下，保存时级联保存 OneToMany 集合属性。出于安全考虑我们没做完整对比，只针对实体属性集合的添加或更新操作，因此不会删除数据库表已有的数据。<para></para>
    /// </summary>
    public bool EnableCascadeSave { get; set; } = false;

    /// <summary>
    /// 保存方法
    /// </summary>
    /// <param name="model">实体</param>
    /// <param name="changedType">数据变化类型</param>
    /// <returns></returns>
    public override async Task<bool> SaveAsync(TModel model, ItemChangedType changedType)
    {
        var repo = fsql.GetRepository<TModel>();

        //一对一(OneToOne)、一对多(OneToMany)、多对多(ManyToMany) 级联保存功能
        repo.DbContextOptions.EnableCascadeSave = EnableCascadeSave;

        await repo.InsertOrUpdateAsync(model);

        //TODO 测试mssql2005
        //if (EnableCascadeSave)
        //{
        //    if (changedType == ItemChangedType.Add)
        //    {
        //        await repo.InsertAsync(model);
        //    }
        //    else
        //    {
        //        await repo.UpdateAsync(model);
        //    }

        //}
        //else
        //{
        //    await repo.InsertOrUpdateAsync(model);
        //}
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

        TotalCount = null;
        return true;
    }

    /// <summary>
    /// 缓存记录总数
    /// </summary>
    long? TotalCount { get; set; }

    /// <summary>
    /// 缓存记录
    /// </summary>
    public List<TModel>? Items { get; set; }

    /// <summary>
    /// 全部记录
    /// </summary>
    /// <param name="WhereCascade">附加查询条件使用and结合</param>
    /// <param name="IncludeByPropertyNames">附加IncludeByPropertyName查询条件, 单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. (暂时只支持一个then附加)</param>
    /// <param name="LeftJoinString">左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")</param>
    /// <param name="OrderByPropertyName">强制排序,但是手动排序优先</param>
    /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
    /// <param name="WhereLamda">查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")</param>
    /// <returns></returns>
    public List<TModel>? GetAllItems(
                DynamicFilterInfo? WhereCascade = null,
                List<string>? IncludeByPropertyNames = null,
                string? LeftJoinString = null,
                List<string>? OrderByPropertyName = null,
                List<string>? WhereCascadeOr = null,
                Expression<Func<TModel, bool>>? WhereLamda = null)
    {
        var res = FsqlUtil.Fetch<TModel>(OptionsCache, OptionsCache, null, fsql, WhereCascade, IncludeByPropertyNames, LeftJoinString, OrderByPropertyName, WhereCascadeOr, true, WhereLamda);
        return res.Items?.ToList();
    }

    /// <summary>
    /// 缓存查询条件
    /// </summary>
    QueryPageOptions OptionsCache { get; set; } = new QueryPageOptions();

    /// <summary>
    /// 查询方法
    /// </summary>
    /// <param name="option"></param>
    /// <param name="WhereCascade">附加查询条件使用and结合</param>
    /// <param name="IncludeByPropertyNames">附加IncludeByPropertyName查询条件, 单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. (暂时只支持一个then附加)</param>
    /// <param name="LeftJoinString">左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")</param>
    /// <param name="OrderByPropertyName">强制排序,但是手动排序优先</param>
    /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
    /// <param name="WhereLamda">查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")</param>
    /// <returns></returns>
    public Task<QueryData<TModel>> QueryAsyncWithWhereCascade(
                QueryPageOptions option,
                DynamicFilterInfo? WhereCascade = null,
                List<string>? IncludeByPropertyNames = null,
                string? LeftJoinString = null,
                List<string>? OrderByPropertyName = null,
                List<string>? WhereCascadeOr = null,
                Expression<Func<TModel, bool>>? WhereLamda = null)
    {
        var res = FsqlUtil.Fetch(option, OptionsCache, TotalCount, fsql, WhereCascade, IncludeByPropertyNames, LeftJoinString, OrderByPropertyName, WhereCascadeOr, WhereLamda: WhereLamda);
        TotalCount = res.TotalCount;
        Items = res.Items?.ToList();
        OptionsCache = option;
        return Task.FromResult(res);
    }


    /// <summary>
    /// 查询方法
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public override Task<QueryData<TModel>> QueryAsync(QueryPageOptions option)
    {
        var res = FsqlUtil.Fetch<TModel>(option, OptionsCache, TotalCount, fsql);
        TotalCount = res.TotalCount;
        Items = res.Items?.ToList();
        OptionsCache = option;
        return Task.FromResult(res);
    }


}

/// <summary>
/// FreeSql ORM 查询工具类
/// </summary>
public static class FsqlUtil
{
    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="options">查询条件</param>
    /// <param name="optionsLast">缓存查询条件</param>
    /// <param name="TotalCount"></param> 
    /// <param name="fsql"></param>
    /// <param name="WhereCascade">附加查询条件使用and结合</param>
    /// <param name="IncludeByPropertyNames">附加IncludeByPropertyName查询条件.<para></para>
    /// 其中单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. <para></para>
    /// 单项第二个逗号隔开可第三层then附加</param>
    /// <param name="LeftJoinString">左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")</param>
    /// <param name="OrderByPropertyName">强制排序,但是手动排序优先</param>
    /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
    /// <param name="forceAllItems">附加查询条件使用or结合</param>
    /// <param name="WhereLamda">查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")</param>
    public static QueryData<TModel> Fetch<TModel>(
                            QueryPageOptions options,
                            QueryPageOptions? optionsLast,
                            long? TotalCount,
                            IFreeSql fsql,
                            DynamicFilterInfo? WhereCascade = null,
                            List<string>? IncludeByPropertyNames = null,
                            string? LeftJoinString = null,
                            List<string>? OrderByPropertyName = null,
                            List<string>? WhereCascadeOr = null,
                            bool forceAllItems = false,
                            Expression<Func<TModel, bool>>? WhereLamda = null) where TModel : class, new()
    {
        var items = new List<TModel>(); ;

        // 过滤
        var isFiltered = options.Filters.Any();

        // 排序
        var isSorted = !string.IsNullOrEmpty(options.SortName);

        var isSerach = false;

        if (forceAllItems) options.IsPage = false;

        optionsLast = optionsLast ?? options;

        try
        {

            var dynamicFilterInfo = MakeDynamicFilterInfo(options, out isSerach, WhereCascade, WhereCascadeOr);

            if (TotalCount != null && !isSerach && options.PageItems != optionsLast.PageItems && TotalCount <= optionsLast.PageItems)
            {
                //当选择的每页显示数量大于总数时，强制认为是一页
                //无搜索,并且总数<=分页总数直接使用内存排序和搜索
                Console.WriteLine($"无搜索,分页数相等{options.PageItems}/{optionsLast.PageItems},直接使用内存排序和搜索");
            }
            else
            {
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

                if (isSerach)
                    fsql_select = fsql_select.WhereDynamicFilter(dynamicFilterInfo);

                fsql_select = fsql_select.OrderByPropertyNameIf(options.SortOrder != SortOrder.Unset, options.SortName, options.SortOrder == SortOrder.Asc);

                if (OrderByPropertyName != null)
                {
                    foreach (var itemtemp in OrderByPropertyName)
                    {
                        try
                        {
                            var item = itemtemp;
                            var isAscending = true;
                            if (item.EndsWith(" desc"))
                            {
                                isAscending = false;
                                item = item.Replace(" desc", "");
                            }
                            if (item.StartsWith("len("))
                            {
                                fsql_select = fsql_select.OrderBy(item);
                            }
                            else if (options.SortOrder == SortOrder.Unset)
                            {
                                fsql_select = fsql_select.OrderByPropertyName(item, isAscending);
                            }
                            else if (options.SortOrder != SortOrder.Unset && options.SortName != null && !item.Equals(options.SortName))
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

                //分页==1 or null 才获取记录总数量,省点性能
                long count = 0;
                if ((TotalCount??0)== 0 || options.PageIndex == 1) fsql_select = fsql_select.Count(out count);

                //判断是否分页
                if (options.IsPage) fsql_select = fsql_select.Page(options.PageIndex, options.PageItems);

                items = fsql_select.ToList();

                TotalCount = ((TotalCount ?? 0) == 0 || options.PageIndex == 1) ? count : TotalCount;

            }
        }
        catch (Exception e)
        {
            Console.WriteLine("FreeSqlDataService Error: " + e.Message);
            items = new List<TModel>();
            TotalCount = 0;
        }
        var ret = new QueryData<TModel>()
        {
            TotalCount = (int)(TotalCount ?? 0),
            Items = items,
            IsSorted = isSorted,
            IsFiltered = isFiltered,
            IsSearch = isSerach
        };

        return ret;
    }

    #region "生成Where子句的DynamicFilterInfo对象"
    /// <summary>
    /// 生成Where子句的DynamicFilterInfo对象
    /// </summary>
    /// <param name="option"></param>
    /// <param name="isSerach"></param>
    /// <param name="WhereCascade">强制and的条件</param>
    /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
    /// <returns></returns>
    public static DynamicFilterInfo? MakeDynamicFilterInfo(QueryPageOptions option,
                                                     out bool isSerach,
                                                     DynamicFilterInfo? WhereCascade = null,
                                                     List<string>? WhereCascadeOr = null)
    {
        var filters = new List<DynamicFilterInfo>();
        object? searchModel = option.SearchModel;
        if (searchModel == null)
        {
            isSerach = false;
            return null;
        }
        Type type = searchModel.GetType();

        var instance = Activator.CreateInstance(type);

        if (string.IsNullOrEmpty(option.SearchText))
        {
            //生成高级搜索子句
            //TODO : 支持更多类型
            foreach (var propertyinfo in type.GetProperties().Where(a => a.PropertyType == typeof(string) || a.PropertyType == typeof(int)).ToList())
            {
                if (propertyinfo.GetValue(searchModel) != null && !propertyinfo.GetValue(searchModel)!.Equals(propertyinfo.GetValue(instance)))
                {
                    var isInt = propertyinfo.PropertyType == typeof(int);
                    var propertyValue = propertyinfo.GetValue(searchModel)?.ToString();
                    if (propertyValue == null || (isInt && !propertyValue.IsNumeric())) continue;
                    var attr = propertyinfo.GetCustomAttribute<ColumnAttribute>();
                    if (attr?.IsIgnore ?? false) continue;
                    object val;
                    try
                    {
                        val = isInt ? Convert.ToInt32(propertyValue) : propertyValue;
                    }
                    catch (Exception)
                    {
                        val = propertyValue;
                        isInt = false;
                    }

                    filters.Add(new DynamicFilterInfo()
                    {
                        Field = propertyinfo.Name,
                        Operator = isInt ? DynamicFilterOperator.Equal : DynamicFilterOperator.Contains,
                        Value = val,
                    });
                }
            }

        }
        else
        {
            //生成默认搜索子句
            //TODO : 支持更多类型
            foreach (var propertyinfo in type.GetProperties().Where(a => a.PropertyType == typeof(string) || a.PropertyType == typeof(int)).ToList())
            {
                var isInt = propertyinfo.PropertyType == typeof(int);
                if (isInt && !option.SearchText.IsNumeric()) continue;
                object val;
                try
                {
                    val = isInt ? Convert.ToInt32(option.SearchText) : option.SearchText;
                }
                catch (Exception)
                {
                    val = option.SearchText;
                    isInt = false;
                }
                var attr = propertyinfo.GetCustomAttribute<ColumnAttribute>();

                //过滤FreeSql忽略特性的列
                if (attr?.IsIgnore ?? false) continue;

                filters.Add(new DynamicFilterInfo()
                {
                    Field = propertyinfo.Name,
                    Operator = isInt ? DynamicFilterOperator.Equal : DynamicFilterOperator.Contains,
                    Value = val,
                });
            }

        }

        if (option.Filters.Any())
        {
            foreach (var item in option.Filters)
            {
                if (item.GetFilterConditions().Filters != null)
                {
                    foreach (var filter in item.GetFilterConditions().Filters!)
                    {
                        var filterOperator = DynamicFilterOperator.Contains;

                        filterOperator = filter.FilterAction switch
                        {
                            FilterAction.Equal => DynamicFilterOperator.Equal,
                            FilterAction.NotEqual => DynamicFilterOperator.NotEqual,
                            FilterAction.Contains => DynamicFilterOperator.Contains,
                            FilterAction.NotContains => DynamicFilterOperator.NotContains,
                            FilterAction.GreaterThan => DynamicFilterOperator.GreaterThan,
                            FilterAction.GreaterThanOrEqual => DynamicFilterOperator.GreaterThanOrEqual,
                            FilterAction.LessThan => DynamicFilterOperator.LessThan,
                            FilterAction.LessThanOrEqual => DynamicFilterOperator.LessThanOrEqual,
                            _ => throw new System.NotSupportedException()
                        };

                        filters.Add(new DynamicFilterInfo()
                        {
                            Field = filter.FieldKey,
                            Operator = filterOperator,
                            Value = filter.FieldValue,
                        });
                    }
                }
            }
        }

        if (filters.Any())
        {
            DynamicFilterInfo dyfilter = new DynamicFilterInfo()
            {
                Logic = string.IsNullOrEmpty(option.SearchText) ? DynamicFilterLogic.And : DynamicFilterLogic.Or,
                Filters = filters
            };
            isSerach = true;

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
            isSerach = true;
            return WhereCascade;
        }

        isSerach = false;
        return null;
    }




    static bool IsNumeric(this string? text) => double.TryParse(text, out _);

    /// <summary>
    /// String转Decimal
    /// </summary>
    /// <param name="t"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    static decimal ToDecimal(this string t, decimal defaultValue = 0m)
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
    static double ToDouble(this string t, double defaultValue = 0d)
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
}
