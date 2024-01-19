// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

namespace AmeBlazor.Components;

/// <summary>
/// 已改名TableAmePro 组件,使用 FreeSqlDataService 注入服务维护表以及详表
/// <para></para>
/// 后三个参数可用NullClass空类
/// </summary>
/// <typeparam name="TItem">主表类名</typeparam>
/// <typeparam name="ItemDetails">详表类名</typeparam>
/// <typeparam name="ItemDetailsII">详表的详表类名</typeparam>
/// <typeparam name="ItemDetailsII">详表3类名</typeparam>
public partial class TableAmePro<TItem, ItemDetails, ItemDetailsII, ItemDetailsIII> : TableAmeProDetailsIII<TItem, ItemDetails, ItemDetailsII, ItemDetailsIII>
    where TItem : class, new()
    where ItemDetails : class, new()
    where ItemDetailsII : class, new()
    where ItemDetailsIII : class, new()
{
}
 
