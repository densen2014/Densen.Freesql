// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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
    /// 获得/设置 子表新建按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task<ItemDetails>>? SubAddAsync { get; set; }

    /// <summary>
    /// 获得/设置 子表编辑按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task>? SubEditAsync { get; set; }

    /// <summary>
    /// 获得/设置 子表保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, ItemChangedType, Task<ItemDetails>>? SubSaveAsync { get; set; }

    /// <summary>
    /// 获得/设置 子表删除按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<IEnumerable<TItem>, Task<bool>>? SubDeleteAsync { get; set; }

    /// <summary>
    /// 附加属性
    /// </summary>
    /// <param name="builder"></param>
    public override void TRenderTableAdditionalAttributes(RenderTreeBuilder builder)
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
            builder.AddAttribute(52, nameof(SaveAsync), SubSaveAsync);
        }
        if (SubDeleteAsync != null)
        {
            builder.AddAttribute(52, nameof(DeleteAsync), SubDeleteAsync);
        }
    }
}
