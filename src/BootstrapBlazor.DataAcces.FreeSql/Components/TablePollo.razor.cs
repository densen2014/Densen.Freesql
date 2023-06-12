// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AME;
using BootstrapBlazor.Components;
using Densen.DataAcces.FreeSql;
using Densen.Service;
using DocumentFormat.OpenXml.Spreadsheet;
using FreeSql.Internal.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using static AME.EnumsExtensions;
using Alignment = BootstrapBlazor.Components.Alignment;

namespace AmeBlazor.Components;

/// <summary>
/// TablePollo 组件基类
/// </summary>
public partial class TbPolloBase : BootstrapComponentBase, IAsyncDisposable
{
    //[Inject]
    //protected Microsoft.AspNetCore.Hosting.IWebHostEnvironment HostEnvironment { get; set; }

    [Inject]
    [NotNull]
    protected NavigationManager? NavigationManager { get; set; }


    #region TablePollo 的设置

    /// <summary>
    /// 指定数据库连接字符串
    /// </summary>
    [Parameter]
    public string? ibstring { get; set; }

    [Inject]
    [NotNull]
    protected DialogService? DialogService { get; set; }

    [Inject]
    [NotNull]
    protected ToastService? ToastService { get; set; }

    [Inject]
    [NotNull]
    protected IImportExport? Exporter { get; set; }

    /// <summary>
    /// 获得/设置 IJSRuntime 实例
    /// </summary>
    [Inject]
    [NotNull]
    protected IJSRuntime? JS { get; set; }

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

    protected Modal? ExtraLargeModal { get; set; }

    [Parameter] public EventCallback<string> Excel导入 { get; set; }

    [Parameter] public string? Excel导入文本 { get; set; } = "Excel导入";

    [Parameter] public EventCallback<string> 导入 { get; set; }

    [Parameter] public string? 导入文本 { get; set; } = "导入";

    [Parameter] public EventCallback<string> 导入II { get; set; }

    [Parameter] public string? 导入II文本 { get; set; } = "导入II";

    [Parameter] public EventCallback<string> 执行添加 { get; set; }

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
    /// 获得/设置 导出到流或者文件 默认为 true 流
    /// </summary>
    [Parameter]
    public bool ExportToStream { get; set; } = true;


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
    /// 获得/设置 是否为多选模式 默认为 true 显示
    /// </summary>
    [Parameter] public bool IsMultipleSelect { get; set; } = true;

    /// <summary>
    /// 获得/设置 是否显示搜索框 默认为 true 显示搜索框
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

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

    #endregion

    public IJSObjectReference? module;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (!firstRender) return;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./_content/Densen.Extensions.BootstrapBlazor/download.js" + "?v=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            }
        }
        catch
        {
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }

}

/// <summary>
/// TablePollo 组件,使用Idlebus注入服务维护表以及详表
/// <para></para>
/// 后两个参数可用NullClass空类
/// </summary>
/// <typeparam name="TItem">主表类名</typeparam>
/// <typeparam name="ItemDetails">详表类名</typeparam>
/// <typeparam name="ItemDetailsII">详表的详表类名</typeparam>
/// <typeparam name="ItemDetailsII">详表3类名</typeparam>
public partial class TablePollo<TItem, ItemDetails, ItemDetailsII, ItemDetailsIII> : TbPolloBase
    where TItem : class, new()
    where ItemDetails : class, new()
    where ItemDetailsII : class, new()
    where ItemDetailsIII : class, new()
{

    /// <summary>
    /// 获得/设置 注入数据服务
    /// </summary>
    [Inject]
    [NotNull]
    protected IEnumerable<FreeSqlDataService<TItem>>? DataServices { get; set; }

    public TItem? SelectOneItem { get; set; }

    [Parameter] public EventCallback<(IEnumerable<TItem>, ExportType)> 导出 { get; set; }

    [Parameter] public Func<IEnumerable<TItem>, Task>? 批量执行 { get; set; }


    [Parameter] public EventCallback<(IEnumerable<TItem>, bool)> 升级 { get; set; }

    [Parameter] public EventCallback<(IEnumerable<TItem>, bool)> 升级II { get; set; }

    /// <summary>
    /// 查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")
    /// </summary>
    [Parameter] public Expression<Func<TItem, bool>>? WhereLamda { get; set; }


    #region 继承bb table的设置

    /// <summary>
    /// 获得/设置 数据服务
    /// </summary>
    [Parameter]
    public IDataService<TItem>? DataService { get; set; }

    /// <summary>
    /// 获得/设置 数据集合，适用于无功能时仅做数据展示使用，高级功能时请使用 <see cref="OnQueryAsync"/> 回调委托
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// 获得/设置 导出按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<IEnumerable<TItem>, QueryPageOptions, Task<bool>>? OnExportAsync { get; set; }

    /// <summary>
    /// 获得/设置 是否显示每行的明细行展开图标
    /// </summary>
    [Parameter]
    public Func<TItem, bool> ShowDetailRow { get; set; } = _ => false;

    /// <summary>
    /// 获得/设置 新建按钮回调方法
    /// </summary>
    [Parameter]
    public Func<TItem, Task<TItem>>? AddAsync { get; set; }

    /// <summary>
    /// 获得/设置 保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<TItem, ItemChangedType, Task<TItem>>? SaveAsync { get; set; }

    /// <summary>
    /// 保存数据后异步回调方法
    /// </summary>
    [Parameter] public EventCallback<(TItem, ItemChangedType)> AfterSaveAsync { get; set; }

    /// <summary>
    /// 获得/设置 单击行回调委托方法
    /// </summary>
    [Parameter]
    public Func<TItem, Task>? OnClickRowCallback { get; set; }

    /// <summary>
    /// 获得/设置 RowButtonTemplate 实例 此模板生成的按钮默认放置到按钮后面 />
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? RowButtonTemplate { get; set; }

    /// <summary>
    /// 获得/设置 EditTemplate 实例
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? EditTemplate { get; set; }

    Table<TItem>? mainTable;

    TablePollo<ItemDetails, ItemDetailsII, ItemDetailsIII, NullClass>? detalisTable;

    public IEnumerable<TItem>? ItemsCache { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ShowDetailRowS)
            ShowDetailRow = _ => true;

        if (PageItemsSourceStart != null && PageItemsSourceStart < PageItemsSource.FirstOrDefault())
        {
            PageItemsSource = PageItemsSource.Append(PageItemsSourceStart.Value).OrderBy(a => a).ToList();
        }

    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (!firstRender) return;
        System.Console.WriteLine($"TablePollo OnAfterRender=> Field: {Field}, FieldValue: {FieldValue}");
    }


    //protected Task<TItem> OnAddAsync() => Task.FromResult(new TItem());
    public async Task<TItem> OnAddAsync()
    {
        var newone = new TItem();
        if (FieldValue != null || FieldValueD != null) newone.FieldSetValue(Field, FieldValue ?? FieldValueD);
        if (AddAsync != null)
        {
            newone = await AddAsync(newone);
        }
        return newone;
    }

    public async Task<bool> OnSaveAsync(TItem item, ItemChangedType changedType)
    {
        if (SaveAsync != null)
        {
            item = await SaveAsync(item, changedType);
        }
        var res = await GetDataService().SaveAsync(item, changedType);
        if (AfterSaveAsync.HasDelegate)
            await AfterSaveAsync.InvokeAsync((item, changedType));
        return res;
    }

    protected FreeSqlDataService<TItem> GetDataService()
    {
        if (DataServices.Any())
        {
            DataServices.Last().SaveManyChildsPropertyName = SaveManyChildsPropertyName;
            DataServices.Last().ibstring = ibstring;
            DataServices.Last().EnableCascadeSave = EnableCascadeSave;
            return DataServices.Last();
        }
        else
        {
            throw new InvalidOperationException("DataServiceInvalidOperationText");
        }
    }

    public async Task<QueryData<TItem>> OnQueryAsync(QueryPageOptions options)
    {
        var items1 = await GetDataService().QueryAsyncWithWhereCascade(
                options,
                dynamicFilterInfo,
                IncludeByPropertyNames,
                LeftJoinString,
                OrderByPropertyName,
                WhereCascadeOr,
                WhereLamda
            );
        ItemsCache = items1.Items;
        System.Console.WriteLine($"[展开]TablePollo OnQueryAsync => Field:{dynamicFilterInfo?.Field} /Value: {dynamicFilterInfo?.Value} , itemsTotalCount: {items1.TotalCount}");
        return items1;
    }

    //public async Task<Func<TItem, Task>> OnAfterSaveAsync(TItem item)
    //{
    //    if (AfterSaveAsync.HasDelegate)
    //        await AfterSaveAsync.InvokeAsync(item);
    //    return Task.FromResult;
    //}

    public async Task<bool> OnDeleteAsync(IEnumerable<TItem> items) => await GetDataService().DeleteAsync(items);

    /// <summary>
    /// 查询按钮调用此方法
    /// </summary>
    /// <returns></returns>
    public async Task QueryAsync()
    {
        await mainTable!.QueryAsync();
    }

    /// <summary>
    /// 附加复杂查询条件
    /// </summary>
    [Parameter] public DynamicFilterInfo? DynamicFilterInfo { get; set; }

    /// <summary>
    /// 附加查询条件，使用and结合
    /// </summary>
    public DynamicFilterInfo? dynamicFilterInfo
    {
        get => DynamicFilterInfo ?? ((FieldValue == null && FieldValueD == null) ? null : new DynamicFilterInfo()
        {
            Field = FieldD ?? Field,
            Operator = DynamicFilterOperator.Equal,
            Value = FieldValue ?? FieldValueD,
        });
    }

    #region 动态生成控件
    /// <summary>
    /// 动态获取表达式
    /// </summary>
    /// <param name="model"></param>
    /// <param name="field">列名,默认"ID"</param>
    /// <param name="fieldType">列类型,默认typeof(int)</param>
    /// <returns></returns>
    object GetExpression(object model, string? field = "ID", Type? fieldType = null)
    {
        // ValueExpression
        var body = Expression.Property(Expression.Constant(model), typeof(TItem), field ?? "ID");
        var tDelegate = typeof(Func<>).MakeGenericType(fieldType ?? typeof(int));
        var valueExpression = Expression.Lambda(tDelegate, body);
        return valueExpression;
    }


    /// <summary>
    /// 动态生成控件 TableColumn 图片列
    /// </summary>
    /// <param name="model"></param>
    /// <param name="tableImgField"></param>
    /// <returns></returns>
    private RenderFragment RenderTableImgColumn(TItem model, TableImgField? tableImgField = null) => builder =>
      {
          tableImgField = tableImgField ?? TableImgField ?? new TableImgField();
          var fieldExpresson = GetExpression(model, tableImgField.Field, tableImgField.FieldType);
          builder.OpenComponent(0, typeof(TableColumn<,>).MakeGenericType(typeof(TItem), tableImgField.FieldType));
          builder.AddAttribute(1, "FieldExpression", fieldExpresson);
          //builder.AddAttribute(2, "Width", 200);
          builder.AddAttribute(3, "Template", new RenderFragment<TableColumnContext<TItem, string>>(context => buttonBuilder =>
          {
              buttonBuilder.OpenComponent<ImgColumn>(0);
              buttonBuilder.AddAttribute(1, nameof(ImgColumn.Title), tableImgField.Title);
              buttonBuilder.AddAttribute(2, nameof(ImgColumn.Name), tableImgField.Name);
              var value = (context.Row).GetIdentityKey(tableImgField.Field);
              buttonBuilder.AddAttribute(3, nameof(ImgColumn.Url), value);
              if (!string.IsNullOrEmpty(tableImgField.BaseUrl)) buttonBuilder.AddAttribute(4, nameof(ImgColumn.BaseUrl), tableImgField.BaseUrl);
              if (!string.IsNullOrEmpty(tableImgField.Style)) buttonBuilder.AddAttribute(5, nameof(ImgColumn.Style), tableImgField.Style);
              buttonBuilder.CloseComponent();
          }));
          if (!string.IsNullOrEmpty(tableImgField.ColumnText)) builder.AddAttribute(4, "Text", tableImgField.ColumnText);
          builder.CloseComponent();
      };


    //<TableColumn Field = "@Field" Text="操作" Width="100" Editable="false">
    //  <Template Context = "value" >
    //    < Button Color="Color.Primary" Size="Size.Small"
    //            @onclick="(() => {SelectOneItem=((TItem)value.Row);ExtraLargeModal.Toggle();})">
    //        子项
    //    </Button>
    //  </Template>
    //</TableColumn>


    /// <summary>
    /// 动态生成控件 TableColumn 明细行按钮
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private RenderFragment RenderTableColumn(TItem model) => builder =>
    {
        var fieldExpresson = GetExpression(model, Field, FieldType); // 刚才你的那个获取表达式 GetExpression() 的返回值的
        builder.OpenComponent(0, typeof(TableColumn<,>).MakeGenericType(typeof(TItem), FieldType));
        builder.AddAttribute(1, "FieldExpression", fieldExpresson);
        // 这里继续添加你原来 Razor 文件中的哪些属性

        // 添加模板
        builder.AddAttribute(2, "Template", new RenderFragment<TableColumnContext<TItem, int>>(context => buttonBuilder =>
        {
            // 这里写按钮的
            buttonBuilder.OpenComponent<Button>(0);
            buttonBuilder.AddAttribute(2, nameof(Button.Text), "明细行");
            buttonBuilder.AddAttribute(1, nameof(Button.OnClickWithoutRender), new Func<Task>(async () =>
            {
                //var op = new DialogOption()
                //{
                //    BodyContext = context
                //};
                //await DialogService.Show(op);
                detalisTable!.FieldValueD = (context.Row).GetIdentityKey(Field);
                await ExtraLargeModal!.Toggle();
                await detalisTable.QueryAsync();

            }));
            buttonBuilder.CloseComponent();
        }));
        builder.CloseComponent();
    };



    /// <summary>
    /// 动态生成控件 TableColumn 功能列
    /// </summary>
    /// <param name="model"></param>
    /// <param name="tableImgField"></param>
    /// <returns></returns>
    private RenderFragment RenderTableFunctionsColumn(TItem model, TableImgField tableImgField) => builder =>
    {
        var fieldExpresson = GetExpression(model, tableImgField.Field, tableImgField.FieldType);
        builder.OpenComponent(0, typeof(TableColumn<,>).MakeGenericType(typeof(TItem), tableImgField.FieldType));
        builder.AddAttribute(1, "FieldExpression", fieldExpresson);
        //builder.AddAttribute(2, "Width", 200);
        builder.AddAttribute(3, "Template", new RenderFragment<TableColumnContext<TItem, string>>(context => buttonBuilder =>
        {
            buttonBuilder.OpenComponent<Button>(0);
            buttonBuilder.AddAttribute(1, nameof(Button.Text), tableImgField.Title);
            var value = (context.Row).GetIdentityKey(tableImgField.Field);
            if (tableImgField.Callback.HasDelegate)
            {
                buttonBuilder.AddAttribute(1, nameof(Button.OnClickWithoutRender), new Func<Task>(async () =>
                {
                    await tableImgField.Callback.InvokeAsync(value);
                }));
            }
            if (!string.IsNullOrEmpty(tableImgField.Style)) buttonBuilder.AddAttribute(5, nameof(ImgColumn.Style), tableImgField.Style);
            buttonBuilder.CloseComponent();
        }));
        if (!string.IsNullOrEmpty(tableImgField.ColumnText)) builder.AddAttribute(4, "Text", tableImgField.ColumnText);
        builder.CloseComponent();
    };

    //<TableColumn @bind-Field="@context1.PhotoUrl" Text="图" Width="60" Visible="true">
    //    <Template Context = "value" >
    //        <img class="category-image"
    //             onerror="onerror=null;src='images/blank.jpg'"
    //             src="@CheckPhoto(((AppCollects)value.Row))"
    //             title="@value.Value"
    //             @onclick='(() => dataService.ItemPhoto(((AppCollects)value.Row)))'>
    //    </Template>
    //</TableColumn>


    /// <summary>
    /// 动态生成控件 TableColumn 图片列
    /// </summary>
    /// <returns></returns>
    private RenderFragment RenderTableColumnPhotoUrl(TItem model) => builder =>
    {
        var fieldExpresson = GetExpression(model); // 刚才你的那个获取表达式 GetExpression() 的返回值的
        builder.OpenComponent(0, typeof(TableColumn<,>).MakeGenericType(typeof(TItem), FieldType));
        builder.AddAttribute(1, "FieldExpression", fieldExpresson);
        // 这里继续添加你原来 Razor 文件中的哪些属性

        // 添加模板
        builder.AddAttribute(2, "Template", new RenderFragment<TableColumnContext<TItem, int>>(context => buttonBuilder =>
        {
            // 这里写按钮的
            buttonBuilder.OpenComponent<Button>(0);
            buttonBuilder.AddAttribute(2, nameof(Button.Text), "明细行");
            buttonBuilder.AddAttribute(1, nameof(Button.OnClickWithoutRender), new Func<Task>(async () =>
            {
                //var op = new DialogOption()
                //{
                //    BodyContext = context
                //};
                //await DialogService.Show(op);
                detalisTable!.FieldValueD = (context.Row).GetIdentityKey(Field);
                await ExtraLargeModal!.Toggle();
                await detalisTable.QueryAsync();

            }));
            buttonBuilder.CloseComponent();
        }));
        builder.CloseComponent();
    };


    enum TableDetailRowType
    {
        选项卡1,
        选项卡2,
        自定义选项卡风格,
        选项卡3,
    }

    //<TablePollo TItem = "@ItemDetails"
    //            Field="@(FieldD??Field)"
    //            FieldValue="@context.GetIdentityKey(Field)"
    //            ItemDetails="NullClass"
    //            IsBordered="false" />

    /// <summary>
    /// 动态生成控件 TablePollo
    /// </summary>
    /// <param name="model"></param>
    /// <param name="rowType"></param>
    /// <returns></returns>
    private RenderFragment RenderTableDetailRow(object model, TableDetailRowType rowType = TableDetailRowType.选项卡1) => builder =>
    {
        var _Field = Field;
        var _FieldValue = model.GetIdentityKey(Field);
        if (rowType == TableDetailRowType.选项卡2)
        {
            //第二选项卡
            builder.OpenComponent<TablePollo<ItemDetailsII, NullClass, NullClass, NullClass>>(0);
            if (FieldII != null && FieldII != Field)
            {
                _Field = FieldII ?? Field;
                _FieldValue = model.GetIdentityKey(FieldII);
            }
            _Field = FieldIID ?? Field;

        }
        else if (rowType == TableDetailRowType.选项卡3)
        {
            builder.OpenComponent<TablePollo<ItemDetailsIII, NullClass, NullClass, NullClass>>(0);
            if (FieldII != null && FieldII != Field)
            {
                _Field = FieldIII ?? Field;
                _FieldValue = model.GetIdentityKey(FieldIII);
            }
            _Field = FieldIIID ?? Field;

        }
        else if (ShowDetailRowType == ShowDetailRowType.表内明细II)
        {
            builder.OpenComponent<TablePollo<ItemDetails, ItemDetailsII, NullClass, NullClass>>(0);
            _Field = FieldD ?? Field;
        }
        else
        {
            builder.OpenComponent<TablePollo<ItemDetails, NullClass, NullClass, NullClass>>(0);
            _Field = FieldD ?? Field;
        }
        builder.AddAttribute(0, nameof(Field), _Field);
        if (IsDebug) ToastService.Success($"展开详情行 {_Field} : _FieldValue");
        builder.AddAttribute(1, nameof(FieldValue), _FieldValue);
        builder.AddAttribute(2, nameof(IsBordered), true);
        builder.AddAttribute(3, nameof(ShowToolbar), ShowToolbar);
        builder.AddAttribute(4, nameof(ShowDefaultButtons), ShowDefaultButtons);
        builder.AddAttribute(5, nameof(ShowExtendButtons), ShowExtendButtons);
        builder.AddAttribute(6, nameof(ShowSearch), ShowSearch);
        builder.AddAttribute(7, nameof(ShowColumnList), ShowColumnList);
        builder.AddAttribute(8, nameof(IsMultipleSelect), IsMultipleSelect);
        builder.AddAttribute(9, nameof(DoubleClickToEdit), DoubleClickToEdit);
        if (rowType == TableDetailRowType.自定义选项卡风格)
        {
            builder.AddAttribute(10, nameof(TableSize), TableSize.Compact);
            builder.AddAttribute(11, nameof(HeaderStyle), TableHeaderStyle.Light);
        }
        else
        {
            builder.AddAttribute(10, nameof(TableSize), TableSizeDetails);
            builder.AddAttribute(11, nameof(HeaderStyle), HeaderStyleDetails);
        }
        builder.AddAttribute(12, nameof(ShowLineNo), ShowLineNo);
        builder.AddAttribute(13, nameof(ShowSkeleton), ShowSkeleton);
        builder.AddAttribute(14, nameof(ShowRefresh), ShowRefresh);
        builder.AddAttribute(15, nameof(IncludeByPropertyNames), rowType == TableDetailRowType.选项卡2 ? (SubIncludeByPropertyNamesII ?? SubIncludeByPropertyNames) : rowType == TableDetailRowType.选项卡3 ? (SubIncludeByPropertyNamesIII ?? SubIncludeByPropertyNames) : SubIncludeByPropertyNames);
        if (SubTableImgFields != null) builder.AddAttribute(16, nameof(TableImgFields), SubTableImgFields);
        if (SubTableImgField != null) builder.AddAttribute(17, nameof(TableImgField), SubTableImgField);
        if (SubRenderMode != null) builder.AddAttribute(18, nameof(RenderMode), SubRenderMode);
        if (ibstring != null) builder.AddAttribute(19, nameof(ibstring), ibstring);
        if (SubTableFunctionsFields != null) builder.AddAttribute(16, nameof(TableFunctionsFields), SubTableFunctionsFields);
        if (SubRowButtons != null) builder.AddAttribute(16, nameof(RowButtons), SubRowButtons);
        builder.AddAttribute(17, nameof(IsExtendButtonsInRowHeader), IsExtendButtonsInRowHeader);
        builder.CloseComponent();
    };

    #endregion

    #region 工具栏按钮
    private async Task ImportExcel()
    {
        if (Items == null || !Items.Any())
        {
            ToastService?.Error("提示", "数据为空!");
            return;
        }
        var option = new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "提示",
            Content = $"{Excel导入文本}中,请稍等片刻...",
            IsAutoHide = false
        };
        // 弹出 Toast
        await ToastService.Show(option);
        await Task.Delay(100);


        // 开启后台进程进行数据处理
        if (!Excel导入.HasDelegate)
            await MockDownLoadAsync();
        else
            await Excel导入.InvokeAsync("");

        // 关闭 option 相关联的弹窗
        option.Close();

        // 弹窗告知下载完毕
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "提示",
            Content = $"{Excel导入文本}成功,请检查数据",
            IsAutoHide = false
        });

        await mainTable!.QueryAsync();
    }
    private async Task ImportItems()
    {
        if (!导入.HasDelegate)
        {
            ToastService?.Error("提示", "操作过程为空!");
            return;
        }
        if (Items == null || !Items.Any())
        {
            ToastService?.Error("提示", "数据为空!");
            return;
        }
        var option = new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "提示",
            Content = $"{导入文本}中,请稍等片刻...",
            IsAutoHide = false
        };
        // 弹出 Toast
        await ToastService.Show(option);
        await Task.Delay(100);


        // 开启后台进程进行数据处理
        await 导入.InvokeAsync("");

        // 关闭 option 相关联的弹窗
        option.Close();

        // 弹窗告知下载完毕
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "提示",
            Content = $"{导入文本}成功,请检查数据",
            IsAutoHide = false
        });

        await mainTable!.QueryAsync();
    }
    private async Task ImportItemsII()
    {
        if (!导入II.HasDelegate)
        {
            ToastService?.Error("提示", "操作过程为空!");
            return;
        }
        if (Items == null || !Items.Any())
        {
            ToastService?.Error("提示", "数据为空!");
            return;
        }
        var option = new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "提示",
            Content = $"{导入II文本}中,请稍等片刻...",
            IsAutoHide = false
        };
        // 弹出 Toast
        await ToastService.Show(option);
        await Task.Delay(100);


        // 开启后台进程进行数据处理
        await 导入II.InvokeAsync("");

        // 关闭 option 相关联的弹窗
        option.Close();

        // 弹窗告知下载完毕
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "提示",
            Content = $"{导入II文本}成功,请检查数据",
            IsAutoHide = false
        });

        await mainTable!.QueryAsync();
    }
    private async Task 执行添加Cmd()
    {
        var option = new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "提示",
            Content = $"{执行添加文本}中,请稍等片刻...",
            IsAutoHide = false
        };
        // 弹出 Toast
        await ToastService.Show(option);
        await Task.Delay(100);


        // 开启后台进程进行数据处理
        if (!执行添加.HasDelegate)
            await MockDownLoadAsync();
        else
            await 执行添加.InvokeAsync("");

        // 关闭 option 相关联的弹窗
        option.Close();

        // 弹窗告知下载完毕
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "提示",
            Content = $"{执行添加文本}成功,请检查数据",
            IsAutoHide = false
        });

    }

    #region 导出数据方法
    /// <summary>
    /// 导出数据方法
    /// </summary>
    /// <param name="Items"></param>
    /// <param name="opt"></param>
    /// <returns></returns>
    protected async Task<bool> ExportAsync(IEnumerable<TItem> Items, QueryPageOptions opt)
    {
        var ret = false;
        if (OnExportAsync != null)
        {
            ret = await OnExportAsync(Items, opt);
        }
        else
        {
            ret = await ExportExcelAsync(Items);
        }
        return ret;
    }

    private async Task<bool> ExportExcelAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, UseMiniExcel ? ExportType.MiniExcel : ExportType.Excel);
    private async Task<bool> ExportPDFAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, ExportType.Pdf);
    private async Task<bool> ExportWordAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, UseMiniWord? ExportType.MiniWord :ExportType.Word);
    private async Task<bool> ExportHtmlAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, ExportType.Html);
    private async Task<bool> ExportAutoAsync(IEnumerable<TItem>? items, ExportType exportType = ExportType.MiniExcel)
    {
        if (!导出.HasDelegate && (items == null || !items.Any()) && (ItemsCache == null || !ItemsCache.Any()))
        {
            await ToastService.Error("提示", "无数据可导出");
            return false;
        }
        var option = new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "提示",
            Content = $"{导出文本}正在执行,请稍等片刻...",
            IsAutoHide = false
        };
        // 弹出 Toast
        await ToastService.Show(option);
        await Task.Delay(100);


        // 开启后台进程进行数据处理
        if (!导出.HasDelegate)
            await Export(items!.ToList(), exportType);
        else
            await 导出.InvokeAsync((items!, exportType));

        // 关闭 option 相关联的弹窗
        option.Close();

        // 弹窗告知下载完毕
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "提示",
            Content = $"{导出文本}成功,请检查数据",
            IsAutoHide = false
        });
        return true;

    }

    private async Task MockDownLoadAsync()
    {
        // 此处模拟打包下载数据耗时 5 秒
        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    public virtual async Task Export(List<TItem>? items = null, ExportType exportType = ExportType.Excel)
    {
        try
        {
            var fileName = items == null ? "模板" : typeof(TItem).Name;

            if (ExportBasePath != null)
            {
                if (Directory.Exists(ExportBasePath) == false)
                    Directory.CreateDirectory(ExportBasePath);

                fileName = Path.Combine(ExportBasePath, fileName);
            }

            if (items == null || !items.Any()) items = ItemsCache?.ToList();

            var memoryStream = new MemoryStream();
            if (ExportToStream)
            {
                var res = await Exporter.Export2Stream(items, exportType, fileName: fileName);
                if (res.Stream == null)
                {
                    ToastService?.Error("提示", "导出失败,请检查数据");
                    return;
                }
                memoryStream = (MemoryStream)res.Stream;
                fileName = res.FileName;
            }
            else
            {
                fileName = await Exporter.Export(fileName, items, exportType);
                ToastService?.Success("提示", Path.GetFileName(fileName) + "已生成");
                using FileStream source = File.Open(fileName, FileMode.Open);
                source.CopyTo(memoryStream);
            }

            using var streamRef = new DotNetStreamReference(stream: memoryStream);

            await module!.InvokeVoidAsync("downloadFileFromStream", Path.GetFileName(fileName), streamRef);

        }
        catch (Exception e)
        {
            ToastService?.Error($"导出", $"{exportType}出错,请检查. {e.Message}");
        }
    }

    #endregion

    private async Task 升级Cmd(IEnumerable<TItem> items)
    {
        if (升级.HasDelegate)
        {
            await 升级.InvokeAsync((items, true));
            await mainTable!.QueryAsync();
            ToastService?.Success($"{升级按钮文字}成功", $"{升级按钮文字}成功,请检查数据");
        }
    }
    private async Task 升级IICmd(IEnumerable<TItem> items)
    {
        if (升级II.HasDelegate)
        {
            await 升级II.InvokeAsync((items, true));
            ToastService?.Success($"{升级按钮II文字}成功", $"{升级按钮II文字}成功,请检查数据");
        }
    }
    private async Task ClickRow(TItem item)
    {
        if (OnClickRowCallback != null)
        {
            await OnClickRowCallback.Invoke(item);
        }
    }

    private async Task OnRowButtonClick(TItem item, RowButtonField RowButtonField)
    {
        if (RowButtonField.CallbackFunc != null)
        {
            await RowButtonField.CallbackFunc.Invoke(item);
            await mainTable!.QueryAsync();
        }
    }

    public Task PrintPreview(IEnumerable<TItem> item)
    {
        JS.InvokeAsync<object>(
        "toolsFunctions.printpreview", 100
        );
        return Task.CompletedTask;
    }

    private Task 新窗口打开()
    {
        if (string.IsNullOrEmpty(新窗口打开Url))
        {
            ToastService?.Error("提示", "Url为空!");
            return Task.CompletedTask;
        }
        JS.NavigateToNewTab(新窗口打开Url);
        return Task.CompletedTask;
    }
    private Task 查找文本Icmd()
    {
        if (string.IsNullOrEmpty(查找文本I))
        {
            ToastService?.Error("提示", "Url为空!");
            return Task.CompletedTask;
        }
#pragma warning disable BL0005 
        mainTable!.SearchText = 查找文本I;
#pragma warning restore BL0005 
        return Task.CompletedTask;
    }

    private Task IsExcelToggle()
    {
        IsExcel = !IsExcel;
        DoubleClickToEdit = !IsExcel;
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 全部记录
    /// </summary>
    public List<TItem>? GetAllItems() => GetDataService().GetAllItems(
                dynamicFilterInfo,
                IncludeByPropertyNames,
                LeftJoinString,
                OrderByPropertyName,
                WhereCascadeOr,
                WhereLamda
            );

    #endregion


}
#endregion
