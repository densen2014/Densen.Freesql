// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AmeBlazor.Components;
using Densen.Models.ids;
using FreeSql;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BlazorApp1.Pages;

public partial class TableDetailsDemo
{

    private bool IsDetails { get; set; }
    private bool IsFirstLoad { get; set; } = true;

    [NotNull]
    protected TablePollo<AspNetUserRoles, NullClass, NullClass, NullClass>? TableDetails { get; set; }

    private Expression<Func<AspNetUserRoles, bool>>? DWhere1 { get; set; } = null;

    private async Task ClickRow(AspNetUsers item)
    {
        IsFirstLoad = false;
        IsDetails = true;
        DWhere1 = a => a.UserId == item.Id;
        await TableDetails.QueryAsync();
        StateHasChanged();
    }

    Task 总表()
    {
        IsDetails = false;
        return Task.CompletedTask;
    }

    Task 明细()
    {
        IsDetails = true;
        return Task.CompletedTask;
    }

    List<string> IncludeAspNetUsers
    {
        get => new List<string> {
                nameof(AspNetUserRoles.AspNetRoless) ,
            };
    }
}
