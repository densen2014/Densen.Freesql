// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using Densen.Service;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using static AME.EnumsExtensions;
using Alignment = BootstrapBlazor.Components.Alignment;

namespace AmeBlazor.Components;

/// <summary>
/// TablePollo 组件基类
/// </summary>
public partial class TableAmeBase : BootstrapComponentBase
{

    [Inject]
    [NotNull]
    protected NavigationManager? NavigationManager { get; set; }

    [Inject]
    [NotNull]
    protected DownloadService? DownloadService { get; set; }


    #region TablePollo 的设置

    /// <summary>
    /// 指定数据库连接字符串
    /// </summary>
    [Parameter]
    public string? ibstring { get; set; }

    [Inject]
    [NotNull]
    public DialogService? DialogService { get; set; }

    [Inject]
    [NotNull]
    public ToastService? ToastService { get; set; }

    /// <summary>
    /// 获得/设置 IJSRuntime 实例
    /// </summary>
    [Inject]
    [NotNull]
    public IJSRuntime? JsRuntime { get; set; }

    [Inject]
    [NotNull]
    public IImportExport? Exporter { get; set; }

    /// <summary>
    /// 动态附加查询条件, 主键字段名称
    /// </summary>
    [Parameter]
    public string? Field { get; set; } = "ID";

    /// <summary>
    /// 动态附加查询条件, 详表关联字段名称, 当Field!=FieldD才需要设置
    /// </summary>
    [Parameter]
    public string? FieldD { get; set; } = null;

    /// <summary>
    /// 动态附加查询条件II, 主键字段名称
    /// </summary>
    [Parameter]
    public string? FieldII { get; set; } = null;

    /// <summary>
    /// 动态附加查询条件, 详表关联字段名称, FieldII!=FieldIID才需要设置
    /// </summary>
    [Parameter]
    public string? FieldIID { get; set; } = null;

    /// <summary>
    /// 动态附加查询条件III, 主键字段名称
    /// </summary>
    [Parameter]
    public string? FieldIII { get; set; } = null;

    /// <summary>
    /// 动态附加查询条件, 详表关联字段名称, FieldIII!=FieldIIID才需要设置
    /// </summary>
    [Parameter]
    public string? FieldIIID { get; set; } = null;

    /// <summary>
    /// 动态附加查询条件, 主键字段数值
    /// </summary>
    [Parameter]
    public object? FieldValue { get; set; }

    public object? FieldValueD { get; set; }

    /// <summary>
    /// 动态附加查询条件, 主键字段类型
    /// </summary>
    [Parameter]
    public Type FieldType { get; set; } = typeof(int);

    /// <summary>
    /// 图片列参数,图片列参数集合优先
    /// </summary>
    [Parameter]
    public TableImgField? TableImgField { get; set; }

    /// <summary>
    /// 详表图片列参数,图片列参数集合优先
    /// </summary>
    [Parameter]
    public TableImgField? SubTableImgField { get; set; }

    /// <summary>
    /// 图片列参数集合,优先读取
    /// </summary>
    [Parameter]
    public List<TableImgField>? TableImgFields { get; set; }

    /// <summary>
    /// 详表图片列参数集合,优先读取
    /// </summary>
    [Parameter]
    public List<TableImgField>? SubTableImgFields { get; set; }

    /// <summary>
    /// 附加操作列参数集合,优先读取
    /// </summary>
    [Parameter]
    public List<TableImgField>? TableFunctionsFields { get; set; }

    /// <summary>
    /// 详表附加操作参数集合,优先读取
    /// </summary>
    [Parameter]
    public List<TableImgField>? SubTableFunctionsFields { get; set; }

    /// <summary>
    /// 附加行内操作按钮参数集合,优先读取
    /// </summary>
    [Parameter]
    public List<RowButtonField>? RowButtons { get; set; }

    /// <summary>
    /// 详表附加行内操作按钮参数集合,优先读取
    /// </summary>
    [Parameter]
    public List<RowButtonField>? SubRowButtons { get; set; }

    /// <summary>
    /// 详表附加组件布局方式 默认为 Auto
    /// </summary>
    [Parameter]
    public TableRenderMode? SubRenderMode { get; set; }

    /// <summary>
    /// 附加导航IncludeByPropertyName查询条件, 单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. (暂时只支持一个then附加) <para></para>
    /// 例如 : <para></para>
    ///       new List&lt;string&gt; { "ProductsNamePrice","customers" } <para></para>
    /// 或: 直接附加最终关系<para></para>
    ///       new List&lt;string&gt; { "Orders.Customers1","Orders.Employes1" } <para></para>
    /// 或: 附加查询条件的第二个参数 then<para></para>
    ///       new List&lt;string&gt; { "Orders,Employes"}  对应linq为:IncludeByPropertyName("Orders",then => then.IncludeByPropertyName("Employes")<para></para>
    /// </summary>
    [Parameter] public List<string>? IncludeByPropertyNames { get; set; }

    /// <summary>
    /// 附加查询条件使用or结合 <para></para>
    /// </summary>
    [Parameter] public List<string>? WhereCascadeOr { get; set; }

    /// <summary>
    /// 详表附加导航IncludeByPropertyName查询条件 <para></para>
    /// 例如 : new List&lt;string&gt; { "ProductsNamePrice","customers" }
    /// </summary>
    [Parameter] public List<string>? SubIncludeByPropertyNames { get; set; }

    /// <summary>
    /// 详表附加导航IncludeByPropertyNameII查询条件 <para></para>
    /// 例如 : new List&lt;string&gt; { "ProductsNamePrice","customers" }
    /// </summary>
    [Parameter] public List<string>? SubIncludeByPropertyNamesII { get; set; }

    /// <summary>
    /// 详表附加导航IncludeByPropertyNameIII查询条件 <para></para>
    /// 例如 : new List&lt;string&gt; { "ProductsNamePrice","customers" }
    /// </summary>
    [Parameter] public List<string>? SubIncludeByPropertyNamesIII { get; set; }

    /// <summary>
    /// 左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")
    /// </summary>
    [Parameter] public string? LeftJoinString { get; set; }

    /// <summary>
    /// 强制排序,但是手动排序优先
    /// 例如 : new List&lt;string&gt; { "ProductsNamePrice","customers" }
    /// </summary>
    [Parameter] public List<string>? OrderByPropertyName { get; set; }

    /// <summary>
    /// 获得/设置 明细弹窗还是表内明细
    /// </summary>
    [Parameter] public ShowDetailRowType ShowDetailRowType { get; set; } = ShowDetailRowType.表内明细;

    /// <summary>
    /// 获得/设置 表内明细名称1
    /// </summary>
    [Parameter] public string? DetailRowsName1 { get; set; } = "详表";

    /// <summary>
    /// 获得/设置 表内明细名称2
    /// </summary>
    [Parameter] public string? DetailRowsName2 { get; set; } = "Logs";

    /// <summary>
    /// 获得/设置 表内明细名称3
    /// </summary>
    [Parameter] public string? DetailRowsName3 { get; set; }

    /// <summary>
    /// 获得/设置 显示导入,执行添加工具栏按钮
    /// </summary>
    [Parameter] public ShowToolbarType ShowToolbarType { get; set; } = ShowToolbarType.无;

    [Parameter] public Func<Task>? Excel导入 { get; set; }

    [Parameter] public string? Excel导入文本 { get; set; } = "Excel导入";

    [Parameter] public Func<Task>? 导入 { get; set; }

    [Parameter] public string? 导入文本 { get; set; } = "导入";

    [Parameter] public Func<Task>? 导入II { get; set; }

    [Parameter] public string? 导入II文本 { get; set; } = "导入II";

    [Parameter] public Func<Task>? 执行添加 { get; set; }

    [Parameter] public string? 执行添加文本 { get; set; } = "执行添加";

    [Parameter] public string? 导出文本 { get; set; } = "导出";

    [Parameter] public string? 批量按钮文字 { get; set; } = "批量";

    [Parameter] public string? 升级按钮文字 { get; set; } = "升级";

    [Parameter] public string? 升级按钮II文字 { get; set; } = "升级II";

    [Parameter] public string? 打印文本 { get; set; } = "打印";

    [Parameter] public string? 新窗口打开文字 { get; set; } = "新窗口打开";

    [Parameter] public string? 新窗口打开Url { get; set; }

    [Parameter] public string? 查找文本I文字 { get; set; } = "查询条件";

    [Parameter] public string? 查找文本I { get; set; }


    [Parameter] public int Footercolspan1 { get; set; } = 3;

    [Parameter] public int Footercolspan2 { get; set; } = 2;

    [Parameter] public int Footercolspan3 { get; set; }

    [Parameter] public int FootercolspanTotal { get; set; } = 2;

    [Parameter] public string? FooterText { get; set; } = "合计：";

    [Parameter] public string? FooterText2 { get; set; }

    [Parameter] public string? FooterText3 { get; set; }

    [Parameter] public string? FooterTotal { get; set; }

    [Parameter] public Alignment FooterAlign { get; set; } = Alignment.Right;

    [Parameter] public AggregateType FooterAggregate { get; set; } = AggregateType.Sum;

    [Parameter] public string? FooterTotalColName { get; set; }

    /// <summary>
    /// 显示漂浮面板
    /// </summary>
    [Parameter] public bool ShowFloatPanel { get; set; }

    /// <summary>
    /// 显示工具栏漂浮面板
    /// </summary>
    [Parameter] public bool ShowFloatPanelToolbar { get; set; }

    /// <summary>
    /// 使用 MiniExcel 库导出,默认为 true
    /// </summary>
    [Parameter] public bool UseMiniExcel { get; set; } = true;

    /// <summary>
    /// 默认使用内置 MiniExcel/MiniWord 导入导出服务. <para></para>
    /// 如果要使用 ImportExportsService 库全功能导出, 需要自己拉包 BootstrapBlazor.Table.ImportExportsService 注入服务,<para></para>
    /// 默认为 false
    /// </summary>
    [Parameter] public bool UseFullExportService { get; set; }


    /// <summary>
    /// 使用 UseMiniWord 库导出,默认为 false
    /// </summary>
    [Parameter] public bool UseMiniWord { get; set; }

    /// <summary>
    /// * MiniWord 必须指定模板路径,否则出错. <para></para> 
    /// 默认为 false
    /// </summary>
    [Parameter] public string? MiniWordTemplatePath { get; set; }

    /// <summary>
    /// 级联保存字段名
    /// </summary>
    [Parameter] public string? SaveManyChildsPropertyName { get; set; }

    /// <summary>
    /// 显示调试信息:展开列...
    /// </summary>
    [Parameter] public bool IsDebug { get; set; }

    /// <summary>
    /// 获得/设置 是否显示导出Excel按钮 默认为 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowExportExcelButtons { get; set; }

    /// <summary>
    /// 获得/设置 是否显示导出PDF/Word/Html按钮 默认为 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowExportMoreButtons { get; set; } = true;

    /// <summary>
    /// 获得/设置 导出到流或者文件 默认为 false
    /// </summary>
    [Parameter]
    public bool ExportToStream { get; set; } = false;


    /// <summary>
    /// 获得/设置 导出的本机路径 默认为空
    /// </summary>
    [Parameter]
    public string? ExportBasePath { get; set; }

    /// <summary>
    /// 是否开启 一对一(OneToOne)、一对多(OneToMany)、多对多(ManyToMany) 级联保存功能<para></para>
    /// <para></para>
    /// 【一对一】模型下，保存时级联保存 OneToOne 属性。
    /// <para></para>
    /// 【一对多】模型下，保存时级联保存 OneToMany 集合属性。出于安全考虑我们没做完整对比，只针对实体属性集合的添加或更新操作，因此不会删除数据库表已有的数据。<para></para>
    /// </summary>
    [Parameter]
    public bool EnableCascadeSave { get; set; } = false;

    /// <summary>
    /// 获得/设置 自动保存当前页码 默认 false
    /// </summary>
    [Parameter]
    public bool AutoSavePageIndex { get; set; }

    /// <summary>
    /// 获得/设置 保存当前页码Key 默认 PageIndex
    /// <para></para>多个表格请设置不同的key
    /// </summary>
    [Parameter]
    public string AutoSavePageIndexKey { get; set; } = "PageIndex";

    /// <summary>
    /// 获得/设置 当前页码
    /// </summary>
    [Parameter]
    public int? PageIndex { get; set; }

    public int? PageIndexCache { get; set; }

    /// <summary>
    /// 主表只读,默认为 false
    /// </summary>
    [Parameter] public bool IsReadonly { get; set; }

    #region StorageService
    public async Task StorageSetValue<TValue>(string key, TValue value)
    {
        await JSRuntime.InvokeVoidAsync("eval", $"localStorage.setItem('{key}', '{value}')");
    }

    public async Task<TValue?> StorageGetValue<TValue>(string key, TValue? def)
    {
        try
        {
            var cValue = await JSRuntime.InvokeAsync<TValue>("eval", $"localStorage.getItem('{key}');");
            return cValue ?? def;
        }
        catch
        {
            var cValue = await JSRuntime.InvokeAsync<string>("eval", $"localStorage.getItem('{key}');");
            if (cValue == null)
            {
                return def;
            }

            var newValue = StorageGetValueI<TValue>(cValue);
            return newValue ?? def;

        }
    }
    public static T? StorageGetValueI<T>(string value)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null)
        {
            return (T?)converter.ConvertFrom(value);
        }
        return default;
        //return (T)Convert.ChangeType(value, typeof(T));
    }
    #endregion
    #endregion

    #region 继承bb table的设置
    /// <summary>
    /// 获得/设置 是否斑马线样式 默认为 true
    /// </summary>
    [Parameter]
    public bool IsStriped { get; set; } = true;


    /// <summary>
    /// 获得/设置 是否带边框样式 默认为 true
    /// </summary>
    [Parameter]
    public bool IsBordered { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否自动刷新表格 默认为 false
    /// </summary>
    [Parameter]
    public bool IsAutoRefresh { get; set; }

    /// <summary>
    /// 获得/设置 自动刷新时间间隔 默认 2000 毫秒
    /// </summary>
    [Parameter]
    public int AutoRefreshInterval { get; set; } = 2000;

    /// <summary>
    /// 获得/设置 点击行即选中本行 默认为 false
    /// </summary>
    [Parameter]
    public bool ClickToSelect { get; set; }

    /// <summary>
    /// 获得/设置 单选模式下双击即编辑本行 默认为 true
    /// </summary>
    [Parameter]
    public bool DoubleClickToEdit { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否自动生成列信息 默认为 true
    /// </summary>
    [Parameter]
    public bool AutoGenerateColumns { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示工具栏 默认 true 显示
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示新建按钮 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool ShowAddButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示编辑按钮 默认为 true 行内是否显示请使用 <see cref="ShowExtendEditButton"/> 与 <see cref="ShowEditButtonCallback" />
    /// </summary>
    [Parameter]
    public bool ShowEditButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示删除按钮 默认为 true 行内是否显示请使用 <see cref="ShowExtendDeleteButton"/> 与 <see cref="ShowDeleteButtonCallback" />
    /// </summary>
    [Parameter]
    public bool ShowDeleteButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否自动收缩工具栏按钮 默认 true
    /// </summary>
    [Parameter]
    public bool IsAutoCollapsedToolbarButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示行内扩展编辑按钮 默认 true 显示
    /// </summary>
    [Parameter]
    public bool ShowExtendEditButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示行内扩展编辑按钮 默认 true 显示
    /// </summary>
    [Parameter]
    public bool ShowExtendDeleteButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示Excel自由编辑模式 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool ShowExcelModeButtons { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示添加/编辑/删除按钮 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool ShowDefaultButtons { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示扩展按钮 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool ShowExtendButtons { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否分页 默认为 true
    /// </summary>
    [Parameter]
    public bool IsPagination { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否分页 默认为 true
    /// </summary>
    [Parameter]
    public bool? SubIsPagination { get; set; }

    /// <summary>
    /// 获得/设置 是否为多选模式 默认为 true 显示
    /// </summary>
    [Parameter] public bool IsMultipleSelect { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示搜索框 默认为 true 显示搜索框
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    /// <summary>
    /// 获得/设置 搜索栏渲染模式 默认 Popup 弹窗形式
    /// </summary>
    [Parameter]
    public SearchMode SearchMode { get; set; } = SearchMode.Popup;

    /// <summary>
    /// 获得/设置 是否显示搜索按钮 默认为 true 
    /// </summary>
    [Parameter]
    public bool ShowSearchButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示高级搜索 默认为 true 
    /// </summary>
    [Parameter]
    public bool ShowAdvancedSearch { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示清空搜索按钮 默认为 true 
    /// </summary>
    [Parameter]
    public bool ShowResetButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否收缩顶部搜索框 默认为 true 不收缩搜索框 是否显示搜索框请设置 <see cref="SearchMode"/> 值 Top
    /// </summary>
    [Parameter]
    public bool CollapsedTopSearch { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示加载骨架屏 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowSkeleton { get; set; }

    /// <summary>
    /// 获得/设置 是否显示每行的明细行展开图标
    /// </summary>
    [Parameter]
    public bool ShowDetailRowS { get; set; } = false;

    /// <summary>
    /// 获得/设置 组件编辑模式 默认为弹窗编辑行数据 PopupEditForm
    /// </summary>
    [Parameter]
    public EditMode EditMode { get; set; }

    /// <summary>
    /// 获得/设置 组件编辑模式 默认为弹窗编辑行数据 PopupEditForm
    /// </summary>
    [Parameter]
    public EditMode? SubEditMode { get; set; }

    ///// <summary>
    ///// 获得/设置 TableHeader 实例
    ///// </summary>
    //[Parameter]
    //public RenderFragment<TItem> TableColumns { get; set; }

    /// <summary>
    /// 获得/设置 分页记录起始数量
    /// </summary>
    [Parameter]
    public int? PageItemsSourceStart { get; set; }

    /// <summary>
    /// 获取/设置 表格 thead 样式 <see cref="TableHeaderStyle"/>，默认为浅色<see cref="TableHeaderStyle.None"/>
    /// </summary>
    [Parameter]
    public TableHeaderStyle HeaderStyle { get; set; } = TableHeaderStyle.None;

    /// <summary>
    /// 获取/设置 详细表格 thead 样式 <see cref="TableHeaderStyle"/>，默认为浅色<see cref="TableHeaderStyle.Light"/>
    /// </summary>
    [Parameter]
    public TableHeaderStyle HeaderStyleDetails { get; set; } = TableHeaderStyle.Light;

    /// <summary>
    /// 获得/设置 表格组件大小 默认为 Normal 正常模式
    /// </summary>
    [Parameter]
    public TableSize TableSize { get; set; }

    /// <summary>
    /// 获得/设置 详细表格组件大小 默认为 Compact 模式
    /// </summary>
    [Parameter]
    public TableSize TableSizeDetails { get; set; } = BootstrapBlazor.Components.TableSize.Compact;

    /// <summary>
    /// 获得/设置 默认每页数据数量 默认 0 使用 <see cref="PageItemsSource"/> 第一个值
    /// </summary>
    [Parameter]
    public int? SubPageItems { get; set; }

    /// <summary>
    /// 获得/设置 
    /// </summary>
    [Parameter]
    public int? SubHeight { get; set; }

    /// <summary>
    /// 获得/设置 是否允许列宽度调整 默认 false 固定表头时此属性生效
    /// </summary>
    [Parameter]
    public bool AllowResizing { get; set; }

    /// <summary>
    /// 获得/设置 是否显示列选择下拉框 默认为 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowColumnList { get; set; }

    /// <summary>
    /// 获得/设置 是否显示行号
    /// </summary>
    [Parameter]
    public bool ShowLineNo { get; set; }

    /// <summary>
    /// 获得/设置 是否显示表脚 默认为 false
    /// </summary>
    [Parameter]
    public bool ShowFooter { get; set; }

    ///// <summary>
    ///// 获得/设置 双击行回调委托方法
    ///// </summary>
    //[Parameter]
    //public Func<TItem, Task>? OnDoubleClickRowCallback { get; set; }

    /// <summary>
    /// 获得/设置 是否显示刷新按钮 默认为 true
    /// </summary>
    [Parameter]
    public bool ShowRefresh { get; set; } = true;

    /// <summary>
    /// 获得/设置 每页显示数据数量的外部数据源
    /// </summary>
    [Parameter]
    public IEnumerable<int> PageItemsSource { get; set; } = new int[] { 20, 50, 100, 200, 500, 1000 };

    /// <summary>
    /// 获得/设置 组件是否采用 Tracking 模式对编辑项进行直接更新 默认 false
    /// </summary>
    [Parameter]
    public bool IsTracking { get; set; }

    /// <summary>
    /// 获得/设置 组件工作模式为 Excel 模式 默认 false
    /// </summary>
    [Parameter]
    public bool IsExcel { get; set; }

    /// <summary>
    /// 获得/设置 固定表头 默认 false
    /// </summary>
    /// <remarks>固定表头时设置 <see cref="Height"/> 即可出现滚动条，未设置时尝试自适应</remarks>
    [Parameter]
    public bool IsFixedHeader { get; set; }

    /// <summary>
    /// 获得/设置 是否显示导出按钮 默认为 false 显示
    /// </summary>
    [Parameter]
    public bool ShowExportButton { get; set; }

    /// <summary>
    /// 获得/设置 导出按钮图标 默认为 fa-solid fa-download
    /// </summary>
    [Parameter]
    public string? ExportButtonIcon { get; set; }

    /// <summary>
    /// 获得/设置 Table 高度 默认为 null
    /// </summary>
    /// <remarks>开启固定表头功能时生效 <see cref="IsFixedHeader"/></remarks>

    [Parameter]
    public int? Height { get; set; }

    ///// <summary>
    ///// 获得/设置 多表头模板
    ///// </summary>
    //[Parameter]
    //public RenderFragment? MultiHeaderTemplate { get; set; }

    /// <summary>
    /// 获得/设置 数据滚动模式
    /// </summary>
    [Parameter]
    public ScrollMode ScrollMode { get; set; }
    protected ScrollMode scrollMode;

    /// <summary>
    /// 获得/设置 是否显示设置数据滚动模式模式 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool ShowScrollModeButtons { get; set; } = true;

    /// <summary>
    /// 获得/设置 明细表数据滚动模式
    /// </summary>
    [Parameter]
    public ScrollMode? SubScrollMode { get; set; }

    /// <summary>
    /// 获得/设置 虚拟滚动行高 默认为 39.5
    /// </summary>
    /// <remarks>需要设置 <see cref="ScrollMode"/> 值为 Virtual 时生效</remarks>
    [Parameter]
    public float RowHeight { get; set; } = 39.5f;

    /// <summary>
    /// 获得/设置 组件布局方式 默认为 Auto
    /// </summary>
    [Parameter]
    public TableRenderMode RenderMode { get; set; }

    [Parameter]
    public bool ShowCardView { get; set; } = true;

    /// <summary>
    /// 获得/设置 每行显示组件数量 默认为 2
    /// </summary>
    [Parameter]
    public int EditDialogItemsPerRow { get; set; } = 2;

    /// <summary>
    /// 获得/设置 设置行内组件布局格式 默认 Inline 布局
    /// </summary>
    [Parameter]
    public RowType EditDialogRowType { get; set; } = RowType.Inline;

    /// <summary>
    /// 获得/设置 设置 <see cref="EditDialogRowType" /> Inline 模式下标签对齐方式 默认 None 等效于 Left 左对齐
    /// </summary>
    [Parameter]
    public Alignment EditDialogLabelAlign { get; set; } = Alignment.None;

    /// <summary>
    /// 编辑框的大小
    /// </summary>
    [Parameter]
    public Size EditDialogSize { get; set; } = Size.ExtraLarge;

    /// <summary>
    /// 获得/设置 编辑框是否显示最大化按钮 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool EditDialogShowMaximizeButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 表格 Toolbar 按钮模板
    /// <para>表格工具栏左侧按钮模板，模板中内容出现在默认按钮前面</para>
    /// </summary>
    [Parameter]
    public RenderFragment? TableToolbarBeforeTemplate { get; set; }

    /// <summary>
    /// 获得/设置 表格 Toolbar 按钮模板
    /// <para>表格工具栏左侧按钮模板，模板中内容出现在默认按钮后面</para>
    /// </summary>
    [Parameter]
    public RenderFragment? TableToolbarTemplate { get; set; }

    /// <summary>
    /// 获得/设置 表格 Toolbar 按钮模板
    /// <para>表格工具栏右侧按钮模板，模板中内容出现在默认按钮前面</para>
    /// </summary>
    [Parameter]
    public RenderFragment? TableExtensionToolbarBeforeTemplate { get; set; }

    /// <summary>
    /// 获得/设置 表格 Toolbar 按钮模板
    /// <para>表格工具栏右侧按钮模板，模板中内容出现在默认按钮后面</para>
    /// </summary>
    [Parameter]
    public RenderFragment? TableExtensionToolbarTemplate { get; set; }

    /// <summary>
    /// 获得/设置 表格 列模板
    /// <para>列模板，模板中内容出现在现有列后面</para>
    /// </summary>
    [Parameter]
    public RenderFragment? TableColumnsTemplate { get; set; }

    /// <summary>
    /// 获得/设置 扩展按钮是否在前面 默认 false 在行尾
    /// </summary>
    [Parameter]
    public bool IsExtendButtonsInRowHeader { get; set; }

    /// <summary>
    /// 获得/设置 是否允许拖动列标题调整表格列顺序 默认 false
    /// </summary>
    [Parameter]
    public bool AllowDragColumn { get; set; }

    /// <summary>
    /// 获得/设置 默认每页数据数量 默认 0 使用 <see cref="PageItemsSource"/> 第一个值
    /// </summary>
    [Parameter]
    public int PageItems { get; set; }

    /// <summary>
    /// 获得/设置 详表组件工作模式为 Excel 模式 默认 false
    /// </summary>
    [Parameter]
    public bool SubIsExcel { get; set; }

    /// <summary>
    /// 详表只读,默认为 false
    /// </summary>
    [Parameter] public bool SubIsReadonly { get; set; }

    /// <summary>
    /// 获得/设置 是否表头允许折行 默认 false 不折行 此设置为 true 时覆盖 <see cref="ITableColumn.HeaderTextWrap"/> 参数值
    /// </summary>
    [Parameter]
    public bool HeaderTextWrap { get; set; }

    /// <summary>
    /// 获得/设置 是否显示过滤表头 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowFilterHeader { get; set; }

    protected int PageHeight { get; set; } = 700;

    [Parameter]
    public bool AutoPageHeight { get; set; } = true;

    [Parameter]
    public bool IsFixedFooter { get; set; } = true;

    /// <summary>
    /// 获得/设置 滚动条宽度 默认为 6
    /// </summary>
    [Parameter]
    public int ScrollWidth { get; set; } = 6;

    /// <summary>
    /// 获得/设置 无数据时是否隐藏表格 Footer 默认为 false 不隐藏
    /// </summary>
    [Parameter]
    public bool IsHideFooterWhenNoData { get; set; }

    /// <summary>
    /// 获得/设置 是否显示过滤表头 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowMultiFilterHeader { get; set; }

    /// <summary>
    /// 获得/设置 首次加载时是否自动查询数据 默认 true <see cref="Items"/> 模式下此参数不起作用
    /// </summary>
    [Parameter]
    public bool IsAutoQueryFirstRender { get; set; } = true; 

    #endregion

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (!firstRender)
        {
            return;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!firstRender)
        {
            return;
        }

        if (AutoSavePageIndex)
        {
            PageIndexCache = await StorageGetValue(AutoSavePageIndexKey, 1);
        }
        else
        {
            PageIndexCache = PageIndex;
        }

        scrollMode = ScrollMode;
        PageHeight = await JsRuntime.InvokeAsync<int>("eval", new object[] { "window.innerHeight" });
        if (IsFixedHeader && Height == null)
        {
            Height = PageHeight - 150;
            StateHasChanged();
        }

    }

}
