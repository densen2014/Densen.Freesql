// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AME;
using BootstrapBlazor.Components; 
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering; 
using static AME.EnumsExtensions;

namespace AmeBlazor.Components;

/// <summary>
/// TableAmePro 组件,使用 FreeSqlDataService 注入服务维护表以及详表
/// <para></para>
/// 后三个参数可用NullClass空类
/// </summary>
/// <typeparam name="TItem">主表类名</typeparam>
/// <typeparam name="ItemDetails">详表类名</typeparam>
/// <typeparam name="ItemDetailsII">详表的详表类名</typeparam>
/// <typeparam name="ItemDetailsIII">详表3类名</typeparam>
public partial class TableAmeProDetailsIII<TItem, ItemDetails, ItemDetailsII, ItemDetailsIII> : TableAmeProDetails<TItem, ItemDetails>
    where TItem : class, new()
    where ItemDetails : class, new()
    where ItemDetailsII : class, new()
    where ItemDetailsIII : class, new()
{

    #region 动态生成控件 


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
        if (Type.GetTypeCode(FieldType) == TypeCode.Int32)
        {
            builder.AddAttribute(2, "Template", DialogTableDetails<int>());
        }
        else
        {
            builder.AddAttribute(2, "Template", DialogTableDetails<string>());
        }
        builder.AddAttribute(3, "Visible", true);
        builder.CloseComponent();
    };

    /// <summary>
    /// 动态生成明细弹窗控件
    /// </summary>
    /// <returns></returns>
    public virtual RenderFragment<TableColumnContext<TItem, TValue>> DialogTableDetails<TValue>() 
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
        builder.OpenComponent(0, typeof(TableColumn<,>).MakeGenericType(typeof(TItem), FieldType));
        builder.AddAttribute(1, "FieldExpression", fieldExpresson);

        // 添加模板
        if (Type.GetTypeCode(FieldType) == TypeCode.Int32)
        {
            builder.AddAttribute(2, "Template", DialogTableDetails<int>());
        }
        else
        {
            builder.AddAttribute(2, "Template", DialogTableDetails<string>());
        }
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
        if (SubAddAsync != null)
        {
            builder.AddAttribute(32, nameof(AddAsync), SubAddAsync);
        }
        builder.CloseComponent();
    };

    /// <summary>
    /// 明细表控件
    /// </summary>
    /// <param name="rowType"></param>
    /// <param name="builder"></param>
    public virtual void TRenderTable(TableDetailRowType rowType, RenderTreeBuilder builder)
    {
        if (rowType == TableDetailRowType.选项卡2)
        {
            builder.OpenComponent<TableAmePro<ItemDetailsII, NullClass, NullClass, NullClass>>(0);
        }
        else if (ShowDetailRowType == ShowDetailRowType.表内明细II)
        {
            builder.OpenComponent<TableAmePro<ItemDetails, ItemDetailsII, NullClass, NullClass>>(0);
        }
        else if (rowType == TableDetailRowType.选项卡3)
        {
            builder.OpenComponent<TableAmePro<ItemDetailsIII, NullClass, NullClass, NullClass>>(0);
        }
        else
        {
            builder.OpenComponent<TableAmePro<ItemDetails, NullClass, NullClass, NullClass>>(0);
        }
    }


    #endregion


}
