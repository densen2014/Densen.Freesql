// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using Densen.DataAcces.FreeSql;
using Densen.Service;
using FreeSql.Internal.Model;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Densen.Components;

public partial class TableAdv<TItem> : BootstrapComponentBase
    where TItem : class, new()
{

    [Inject]
    [NotNull]
    private NavigationManager? NavigationManager { get; set; }

    [Inject]
    [NotNull]
    protected IImportExport? Exporter { get; set; }

    [Inject]
    [NotNull]
    private ToastService? ToastService { get; set; }

    [NotNull]
    private Table<TItem>? mainTable { get; set; }

    [Parameter] public string? PinPaiName { get; set; }

    [Parameter] public string? QuDaoShangName { get; set; }

    [Parameter] public string? TuiGuangShangName { get; set; }

    // 由于使用了FreeSql ORM 数据服务,可以直接取对象
    [Inject]
    [NotNull]
    private IFreeSql? fsql { get; set; }

    [Inject] private ToastService? toastService { get; set; }
    [Inject] private SwalService? SwalService { get; set; }

    #region 数据服务

    [Inject]
    [NotNull]
    private FreeSqlDataService<TItem>? DataService { get; set; }

#nullable disable

    /// <summary>
    /// 获得/设置 注入数据服务
    /// </summary>
    [Inject] protected IEnumerable<FreeSqlDataService<TItem>> DataServices { get; set; }
    /// <summary>
    /// 保存数据后异步回调方法
    /// </summary>
    [Parameter] public EventCallback<TItem> AfterSaveAsync { get; set; }


    /// <summary>
    /// 动态附加查询条件, 主键字段数值
    /// </summary>
    [Parameter]
    public object FieldValue { get; set; }

    public object FieldValueD { get; set; }

    /// <summary>
    /// 动态附加查询条件, 主键字段名称
    /// </summary>
    [Parameter]
    public string Field { get; set; } = "ID";

    /// <summary>
    /// 动态附加查询条件, 详表关联字段名称, 当Field!=FieldD才需要设置
    /// </summary>
    [Parameter]
    public string FieldD { get; set; } = null;

    /// <summary>
    /// 动态附加查询条件, 主键字段类型
    /// </summary>
    [Parameter]
    public Type FieldType { get; set; } = typeof(int);

    /// <summary>
    /// 动态附加FreeSql实例
    /// </summary>
    [Parameter]
    public IFreeSql Fsql { get; set; }

    /// <summary>
    /// 级联保存字段名
    /// </summary>
    [Parameter] public string SaveManyChildsPropertyName { get; set; }

    /// <summary>
    /// 附加导航IncludeByPropertyName查询条件 <para></para>
    /// 例如 : <para></para>
    ///       new List&lt;string&gt; { "ProductsNamePrice","customers" } <para></para>
    /// 或: 直接附加最终关系<para></para>
    ///       new List&lt;string&gt; { "Orders.Customers1","Orders.Employes1" } 
    /// </summary>
    [Parameter] public List<string> IncludeByPropertyNames { get; set; }

    /// <summary>
    /// 左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")
    /// </summary>
    [Parameter] public string LeftJoinString { get; set; }

    /// <summary>
    /// 强制排序,但是手动排序优先
    /// 例如 : new List&lt;string&gt; { "ProductsNamePrice","customers" }
    /// </summary>
    [Parameter] public List<string> OrderByPropertyName { get; set; }

    /// <summary>
    /// 附加查询条件使用or结合 <para></para>
    /// 例如 : <para></para>
    ///       new List&lt;string&gt; { "ProductsNamePrice","customers" } <para></para>
    /// 或: 直接附加最终关系<para></para>
    ///       new List&lt;string&gt; { "Orders.Customers1","Orders.Employes1" } 
    /// </summary>
    [Parameter] public List<string> WhereCascadeOr { get; set; }

    public IEnumerable<TItem> ItemsCache { get; set; }

    /// <summary>
    /// 附加复杂查询条件
    /// </summary>
    [Parameter] public DynamicFilterInfo DynamicFilterInfo { get; set; }

    /// <summary>
    /// 附加查询条件，使用and结合
    /// </summary>
    public DynamicFilterInfo dynamicFilterInfo
    {
        get => DynamicFilterInfo ?? ((FieldValue == null && FieldValueD == null) ? null : new DynamicFilterInfo()
        {
            Field = FieldD ?? Field,
            Operator = DynamicFilterOperator.Equal,
            Value = FieldValue ?? FieldValueD,
        });
    }

    protected FreeSqlDataService<TItem> GetDataService()
    {
        if (DataServices.Any())
        {
            DataServices.Last().SaveManyChildsPropertyName = SaveManyChildsPropertyName;
            if (Fsql != null)
            {
                DataServices.Last().fsql = Fsql;
            }

            return DataServices.Last();
        }
        else
        {
            throw new InvalidOperationException("DataServiceInvalidOperationText");
        }
    }

    public Task<TItem> OnAddAsync()
    {
        var newone = new TItem();
        if (FieldValue != null || FieldValueD != null)
        {
            newone.FieldSetValue(Field, FieldValue ?? FieldValueD);
        }

        return Task.FromResult(newone);
    }

    public async Task<QueryData<TItem>> OnQueryAsync(QueryPageOptions options)
    {
        var items1 = await GetDataService().QueryAsyncWithWhereCascade(
        options,
                dynamicFilterInfo,
                IncludeByPropertyNames,
                LeftJoinString,
                OrderByPropertyName,
                WhereCascadeOr
            );
        ItemsCache = items1.Items;
        return items1;
    }

    public async Task<bool> OnSaveAsync(TItem item, ItemChangedType changedType)
    {
        var res = await GetDataService().SaveAsync(item, changedType);
        if (AfterSaveAsync.HasDelegate)
        {
            await AfterSaveAsync.InvokeAsync(item);
        }

        return res;
    }

    public async Task<bool> OnDeleteAsync(IEnumerable<TItem> items) => await GetDataService().DeleteAsync(items);

    /// <summary>
    /// 全部记录
    /// </summary>
    public List<TItem> GetAllItems() => GetDataService().GetAllItems(
                dynamicFilterInfo,
                IncludeByPropertyNames,
                LeftJoinString,
                OrderByPropertyName,
                WhereCascadeOr
            );

#nullable enable

    #endregion

    public bool IsExcel { get; set; }
    public bool DoubleClickToEdit { get; set; } = true;
    protected string UploadPath = "";
    protected string? uploadstatus;
    private long maxFileSize = 1024 * 1024 * 15;
    private string? tempfilename;

    private AggregateType Aggregate { get; set; }


    protected override async void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            await mainTable!.QueryAsync();
        }
    }

    private Task IsExcelToggle()
    {
        IsExcel = !IsExcel;
        DoubleClickToEdit = !IsExcel;
        StateHasChanged();
        return Task.CompletedTask;
    }

    #region 导入导出
    public async Task<bool> Export模板Async()
    {
        await Export(exportTemplate: true);
        return true;
    }

    private async Task<bool> ExportExcelAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Excel);
    private async Task<bool> ExportPDFAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Pdf);
    private async Task<bool> ExportWordAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Word);
    private async Task<bool> ExportHtmlAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Html);

    private async Task<bool> ExportAutoAsync(IEnumerable<TItem> items, ExportType exportType = ExportType.Excel)
    {
        if (items == null || !items.Any())
        {
            await ToastService.Error("提示", "无数据可导出");
            return false;
        }
        var option = new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "提示",
            Content = $"导出正在执行,请稍等片刻...",
            IsAutoHide = false
        };
        // 弹出 Toast
        await ToastService.Show(option);
        await Task.Delay(100);


        // 开启后台进程进行数据处理
        await Export(items?.ToList(), exportType);

        // 关闭 option 相关联的弹窗
        option.Close();

        // 弹窗告知下载完毕
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "提示",
            Content = $"导出成功,请检查数据",
            IsAutoHide = false
        });
        return true;

    }

    private async Task Export(List<TItem>? items = null, ExportType exportType = ExportType.Excel, bool exportTemplate = false)
    {
        try
        {
            if (!exportTemplate && (items == null || !items.Any()))
            {
                ToastService?.Error($"导出", $"{exportType}出错,无数据可导出");
                return;
            }
            var fileName = items == null ? "模板" : typeof(TItem).Name;
            var fullName = Path.Combine(UploadPath, fileName);
            fullName = await Exporter.Export(fullName, items, exportType);
            fileName = (new System.IO.FileInfo(fullName)).Name;
            ToastService?.Success("提示", fileName + "已生成");

            //下载后清除文件
            NavigationManager.NavigateTo($"uploads/{fileName}", true);
            _ = Task.Run(() =>
            {
                Thread.Sleep(50000);
                System.IO.File.Delete(fullName);
            });

        }
        catch (Exception e)
        {
            ToastService?.Error($"导出", $"{exportType}出错,请检查. {e.Message}");
        }
    }

    public async Task<bool> EmptyAll()
    {
        fsql.Delete<TItem>().Where(a => 1 == 1).ExecuteAffrows();
        await ToastService!.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "提示",
            Content = "已清空数据",
        });

        await mainTable!.QueryAsync();
        return true;
    }
    private async Task ImportExcel()
    {
        if (string.IsNullOrEmpty(tempfilename))
        {
            ToastService?.Error("提示", "请正确选择文件上传");
            return;
        }
        var option = new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "提示",
            Content = "导入文件中,请稍等片刻...",
            IsAutoHide = false
        };
        // 弹出 Toast
        await ToastService!.Show(option);
        await Task.Delay(100);


        // 开启后台进程进行数据处理
        var isSuccess = await MockImportExcel();

        // 关闭 option 相关联的弹窗
        option.Close();

        // 弹窗告知下载完毕
        await ToastService.Show(new ToastOption()
        {
            Category = isSuccess ? ToastCategory.Success : ToastCategory.Error,
            Title = "提示",
            Content = isSuccess ? "操作成功,请检查数据" : "出现错误,请重试导入或者上传",
            IsAutoHide = false
        });

        await mainTable!.QueryAsync();
    }

    private async Task<bool> MockImportExcel()
    {
        var items_temp = await ImportExportsService.ImportFormExcel<TItem>(tempfilename!);
        if (items_temp.items == null)
        {
            ToastService?.Error("提示", "文件导入失败: " + items_temp.error);
            return false;
        }
        //items = SmartCombine(items_temp, items).ToList(); 新数据和老数据合并处理,略100字
        await fsql.Insert<TItem>().AppendData(items_temp!.items.ToList()).ExecuteAffrowsAsync();
        return true;
    }

    /// <summary>
    /// 导出数据方法
    /// </summary>
    /// <param name="Items"></param>
    /// <param name="opt"></param>
    /// <returns></returns>
    protected async Task<bool> ExportAsync(ITableExportDataContext<TItem> items)
    {
        var ret = await ExportExcelAsync(items.Rows);
        return ret;
    }
    #endregion


    [Inject]
    [NotNull]
    private DialogService? DialogService { get; set; }


}

public static partial class ObjectExtensions
{
#nullable disable 
    /// <summary>
    /// 创建属性
    /// </summary>
    /// <param name="instance">object</param>
    /// <param name="propertyName">需要判断的属性</param>
    /// <param name="value"></param> 
    /// <returns>是否包含</returns>
    public static object FieldSetValue<TItem>(this TItem instance, string propertyName, object value)
    {

        if (instance != null && !string.IsNullOrEmpty(propertyName))
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            propertyInfo.SetValue(instance, value);
            return value;
        }
        return null;
    }

    /// <summary>
    /// 获取属性
    /// </summary>
    /// <param name="instance">object</param>
    /// <param name="propertyName">需要判断的属性</param>
    /// <returns>是否包含</returns>
    public static object GetField<TItem>(this TItem instance, string propertyName)
    {

        if (instance != null && !string.IsNullOrEmpty(propertyName))
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            return propertyInfo;
        }
        return null;
    }
}
