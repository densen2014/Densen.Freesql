﻿// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel;
#nullable disable

namespace Densen.Models.ids;

/// <summary>
/// 用户登录
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserLogins
{

    [DisplayName("外联登录")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string LoginProvider { get; set; }

    [AutoGenerateColumn(Visible = false, Width = 30, TextEllipsis = true)]
    [DisplayName("用户ID")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string UserId { get; set; }

    [DisplayName("外联Key")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string ProviderKey { get; set; }

    [DisplayName("外联名称")]
    [JsonProperty, Column(StringLength = -2)]
    public string ProviderDisplayName { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(UserId))]
    public virtual AspNetUsers AspNetUsers { get; set; }

}
