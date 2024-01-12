// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AME;
using BootstrapBlazor.Components;
using Densen.DataAcces.FreeSql;
using FreeSql;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using static AME.EnumsExtensions;

namespace AmeBlazor.Components;

/// <summary>
/// TablePollo 组件,使用Idlebus注入服务维护表以及详表
/// <para></para>
/// 后两个参数可用NullClass空类
/// </summary>
/// <typeparam name="TItem">主表类名</typeparam>
/// <typeparam name="ItemDetails">详表类名</typeparam>
/// <typeparam name="ItemDetailsII">详表的详表类名</typeparam>
/// <typeparam name="ItemDetailsII">详表3类名</typeparam>
public partial class TablePollo<TItem, ItemDetails, ItemDetailsII, ItemDetailsIII> : TableAmeProDetails<TItem, ItemDetails>
    where TItem : class, new()
    where ItemDetails : class, new()
    where ItemDetailsII : class, new()
    where ItemDetailsIII : class, new()
{

    TablePollo<ItemDetails, ItemDetailsII, ItemDetailsIII, NullClass>? detalisTable;

    #region 数据服务
    /// <summary>
    /// 获得/设置 注入数据服务
    /// </summary>
    [Inject]
    [NotNull]
    protected IEnumerable<FreeSqlDataService<TItem>>? DataServices { get; set; }

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

    public async Task<bool> OnDeleteAsync(IEnumerable<TItem> items) => await GetDataService().DeleteAsync(items);

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

        if (AfterQueryAsync != null)
        {
            var select = ISelectCache();
            if (SelectCache != select)
            {
                SelectCache = select;
                AfterQueryAsync(ISelectCache());
            }
        }

        return items1;
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

    #region 动态生成控件 


    /// <summary>
    /// 动态生成控件 TableColumn 明细行按钮
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private RenderFragment RenderTableColumn(TItem model) => builder =>
    {
        var fieldExpresson = GetExpression(model, FieldD ?? Field ?? "ID"); // 刚才你的那个获取表达式 GetExpression() 的返回值的
        builder.OpenComponent(0, typeof(TableColumn<,>).MakeGenericType(typeof(TItem), FieldType));
        builder.AddAttribute(1, "FieldExpression", fieldExpresson);
        // 这里继续添加你原来 Razor 文件中的哪些属性

        // 添加模板
        builder.AddAttribute(2, "Template", new RenderFragment<TableColumnContext<TItem, int>>(context => buttonBuilder =>
        {
            // 这里写按钮的
            buttonBuilder.OpenComponent<Button>(0);
            buttonBuilder.AddAttribute(2, nameof(Button.Text), "明细");
            buttonBuilder.AddAttribute(1, nameof(Button.OnClickWithoutRender), new Func<Task>(async () =>
            {
                var op = new DialogOption()
                {
                    BodyContext = context,
                    BodyTemplate = RenderTableDetailRow(context.Row),
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
                                await TableMain.QueryAsync();
                        }
                    },
                    OnSaveAsync = async () => {
                        if (DetailsDialogSaveAsync != null)
                        {
                            if (await DetailsDialogSaveAsync(context.Row))
                                await TableMain.QueryAsync();
                        }
                        return true;
                    },
                };
                await DialogService.Show(op);
                //detalisTable!.FieldValueD = (context.Row).GetIdentityKey(Field);
                //await ExtraLargeModal!.Toggle();
                //await detalisTable.QueryAsync();

            }));
            buttonBuilder.CloseComponent();
        }));
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
        if (SubPageItems != null) {
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
        if (DetailsSaveAsync != null)
        {
            builder.AddAttribute(28, nameof(SaveAsync), DetailsSaveAsync);
        }
        builder.AddAttribute(29, nameof(IsExcel), SubIsExcel);
        if (SubSaveAsync != null)
        {
            builder.AddAttribute(30, nameof(SaveAsync), SubSaveAsync);
        }
        if (SubEditAsync != null)
        {
            builder.AddAttribute(31, nameof(EditAsync), SubEditAsync);
        }
        builder.CloseComponent();
    };

    #endregion
     



}

