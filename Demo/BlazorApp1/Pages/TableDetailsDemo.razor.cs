﻿// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AmeBlazor.Components;
using BootstrapBlazor.Components;
using Densen.Models.ids;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BlazorApp1.Pages;

public partial class TableDetailsDemo
{
    [NotNull]
    [Inject]
    private IFreeSql? fsql { get; set; }

    private bool IsDetails { get; set; }
    private bool IsFirstLoad { get; set; } = true;

    [NotNull]
    [Inject]
    ILookupService? LookupKeyService { get; set; }

    protected TablePollo<AspNetUserRoles, NullClass, NullClass, NullClass>? TableDetails { get; set; }

    private Expression<Func<AspNetUserRoles, bool>>? DWhere1 { get; set; } = null;

    [DisplayName("角色")]
    private SelectedItem? RolesName { get; set; }
    private AspNetUsers? SelectedUser { get; set; }

    private List<SelectedItem> RolesNameList { get; set; } = [new SelectedItem() { Text = "角色", Value = "" },];

    private async Task ClickRow(AspNetUsers item)
    {
        System.Console.WriteLine($"* 选择 {item.UserName} - {item.Id}");

        SelectedUser = item;
        IsFirstLoad = false;
        IsDetails = true;
        DWhere1 = a => a.UserId == item.Id;
        LookupKeyService.GetItemsByKey("SetLookupKey",item.Id);
        if (TableDetails != null)
        {
            await TableDetails.QueryAsync(DWhere1);
        }

        var list = fsql.Select<AspNetUserRoles>().Where(DWhere1).Distinct().ToList(d => d.AspNetRoless.Name);
        RolesNameList.AddRange(list.Select(d => new SelectedItem(d, d)));

        StateHasChanged();
    }

    private async Task OnCascadeBindSelectClick(SelectedItem item)
    {
        if (SelectedUser != null)
        {
            System.Console.WriteLine($"下拉选择 {item.Value}");
            DWhere1 = a => a.UserId == SelectedUser.Id;
            if (item.Value != "")
            {
                DWhere1 = DWhere1.And(a => a.AspNetRoless.Name == item.Value);
            }
            if (TableDetails != null)
            {
                await TableDetails.QueryAsync(DWhere1);
            }
            //StateHasChanged();
        }
    }

    private Task 总表()
    {
        IsDetails = false;
        return Task.CompletedTask;
    }

    private Task 明细()
    {
        IsDetails = true;
        return Task.CompletedTask;
    }

    private List<string> IncludeAspNetUsers
    {
        get => new List<string> {
                nameof(AspNetUserRoles.AspNetRoless) ,
            };
    }
}
