// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace AmeBlazor.Components;

/// <summary>
/// TablePollo 组件,使用Idlebus注入服务维护表以及详表
/// <para></para>
/// 后一个参数可用NullClass空类
/// </summary>
/// <typeparam name="TItem">主表类名</typeparam>
/// <typeparam name="ItemDetails">详表类名</typeparam>
public partial class TableAmeProDetails<TItem, ItemDetails> : TableAmeProBase<TItem>
where TItem : class, new()
where ItemDetails : class, new()
{

    /// <summary>
    /// 获得/设置 明细表新建按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task<ItemDetails>>? SubAddAsync { get; set; }

    /// <summary>
    /// 获得/设置 明细表编辑按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task>? SubEditAsync { get; set; }

    /// <summary>
    /// 获得/设置 明细表保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, ItemChangedType, Task<ItemDetails>>? SubSaveAsync { get; set; }

    /// <summary>
    /// 明细表保存数据后异步回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, ItemChangedType, Task>? SubAfterSaveAsync { get; set; }

    /// <summary>
    /// 获得/设置 明细表删除按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<IEnumerable<ItemDetails>, Task<bool>>? SubDeleteAsync { get; set; }

    /// <summary>
    /// 获得/设置 编辑弹窗配置类扩展回调方法 新建/编辑弹窗弹出前回调此方法用于设置弹窗配置信息
    /// </summary>
    [Parameter]
    public Action<ITableEditDialogOption<ItemDetails>>? SubBeforeShowEditDialogCallback { get; set; }

    /// <summary>
    /// 获得/设置 明细表表格 Toolbar 按钮模板
    /// <para>明细表表格工具栏左侧按钮模板，模板中内容出现在默认按钮后面*</para>
    /// </summary>
    [Parameter]
    public RenderFragment? SubTableToolbarTemplate { get; set; }

    /// <summary>
    /// 获得/设置 明细表表格 Toolbar 按钮模板
    /// <para>表格工具栏左侧按钮模板，模板中内容出现在默认按钮前面</para>
    /// </summary>
    [Parameter]
    [NotNull]
    public RenderFragment? SubTableToolbarBeforeTemplate { get; set; }

    /// <summary>
    /// 获得/设置 明细表是否显示批量添加按钮 默认为 false 不显示
    /// </summary>
    [Parameter]
    public bool SubShowBatchAddButton { get; set; } = false;

    /// <summary>
    /// 获得/设置 明细表是否显示添加按钮 默认为 true 显示
    /// </summary>
    [Parameter]
    public bool SubShowAddButton { get; set; } = true;

    /// <summary>
    /// 获得/设置 明细表批量添加按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task<bool>>? SubBatchAddAsync { get; set; }

    /// <summary>
    /// 获得/设置 刷新明细表
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task>? SubRefresh { get; set; }

    /// <summary>
    /// 获得/设置 明细表模板
    /// </summary>
    [Parameter]
    public RenderFragment<ItemDetails>? SubTableColumns { get; set; }


    /// <summary>
    /// 附加属性
    /// </summary>
    /// <param name="builder"></param>
    public override void TRenderTableAdditionalAttributes(RenderTreeBuilder builder, TableDetailRowType rowType = TableDetailRowType.选项卡1)
    {
        if (SubAddAsync != null)
        {
            builder.AddAttribute(50, nameof(AddAsync), SubAddAsync);
        }
        if (SubEditAsync != null)
        {
            builder.AddAttribute(51, nameof(EditAsync), SubEditAsync);
        }
        if (SubSaveAsync != null)
        {
            builder.AddAttribute(521, nameof(SaveAsync), SubSaveAsync);
        }
        if (SubAfterSaveAsync != null)
        {
            builder.AddAttribute(52, nameof(AfterSaveAsync), SubAfterSaveAsync);
        }
        if (SubDeleteAsync != null)
        {
            builder.AddAttribute(53, nameof(DeleteAsync), SubDeleteAsync);
        }
        if (SubBeforeShowEditDialogCallback != null)
        {
            builder.AddAttribute(54, nameof(BeforeShowEditDialogCallback), SubBeforeShowEditDialogCallback);
        }
        if (SubTableToolbarTemplate != null)
        {
            builder.AddAttribute(55, nameof(TableToolbarTemplate), SubTableToolbarTemplate);
        }
        if (SubTableToolbarBeforeTemplate != null)
        {
            builder.AddAttribute(56, nameof(TableToolbarBeforeTemplate), SubTableToolbarBeforeTemplate);
        }
        if (rowType == TableDetailRowType.选项卡1)
        {
            builder.AddAttribute(56, nameof(ShowBatchAddButton), SubShowBatchAddButton);
            builder.AddAttribute(58, nameof(ShowAddButton), SubShowAddButton);
            if (SubBatchAddAsync != null)
            {
                builder.AddAttribute(56, nameof(BatchAddAsync), SubBatchAddAsync);
            }
        } 
        if (SubRefresh != null)
        {
            builder.AddAttribute(57, nameof(Refresh), SubRefresh);
        }
    }

   
}
