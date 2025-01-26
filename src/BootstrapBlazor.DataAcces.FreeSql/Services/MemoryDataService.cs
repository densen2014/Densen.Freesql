// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using MiniExcelLibs;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Densen.DataAcces.MemoryData;

/// <summary>
/// 内存数据 的 IDataService 数据注入服务接口实现
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class MemoryDataService<TModel> : DataServiceBase<TModel>
        where TModel : class, new()
{
    public List<TModel> Items { get; set; } = [];
    public List<TModel> ItemsResult { get; set; } = [];
    public IEnumerable<int> PageItemsSource => new int[] { 10, 20, 100, 500 };
    public IEnumerable<int> PageItemsSource50 => new int[] { 50, 100, 200, 500 };

    /// <summary>
    /// 设置主表主键,用于保存临时数据
    /// </summary>
    [NotNull]
    public string? Field { get; set; }

    /// <summary>
    /// 导入Excel
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task ImportFormExcel(string filePath)
    {
        if (filePath == null)
        {
            return;
        }
        //获取到的导入结果为一个字典类型，Key为Sheet名，Value为Sheet对应的数据
        var res = await MiniExcel.QueryAsync<TModel>(filePath, excelType: ExcelType.XLSX);
        Items = res.ToList();
    }

    /// <summary>
    /// 保存方法
    /// </summary>
    /// <param name="model"></param>
    /// <param name="changedType"></param>
    /// <returns></returns>
    public override Task<bool> SaveAsync(TModel? model, ItemChangedType changedType)
    {
        // 增加数据演示代码

        if (model == null)
        {
            //model.FieldSetValue(Field,value);
            Items.Add(new TModel());
        }
        else
        {
            var oldItem = Items.FirstOrDefault();
            try
            {
                if (!string.IsNullOrEmpty(Field))
                {
                    oldItem = Items.Where(a => a.GetField(Field)!.Equals(model.GetField(Field))).FirstOrDefault();
                }
                //var oldItem = LazyHeroDataService.Items.Where(a => a.UUID.Equals(model.UUID)).FirstOrDefault();
                if (oldItem != null)
                {
                    MemoryTools.Copy(model, oldItem);
                }
            }
            catch
            {
            }
        }
        return Task.FromResult(true);
    }

    public override Task<bool> DeleteAsync(IEnumerable<TModel> items)
    {
        items.ToList().ForEach(i => Items.Remove(i));
        return Task.FromResult(true);
    }

    private static readonly ConcurrentDictionary<Type, Func<IEnumerable<TModel>, string, SortOrder, IEnumerable<TModel>>> SortLambdaCache = new();
    private int VirtualScrollPageItemsCache = 0;
    public override Task<QueryData<TModel>> QueryAsync(QueryPageOptions options)
    {

        IEnumerable<TModel> items = Items ?? new();

        var isSearch = options.Searches.Any() || options.CustomerSearches.Any() || options.AdvanceSearches.Any();

        // 处理 Searchable=true 列与 SeachText 模糊搜索
        if (options.Searches.Any())
        {
            //类加入 [AutoGenerateClass(Filterable = true, Sortable = true, TextWrap = true, Searchable = true)] 就能搜索了

            items = items.Where(options.Searches.GetFilterFunc<TModel>(FilterLogic.Or));
        }
        else
        {
            // 处理 SearchText 模糊搜索
            if (!string.IsNullOrEmpty(options.SearchText))
            {
                //TODO : LazyHero SearchText 模糊搜索 未实现
                // items = items.Where(item => (item.Name?.Contains(options.SearchText) ?? false)|| (item.Address?.Contains(options.SearchText) ?? false));
            }
        }


        // 过滤
        var isFiltered = false;
        if (options.Filters.Any())
        {
            items = items.Where(options.Filters.GetFilterFunc<TModel>());

            // 通知内部已经过滤数据了
            isFiltered = true;
        }

        // 排序
        var isSorted = false;
        if (!string.IsNullOrEmpty(options.SortName))
        {
            var invoker = SortLambdaCache.GetOrAdd(typeof(TModel), key => LambdaExtensions.GetSortLambda<TModel>().Compile());
            items = invoker(items, options.SortName, options.SortOrder);
            isSorted = true;
        }

        // 设置记录总数
        var total = items.Count();

        // 内存分页
        if (options.IsPage)
        {
            items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems);
        }
        else if (options.IsVirtualScroll)
        {
            //缓存虚拟滚动查询模式分页数
            if (VirtualScrollPageItemsCache == 0 || options.PageItems > VirtualScrollPageItemsCache)
            {
                VirtualScrollPageItemsCache = options.PageItems;
            }
            items = items.Skip(options.StartIndex).Take(VirtualScrollPageItemsCache);
        }
        ItemsResult = items.ToList();

        return Task.FromResult(new QueryData<TModel>()
        {
            Items = items,
            TotalCount = total,
            IsSorted = isSorted,
            IsFiltered = isFiltered,
            IsSearch = true
        });

    }

}

internal class MemoryTools
{

    /// <summary>
    /// 泛型 Copy 方法
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static void Copy<TModel>(TModel source, TModel destination) where TModel : class
    {
        if (source != null && destination != null)
        {
            var type = source.GetType();
            //if (type.IsClass)
            //{
            var valType = destination.GetType();
            if (valType != null)
            {
                type.GetFields().ToList().ForEach(f =>
                {
                    var v = f.GetValue(source);
                    valType.GetField(f.Name)?.SetValue(destination, v);
                });
                type.GetProperties().ToList().ForEach(p =>
                {
                    if (p.CanWrite)
                    {
                        var v = p.GetValue(source);
                        valType.GetProperty(p.Name)?.SetValue(destination, v);
                    }
                });
            }
            //}
        }
    }

}

/// <summary>
/// Object 扩展方法
/// </summary>
internal static partial class ObjectsExtensions
{
    /// <summary>
    /// 获取属性
    /// </summary>
    /// <param name="instance">object</param>
    /// <param name="propertyName">需要判断的属性</param>
    /// <returns>是否包含</returns>
    public static object? GetField<TItem>(this TItem instance, string propertyName)
    {

        if (instance != null && !string.IsNullOrEmpty(propertyName))
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            return propertyInfo;
        }
        return null;
    }


}
