// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AmeBlazor.Components;
using Densen.Models.ids;
using FreeSql;
using System.Diagnostics.CodeAnalysis;

namespace BlazorApp1.Pages;

public partial class TableTotalDemo
{
    [NotNull]
    protected TablePollo<AspNetUsers, NullClass, NullClass, NullClass>? TableMain { get; set; }

    [NotNull]
    protected TablePollo<AspNetUsers, NullClass, NullClass, NullClass>? TableMain2 { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {

        }
    }

    /// <summary>
    /// 查询后计算合计
    /// </summary>
    /// <param name="select"></param>
    /// <returns></returns>
    protected void OnAfterQueryAsync(ISelect<AspNetUsers> select)
    {
        var _sum1 = select.Sum(d => d.PhoneNumberConfirmed);
        var _sum2 = select.Sum(d => d.AccessFailedCount);
        //FloatPanel.Set($"佣金：{_sum2:N2}  合计金额：{_sum1:N2}");
        TableMain.SetFooter($"已确认：{_sum2:N2}  访问失败：{_sum1:N2}");
    } 

    /// <summary>
    /// 查询后计算合计
    /// </summary>
    /// <param name="select"></param>
    /// <returns></returns>
    protected void OnAfterQueryAsync2(ISelect<AspNetUsers> select)
    {
        var _sum1 = select.Sum(d => d.PhoneNumberConfirmed);
        var _sum2 = select.Sum(d => d.AccessFailedCount);
        //FloatPanel.Set($"佣金：{_sum2:N2}  合计金额：{_sum1:N2}");
        TableMain2.SetFooter($"已确认：{_sum2:N2}  访问失败：{_sum1:N2}");
    } 


}
