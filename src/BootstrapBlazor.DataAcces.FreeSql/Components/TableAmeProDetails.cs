// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;

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
public partial class TableAmeProDetails<TItem, ItemDetails> : TableAmeProBase<TItem>
    where TItem : class, new()
    where ItemDetails : class, new()
{

    /// <summary>
    /// 获得/设置 子表编辑按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task>? SubEditAsync { get; set; }

    /// <summary>
    /// 获得/设置 子表新建按钮回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, Task<ItemDetails>>? SubAddAsync { get; set; }

    /// <summary>
    /// 获得/设置 子表保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, ItemChangedType, Task<ItemDetails>>? SubSaveAsync { get; set; }

    /// <summary>
    /// 获得/设置 明细表保存按钮异步回调方法
    /// </summary>
    [Parameter]
    public Func<ItemDetails, ItemChangedType, Task<ItemDetails>>? DetailsSaveAsync { get; set; }
}
