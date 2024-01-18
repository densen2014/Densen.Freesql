// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using FreeSql;
using FreeSql.Internal.Model;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Console = System.Console;

namespace Densen.DataAcces.FreeSql;

/// <summary>
/// FreeSql ORM 的 IDataService 数据注入服务接口实现
/// </summary>
public class FreeSqlDataService<TModel> : DataServiceBase<TModel> where TModel : class, new()
{
    [NotNull]
    public IFreeSql? fsql { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public FreeSqlDataService()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public FreeSqlDataService(IFreeSql fsql)
    {
        this.fsql = fsql;
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
    /// 缓存记录总数
    /// </summary>
    private long? TotalCount { get; set; }

    /// <summary>
    /// 缓存当前编辑实体
    /// </summary>
    public TModel? ItemCache { get; set; }

    /// <summary>
    /// 缓存记录
    /// </summary>
    public List<TModel>? Items { get; set; }

    /// <summary>
    /// 合计列名
    /// </summary>
    public string? TotalColName { get; set; }

    /// <summary>
    /// 缓存合计
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// 缓存查询条件
    /// </summary>
    private QueryPageOptions OptionsCache { get; set; } = new QueryPageOptions();

    /// <summary>
    /// 是否开启 一对一(OneToOne)、一对多(OneToMany)、多对多(ManyToMany) 级联保存功能<para></para>
    /// <para></para>
    /// 【一对一】模型下，保存时级联保存 OneToOne 属性。
    /// <para></para>
    /// 【一对多】模型下，保存时级联保存 OneToMany 集合属性。出于安全考虑我们没做完整对比，只针对实体属性集合的添加或更新操作，因此不会删除数据库表已有的数据。<para></para>
    /// </summary>
    public bool EnableCascadeSave { get; set; } = false;

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

        if (ItemCache != null)
        { 
            //多主键实体,保存前先删除
            var keys = fsql.CodeFirst.GetTableByEntity(typeof(TModel));
            if (keys.Primarys.Any() && keys.Primarys.Length >1)
            {
                await fsql.Delete<TModel>(ItemCache).ExecuteAffrowsAsync();
            }
        }

        await repo.InsertOrUpdateAsync(model);

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
    /// 获得查询子句
    /// </summary>
    public ISelect<TModel> ISelectCache(
                DynamicFilterInfo? WhereCascade = null,
                List<string>? IncludeByPropertyNames = null,
                string? LeftJoinString = null,
                List<string>? WhereCascadeOr = null,
                Expression<Func<TModel, bool>>? WhereLamda = null) => FreeSqlUtil.BuildWhere(OptionsCache, fsql, WhereCascade, IncludeByPropertyNames, LeftJoinString, WhereCascadeOr, WhereLamda);

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
        var res = FreeSqlUtil.Fetch(OptionsCache, OptionsCache, null, fsql, WhereCascade, IncludeByPropertyNames, LeftJoinString, OrderByPropertyName, WhereCascadeOr, true, WhereLamda);
        return res.Items?.ToList();
    }

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
        var res = FreeSqlUtil.Fetch(option, OptionsCache, TotalCount, fsql, WhereCascade, IncludeByPropertyNames, LeftJoinString, OrderByPropertyName, WhereCascadeOr, WhereLamda: WhereLamda);
        TotalCount = res.TotalCount;
        Items = res.Items?.ToList();
        OptionsCache = option;
        ItemCache = null;
        return Task.FromResult(res);
    }


    /// <summary>
    /// 查询方法
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public override Task<QueryData<TModel>> QueryAsync(QueryPageOptions option)
    {
        var res = FreeSqlUtil.Fetch<TModel>(option, OptionsCache, TotalCount, fsql);
        TotalCount = res.TotalCount;
        Items = res.Items?.ToList();
        OptionsCache = option;
        return Task.FromResult(res);
    }


}

