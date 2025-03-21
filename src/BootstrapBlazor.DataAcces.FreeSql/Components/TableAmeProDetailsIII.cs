// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

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

    /// <summary>
    /// 获得/设置 明细表2是否显示添加按钮 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool SubShowAddButton2 { get; set; } = true;

    /// <summary>
    /// 获得/设置 明细表2是否显示批量添加按钮 默认为 false 不显示
    /// </summary>
    [Parameter]
    public bool SubShowBatchAddButton2 { get; set; } = false;

    /// <summary>
    /// 获得/设置 明细表2批量添加按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetailsII, Task<bool>>? SubBatchAddAsync2 { get; set; }

    /// <summary>
    /// 获得/设置 明细表新建按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetailsII, Task<ItemDetailsII>>? SubAddAsync2 { get; set; }

    /// <summary>
    /// 获得/设置 明细表编辑按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetailsII, Task>? SubEditAsync2 { get; set; }

    /// <summary>
    /// 获得/设置 刷新明细表2
    /// </summary>
    [Parameter]
    public Func<ItemDetailsII, Task>? SubRefresh2 { get; set; }

    /// <summary>
    /// 获得/设置 明细表保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetailsII, ItemChangedType, Task<ItemDetailsII>>? SubSaveAsync2 { get; set; }

    /// <summary>
    /// 明细表保存数据后异步回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetailsII, ItemChangedType, Task>? SubAfterSaveAsync2 { get; set; }

    /// <summary>
    /// 获得/设置 明细表删除按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<IEnumerable<ItemDetailsII>, Task<bool>>? SubDeleteAsync2 { get; set; }

    /// <summary>
    /// 获得/设置 明细表2模板
    /// </summary>
    [Parameter]
    public RenderFragment<ItemDetailsII>? SubTableColumns2 { get; set; }

    /// <summary>
    /// 获得/设置 明细表模板III
    /// </summary>
    [Parameter]
    public RenderFragment<ItemDetailsIII>? SubTableColumns3 { get; set; }

    #region 动态生成控件 


    /// <summary>
    /// 明细表控件
    /// </summary>
    /// <param name="rowType"></param>
    /// <param name="builder"></param>
    public override void TRenderTable(TableDetailRowType rowType, RenderTreeBuilder builder)
    {
        if (rowType == TableDetailRowType.选项卡2)
        {
            builder.OpenComponent<TableAmePro<ItemDetailsII, NullClass, NullClass, NullClass>>(0);
            if (SubShowBatchAddButton2)
            {
                builder.AddAttribute(101, nameof(ShowAddButton), SubShowAddButton2);
                builder.AddAttribute(100, nameof(ShowBatchAddButton), SubShowBatchAddButton2);
                if (SubBatchAddAsync != null)
                {
                    builder.AddAttribute(100, nameof(BatchAddAsync), SubBatchAddAsync2);
                }
            }
            if (SubTableColumns2 != null)
            {
                builder.AddAttribute(1001, nameof(TableColumns), SubTableColumns2);
            }
            if (SubAddAsync2 != null)
            {
                builder.AddAttribute(1002, nameof(AddAsync), SubAddAsync2);
            }
            if (SubEditAsync2 != null)
            {
                builder.AddAttribute(1003, nameof(EditAsync), SubEditAsync2);
            }
            if (SubSaveAsync2 != null)
            {
                builder.AddAttribute(1004, nameof(SaveAsync), SubSaveAsync2);
            }
            if (SubAfterSaveAsync2 != null)
            {
                builder.AddAttribute(1005, nameof(AfterSaveAsync), SubAfterSaveAsync2);
            }
            if (SubDeleteAsync2 != null)
            {
                builder.AddAttribute(1006, nameof(DeleteAsync), SubDeleteAsync2);
            }
            if (SubRefresh2 != null)
            {
                builder.AddAttribute(1007, nameof(Refresh), SubRefresh2);
            }
        }
        else if (ShowDetailRowType == ShowDetailRowType.表内明细II)
        {
            builder.OpenComponent<TableAmePro<ItemDetails, ItemDetailsII, NullClass, NullClass>>(0);
            if (SubTableColumns2 != null)
            {
                builder.AddAttribute(101, nameof(TableColumns), SubTableColumns2);
            }
        }
        else if (rowType == TableDetailRowType.选项卡3)
        {
            builder.OpenComponent<TableAmePro<ItemDetailsIII, NullClass, NullClass, NullClass>>(0);
            if (SubTableColumns3 != null)
            {
                builder.AddAttribute(101, nameof(TableColumns), SubTableColumns3);
            }
        }
        else
        {
            builder.OpenComponent<TableAmePro<ItemDetails, NullClass, NullClass, NullClass>>(0);
            if (SubTableColumns != null)
            {
                builder.AddAttribute(101, nameof(TableColumns), SubTableColumns);
            }
        }
    }


    #endregion

 

}
