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
using FreeSql;
using FreeSql.Internal.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using static AME.EnumsExtensions;

namespace AmeBlazor.Components;
public partial class TableAmeProBase<TItem> : TableAmeBase where TItem : class, new()
{

    public TItem? SelectOneItem { get; set; }

    [Parameter] public Func<IEnumerable<TItem>, ExportType, Task>? 导出 { get; set; }

    [Parameter] public Func<IEnumerable<TItem>, Task>? 批量执行 { get; set; }

    [Parameter] public Func<IEnumerable<TItem>, Task>? 升级 { get; set; }

    [Parameter] public Func<IEnumerable<TItem>, Task>? 升级II { get; set; }

    /// <summary>
    /// 查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")
    /// </summary>
    [Parameter] public Expression<Func<TItem, bool>>? WhereLamda { get; set; }

    /// <summary>
    /// 获得/设置 明细表弹窗关闭按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<TItem, Task<bool>>? DetailsDialogCloseAsync { get; set; }

    /// <summary>
    /// 获得/设置 明细表弹窗保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<TItem, Task<bool>>? DetailsDialogSaveAsync { get; set; }

    [NotNull]
    public Table<TItem>? TableMain { get; set; }

    public ISelect<TItem>? SelectCache { get; set; }

    public IEnumerable<TItem>? ItemsCache { get; set; }
    public FloatPanel? FloatPanelUp { get; set; }

    public FloatPanel? FloatPanel { get; set; }

    public string? cacheFooterValue;

    /// <summary>
    /// 获得/设置 Table Footer Content 
    /// </summary>
    [Parameter]
    public RenderFragment? FooterContent { get; set; }

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
    public Func<ITableExportDataContext<TItem>, Task<bool>>? OnExportAsync { get; set; }

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
    /// 获得/设置 编辑按钮回调方法
    /// </summary>
    [Parameter]
    public Func<TItem, Task>? EditAsync { get; set; }

    /// <summary>
    /// 获得/设置 保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<TItem, ItemChangedType, Task<TItem>>? SaveAsync { get; set; }

    /// <summary>
    /// 保存数据后异步回调方法
    /// </summary>
    [Parameter]
    public Func<TItem, ItemChangedType, Task>? AfterSaveAsync { get; set; }

    /// <summary>
    /// 获得/设置 删除按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<IEnumerable<TItem>, Task<bool>>? DeleteAsync { get; set; }

    /// <summary>
    /// 获得/设置 删除后回调委托方法 (SaveModelAsync)
    /// </summary>
    [Parameter]
    public Func<List<TItem>, Task>? OnAfterDeleteAsync { get; set; }

    /// <summary>
    /// 获得/设置 保存删除后回调委托方法 (SaveModelAsync)
    /// </summary>
    [Parameter]
    public Func<Task>? OnAfterModifyAsync { get; set; }

    /// <summary>
    /// 获得/设置 查询回调方法,用于计算合计之类
    /// </summary>
    [Parameter]
    public Action<ISelect<TItem>>? AfterQueryAsync { get; set; }

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

    /// <summary>
    /// 获得/设置 列创建时回调委托方法
    /// </summary>
    [Parameter]
    public Func<List<ITableColumn>, Task>? OnColumnCreating { get; set; }

    /// <summary>
    /// 获得/设置 获得高级搜索条件回调方法 默认 null
    /// </summary>
    [Parameter]
    public Func<PropertyInfo, TItem, List<SearchFilterAction>?>? GetAdvancedSearchFilterCallback { get; set; }

    /// <summary>
    /// 获得/设置 分页信息内容模板
    /// </summary>
    [Parameter]
    public RenderFragment? PageInfoBodyTemplate { get; set; }

    /// <summary>
    /// 获得/设置 明细行模板 <see cref="IsDetails" />
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? DetailRowTemplate { get; set; }

    /// <summary>
    /// 获得/设置 TableFooter 实例
    /// </summary>
    [Parameter]
    public RenderFragment<IEnumerable<TItem>>? TableFooter { get; set; }

    /// <summary>
    /// 获得/设置 Table Footer 模板
    /// </summary>
    [Parameter]
    public RenderFragment<IEnumerable<TItem>>? FooterTemplate { get; set; }

    /// <summary>
    /// 获得/设置 双击行回调委托方法
    /// </summary>
    [Parameter]
    public Func<TItem, Task>? OnDoubleClickRowCallback { get; set; }

    /// <summary>
    /// 获得/设置 被选中数据集合
    /// </summary>
    [Parameter]
    public List<TItem> SelectedRows { get; set; } = [];

    /// <summary>
    /// 获得/设置 选中行变化回调方法
    /// </summary>
    [Parameter]
    public EventCallback<List<TItem>> SelectedRowsChanged { get; set; }

    /// <summary>
    /// 获得/设置 弹窗 Footer
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? EditFooterTemplate { get; set; }

    #endregion

    /// <summary>
    /// 附加复杂查询条件
    /// </summary>
    [Parameter] public DynamicFilterInfo? DynamicFilterInfo { get; set; }

    #region 数据服务
    /// <summary>
    /// 获得/设置 注入数据服务
    /// </summary>
    [Inject]
    [NotNull]
    public virtual IEnumerable<FreeSqlDataService<TItem>>? DataServices { get; set; }

    public virtual FreeSqlDataService<TItem> GetDataService()
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

    public async Task<TItem> OnAddAsync()
    {
        var newone = new TItem();
        if (FieldValue != null || FieldValueD != null)
        {
            newone.FieldSetValue(Field, FieldValue ?? FieldValueD);
        }
        if (AddAsync != null)
        {
            newone = await AddAsync(newone);
        }
        return newone;
    }

    public async Task<TItem> OnEditAsync(TItem item)
    {
        if (EditAsync != null)
        {
            await EditAsync(item);
        }
        GetDataService().ItemCache = item.Clone();
        return item;
    }

    public async Task<bool> OnSaveAsync(TItem item, ItemChangedType changedType)
    {
        if (SaveAsync != null)
        {
            item = await SaveAsync(item, changedType);
        }
        var res = await GetDataService().SaveAsync(item, changedType);
        if (AfterSaveAsync != null)
        {
            await AfterSaveAsync(item, changedType);
        }
        return res;
    }

    public async Task<bool> OnDeleteAsync(IEnumerable<TItem> items)
    {
        if (DeleteAsync != null)
        {
            if (!await DeleteAsync(items))
            {
                return false;
            }
        }
        return await GetDataService().DeleteAsync(items);
    }

    public async Task<QueryData<TItem>> OnQueryAsync(QueryPageOptions options)
    {
        if (PageIndexCache != null)
        {
            options.PageIndex = PageIndexCache.Value;
            PageIndexCache = null;
        }
        else
        {
            if (AutoSavePageIndex)
            {
                await StorageSetValue(AutoSavePageIndexKey, options.PageIndex);
            }
            else
            {
                PageIndex = options.PageIndex;
            }
        }

        var itemsOrm = await GetDataService().QueryAsyncWithWhereCascade(
                options,
                dynamicFilterInfo,
                IncludeByPropertyNames,
                LeftJoinString,
                OrderByPropertyName,
                WhereCascadeOr,
                WhereLamda
            );

        ItemsCache = itemsOrm.Items;

        if (AfterQueryAsync != null)
        {
            var select = ISelectCache();
            if (SelectCache != select)
            {
                SelectCache = select;
                AfterQueryAsync(ISelectCache());
            }
        }

        return itemsOrm;
    }

    /// <summary>
    /// 获得查询子句
    /// </summary>
    public ISelect<TItem> ISelectCache() => GetDataService().ISelectCache(
                dynamicFilterInfo,
                IncludeByPropertyNames,
                LeftJoinString,
                WhereCascadeOr,
                WhereLamda
            );

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

    /// <summary>
    /// Reset all Columns Filter
    /// </summary>
    public async Task ResetFilters()
    {
        await TableMain.ResetFilters();
    }

    /// <summary>
    /// 获得 表头集合
    /// </summary>
    public List<ITableColumn> Columns { get => TableMain.Columns; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ShowDetailRowS)
        {
            ShowDetailRow = _ => true;
        }

        if (PageItemsSourceStart != null && PageItemsSourceStart < PageItemsSource.FirstOrDefault())
        {
            PageItemsSource = PageItemsSource.Append(PageItemsSourceStart.Value).OrderBy(a => a).ToList();
        }
        if (PageItems != 0 && !PageItemsSource.Contains(PageItems))
        {
            PageItemsSource = PageItemsSource.Append(PageItems).OrderBy(a => a).ToList();
        }
        if (IsReadonly)
        {
            EditModalTitle = "查看";
            if (EditFooterTemplate == null) { EditFooterTemplate = (item) => builder => { }; }
        }

    }

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
    }

    /// <summary>
    /// 查询按钮调用此方法
    /// </summary>
    /// <param name="whereLamda">附加and条件,如果传入值为空不使用</param>
    /// <param name="force">强制使用附加and条件,即使传入值为空也使用</param>
    /// <returns></returns>
    public async Task QueryAsync(Expression<Func<TItem, bool>>? whereLamda = null, bool force = false)
    {
        if (whereLamda != null || force)
        {
            WhereLamda = whereLamda;
        }

        await TableMain.QueryAsync();
    }

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
    protected object GetExpression(object model, string? field = "ID", Type? fieldType = null)
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
    protected RenderFragment RenderTableImgColumn(TItem model, TableImgField? tableImgField = null) => builder =>
    {
        tableImgField = tableImgField ?? TableImgField ?? new TableImgField();
        var fieldExpresson = Utility.GenerateValueExpression(model, tableImgField.Field, tableImgField.FieldType);
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
            if (!string.IsNullOrEmpty(tableImgField.BaseUrl))
            {
                buttonBuilder.AddAttribute(4, nameof(ImgColumn.BaseUrl), tableImgField.BaseUrl);
            }

            if (!string.IsNullOrEmpty(tableImgField.Style))
            {
                buttonBuilder.AddAttribute(5, nameof(ImgColumn.Style), tableImgField.Style);
            }

            buttonBuilder.CloseComponent();
        }));
        if (!string.IsNullOrEmpty(tableImgField.ColumnText))
        {
            builder.AddAttribute(4, "Text", tableImgField.ColumnText);
        }

        builder.CloseComponent();
    };

    /// <summary>
    /// 动态生成控件 TableColumn 功能列
    /// </summary>
    /// <param name="model"></param>
    /// <param name="tableImgField"></param>
    /// <returns></returns>
    protected RenderFragment RenderTableFunctionsColumn(TItem model, TableImgField tableImgField) => builder =>
    {
        var fieldExpresson = Utility.GenerateValueExpression(model, tableImgField.Field, tableImgField.FieldType);
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
            if (!string.IsNullOrEmpty(tableImgField.Style))
            {
                buttonBuilder.AddAttribute(5, nameof(ImgColumn.Style), tableImgField.Style);
            }

            buttonBuilder.CloseComponent();
        }));
        if (!string.IsNullOrEmpty(tableImgField.ColumnText))
        {
            builder.AddAttribute(4, "Text", tableImgField.ColumnText);
        }

        builder.CloseComponent();
    };


    /// <summary>
    /// 动态生成控件 TableColumn 明细行按钮
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual RenderFragment RenderTableColumn(TItem model) => builder =>
    {
        var fieldExpresson = Utility.GenerateValueExpression(model, Field ?? FieldD ?? "ID", FieldType);
        var typeTableColumn = typeof(TableColumn<,>).MakeGenericType(typeof(TItem), FieldType);
        builder.OpenComponent(0, typeTableColumn);
        builder.AddAttribute(1, "FieldExpression", fieldExpresson);
        builder.AddAttribute(2, "Template", DialogTableDetails(FieldType));
        builder.AddAttribute(3, "Visible", true);
        builder.CloseComponent();
    };

    /// <summary>
    /// 动态生成控件 TableColumn 明细行按钮
    /// </summary>
    /// <param name="fieldType"></param>
    /// <returns></returns>
    private object DialogTableDetails(Type fieldType)
    {
        var valueChangedInvoker = CreateLambda(this, fieldType).Compile();
        return valueChangedInvoker();

        static Expression<Func<object>> CreateLambda(TableAmeProBase<TItem> table, Type fieldType)
        {
            var method = typeof(TableAmeProBase<TItem>).GetMethod(nameof(CreateRenderFragment), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(fieldType);
            var body = Expression.Call(Expression.Constant(table), method);

            return Expression.Lambda<Func<object>>(body);
        }
    }

    /// <summary>
    /// 动态生成明细弹窗控件
    /// </summary>
    /// <returns></returns>
    private RenderFragment<TableColumnContext<TItem, TValue>> CreateRenderFragment<TValue>()
    {
        return new RenderFragment<TableColumnContext<TItem, TValue>>(context => buttonBuilder =>
        {
            buttonBuilder.OpenComponent<Button>(0);
            buttonBuilder.AddAttribute(1, nameof(Button.Text), "明细");
            buttonBuilder.AddAttribute(2, nameof(Button.OnClickWithoutRender), new Func<Task>(async () =>
            {
                var op = new DialogOption()
                {
                    BodyContext = context,
                    BodyTemplate = RenderTableDetails(context.Row),
                    Size = Size.Large,
                    ShowMaximizeButton = true,
                    ShowResize = true,
                    ShowPrintButton = true,
                    ShowSaveButton = true,
                    IsDraggable = true,
                    SaveButtonText = "保存并应用",
                    OnCloseAsync = async () =>
                    {
                        if (DetailsDialogCloseAsync != null)
                        {
                            if (await DetailsDialogCloseAsync(context.Row))
                            {
                                await TableMain.QueryAsync();
                            }
                        }
                    },
                    OnSaveAsync = async () =>
                    {
                        if (DetailsDialogSaveAsync != null)
                        {
                            if (await DetailsDialogSaveAsync(context.Row))
                            {
                                await TableMain.QueryAsync();
                            }
                        }
                        return true;
                    },
                };
                await DialogService.Show(op);

            }));
            buttonBuilder.CloseComponent();
        });
    }

    /// <summary>
    /// 动态生成控件 TableColumn 图片列
    /// </summary>
    /// <returns></returns>
    public virtual RenderFragment RenderTableColumnPhotoUrl(TItem model) => builder =>
    {
        var fieldExpresson = Utility.GenerateValueExpression(model, Field ?? FieldD ?? "ID", FieldType); // 刚才你的那个获取表达式 GetExpression() 的返回值的
        var value = Utility.GetPropertyValue<object, object>(model, Field ?? FieldD ?? "ID");
        var valueType = value.GetType();
        builder.OpenComponent(0, typeof(TableColumn<,>).MakeGenericType(typeof(TItem), FieldType));
        builder.AddAttribute(1, "FieldExpression", fieldExpresson);
        // 添加模板
        builder.AddAttribute(2, "Template", DialogTableDetails(FieldType));
        builder.CloseComponent();
    };

    /// <summary>
    /// 动态生成明细表控件 TableAmePro
    /// </summary>
    /// <param name="model"></param>
    /// <param name="rowType"></param>
    /// <returns></returns>
    public virtual RenderFragment RenderTableDetails(object model, TableDetailRowType rowType = TableDetailRowType.选项卡1) => builder =>
    {
        var _Field = Field;
        var _FieldValue = model.GetIdentityKey(Field);

        TRenderTable(rowType, builder);

        if (rowType == TableDetailRowType.选项卡2)
        {
            //第二选项卡
            if (FieldII != null && FieldII != Field)
            {
                _Field = FieldII ?? Field;
                _FieldValue = model.GetIdentityKey(FieldII);
            }
            _Field = FieldIID ?? Field;

        }
        else if (ShowDetailRowType == ShowDetailRowType.表内明细II)
        {
            _Field = FieldD ?? Field;
        }
        else if (rowType == TableDetailRowType.选项卡3)
        {
            if (FieldII != null && FieldII != Field)
            {
                _Field = FieldIII ?? Field;
                _FieldValue = model.GetIdentityKey(FieldIII);
            }
            _Field = FieldIIID ?? Field;
        }
        else
        {
            _Field = FieldD ?? Field;
        }

        builder.AddAttribute(0, nameof(Field), _Field);

        if (IsDebug)
        {
            ToastService.Success($"展开详情行 {_Field} : _FieldValue");
        }

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
        if (SubTableImgFields != null)
        {
            builder.AddAttribute(16, nameof(TableImgFields), SubTableImgFields);
        }
        if (SubTableImgField != null)
        {
            builder.AddAttribute(17, nameof(TableImgField), SubTableImgField);
        }
        if (SubRenderMode != null)
        {
            builder.AddAttribute(18, nameof(RenderMode), SubRenderMode);
        }
        if (ibstring != null)
        {
            builder.AddAttribute(19, nameof(ibstring), ibstring);
        }
        if (SubTableFunctionsFields != null)
        {
            builder.AddAttribute(20, nameof(TableFunctionsFields), SubTableFunctionsFields);
        }
        if (SubRowButtons != null)
        {
            builder.AddAttribute(21, nameof(RowButtons), SubRowButtons);
        }
        builder.AddAttribute(22, nameof(IsExtendButtonsInRowHeader), IsExtendButtonsInRowHeader);
        if (SubScrollMode != null)
        {
            builder.AddAttribute(23, nameof(ScrollMode), SubScrollMode);
        }
        if (SubIsPagination != null)
        {
            builder.AddAttribute(24, nameof(IsPagination), SubIsPagination);
        }
        else
        {
            builder.AddAttribute(24, nameof(IsPagination), IsPagination);
        }
        if (SubPageItems != null)
        {
            builder.AddAttribute(25, nameof(PageItems), SubPageItems);
        }
        if (SubHeight != null)
        {
            builder.AddAttribute(26, nameof(Height), SubHeight);
        }
        if (SubEditMode != null)
        {
            builder.AddAttribute(27, nameof(EditMode), SubEditMode);
        }
        builder.AddAttribute(29, nameof(IsExcel), SubIsExcel);
        builder.AddAttribute(30, nameof(IsReadonly), SubIsReadonly);
        builder.AddAttribute(31, nameof(IsSimpleUI), SubIsSimpleUI);
        TRenderTableAdditionalAttributes(builder);
        builder.CloseComponent();
    };

    /// <summary>
    /// 附加属性
    /// </summary>
    /// <param name="builder"></param>
    public virtual void TRenderTableAdditionalAttributes(RenderTreeBuilder builder)
    {
    }

    /// <summary>
    /// 明细表控件
    /// </summary>
    /// <param name="rowType"></param>
    /// <param name="builder"></param>
    public virtual void TRenderTable(TableDetailRowType rowType, RenderTreeBuilder builder)
    {
        builder.OpenComponent<TableAmeProBase<Item>>(0);
    }

    #endregion

    #region 工具栏按钮
    public async Task ImportExcel()
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
        if (Excel导入 == null)
        {
            await MockDownLoadAsync();
        }
        else
        {
            await Excel导入();
        }

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

        await TableMain.QueryAsync();
    }

    public async Task ImportItems()
    {
        if (导入 == null)
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
        await 导入();

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

        await TableMain.QueryAsync();
    }
    public async Task ImportItemsII()
    {
        if (导入II == null)
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
        await 导入II();

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

        await TableMain.QueryAsync();
    }

    public async Task 执行添加Cmd()
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
        if (执行添加 == null)
        {
            await MockDownLoadAsync();
        }
        else
        {
            await 执行添加();
        }

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
    #endregion

    #region 导出数据方法

    /// <summary>
    /// 导出数据方法
    /// </summary>
    /// <param name="Items"></param>
    /// <param name="opt"></param>
    /// <returns></returns>
    protected async Task<bool> ExportAsync(ITableExportDataContext<TItem> items)
    {
        var ret = false;
        if (OnExportAsync != null)
        {
            ret = await OnExportAsync(items);
        }
        else
        {
            ret = await ExportExcelAsync(items.Rows);
        }
        return ret;
    }

    public async Task<bool> ExportExcelAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, UseMiniExcel ? ExportType.MiniExcel : ExportType.Excel);
    public async Task<bool> ExportPDFAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, ExportType.Pdf);
    public async Task<bool> ExportWordAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, UseMiniWord ? ExportType.MiniWord : ExportType.Word);
    public async Task<bool> ExportHtmlAsync(IEnumerable<TItem>? items) => await ExportAutoAsync(items, ExportType.Html);

    public async Task<bool> ExportAutoAsync(IEnumerable<TItem>? items, ExportType? exportType = ExportType.MiniExcel)
    {
        var expType = exportType ?? (UseMiniExcel ? ExportType.MiniExcel : ExportType.Excel);

        if (导出 == null && (items == null || !items.Any()) && (ItemsCache == null || !ItemsCache.Any()))
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
        if (导出 == null)
        {
            await Export(items!.ToList(), expType);
        }
        else
        {
            await 导出(items!, expType);
        }

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
            ExportBasePath ??= Path.Combine(Path.GetTempPath());
            if (ExportBasePath != null)
            {
                if (Directory.Exists(ExportBasePath) == false)
                {
                    Directory.CreateDirectory(ExportBasePath);
                }

                fileName = Path.Combine(ExportBasePath, fileName);
            }

            if (items == null || !items.Any())
            {
                items = ItemsCache?.ToList();
            }

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
                await DownloadService.DownloadFromStreamAsync(Path.GetFileName(fileName) ?? "file", memoryStream);
            }
            else
            {
                fileName = await Exporter.Export(fileName, items, exportType);
                ToastService?.Success("提示", Path.GetFileName(fileName) + "已生成");
                await DownloadService.DownloadFromFileAsync(Path.GetFileName(fileName) ?? "file", fileName);
            }

        }
        catch (Exception e)
        {
            ToastService?.Error($"导出", $"{exportType}出错,请检查. {e.Message}");
        }
    }

    #endregion

    #region 工具栏按钮
    public async Task 升级Cmd(IEnumerable<TItem> items)
    {
        if (升级 != null)
        {
            await 升级(items);
            await TableMain.QueryAsync();
            ToastService?.Success($"{升级按钮文字}成功", $"{升级按钮文字}成功,请检查数据");
        }
    }

    public async Task 升级IICmd(IEnumerable<TItem> items)
    {
        if (升级II != null)
        {
            await 升级II(items);
            ToastService?.Success($"{升级按钮II文字}成功", $"{升级按钮II文字}成功,请检查数据");
        }
    }

    public async Task ClickRow(TItem item)
    {
        if (OnClickRowCallback != null)
        {
            await OnClickRowCallback.Invoke(item);
        }
    }

    public async Task OnRowButtonClick(TItem item, RowButtonField RowButtonField)
    {
        if (RowButtonField.CallbackFunc != null)
        {
            await RowButtonField.CallbackFunc.Invoke(item);
            await TableMain.QueryAsync();
        }
    }

    public Task PrintPreview(IEnumerable<TItem> item)
    {
        JsRuntime.InvokeAsync<object>(
        "toolsFunctions.printpreview", 100
        );
        return Task.CompletedTask;
    }

    public Task 新窗口打开()
    {
        if (string.IsNullOrEmpty(新窗口打开Url))
        {
            ToastService?.Error("提示", "Url为空!");
            return Task.CompletedTask;
        }
        JsRuntime.NavigateToNewTab(新窗口打开Url);
        return Task.CompletedTask;
    }
    public Task 查找文本Icmd()
    {
        if (string.IsNullOrEmpty(查找文本I))
        {
            ToastService?.Error("提示", "Url为空!");
            return Task.CompletedTask;
        }
#pragma warning disable BL0005
        TableMain.SearchText = 查找文本I;
#pragma warning restore BL0005
        return Task.CompletedTask;
    }

    public Task IsExcelToggle()
    {
        IsExcel = !IsExcel;
        DoubleClickToEdit = !IsExcel;
        StateHasChanged();
        return Task.CompletedTask;
    }

    #endregion


    public Task ScrollModeToggle()
    {
        scrollMode = scrollMode == ScrollMode.Virtual ? ScrollMode.None : ScrollMode.Virtual;

        ScrollMode = scrollMode;
        if (AutoPageHeight)
        {
            Height = PageHeight - 150;
        }

        IsPagination = scrollMode == ScrollMode.None;
        PageItems = scrollMode == ScrollMode.Virtual ? 30 : 15;
        AllowDragColumn = !IsPagination;
        AllowResizing = !IsPagination;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public void SetFooter(string value)
    {
        //FooterTotalOneLine = value;
        //调用刷新会复位分页页码选择之类,所以不用
        //StateHasChanged();
        cacheFooterValue = value;
        FloatPanel?.Set(value);
        FloatPanelUp?.Set(value);
    }

}
