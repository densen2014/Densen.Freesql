// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using Densen.Models.ids;
using System.Runtime.Caching;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 演示网站示例数据注入服务实现类
/// </summary>
internal class DemoLookupService : ILookupService
{
    private IServiceProvider Provider { get; }

    private IFreeSql fsql { get; set; }

    private string? LookupKey { get; set; }

    private ObjectCache cache = MemoryCache.Default;

    private CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
    {
        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(60.0),
    };

    public DemoLookupService(IServiceProvider provider, IFreeSql fsql)
    {
        Provider = provider;
        this.fsql = fsql;
    }

    public IEnumerable<SelectedItem>? GetItemsByKey(string? key)
    {
        IEnumerable<SelectedItem>? items = null;
        var cacheKey = $"{key}_{LookupKey}";
        if (cache.Get(cacheKey) != null)
        {
            return cache.Get(cacheKey) as IEnumerable<SelectedItem>;
        }
        else
        {
            if (key == "Provideres")
            {
                items = new List<SelectedItem>()
            {
                new() { Value = "True", Text = "真真" },
                new() { Value = "False", Text = "假假" },
                new() { Value = "Google", Text = "谷歌" },
                new() { Value = "Google2", Text = "谷歌" },
                new() { Value = "Google3", Text = "谷歌" },
                new() { Value = "Google4", Text = "谷歌" },
                new() { Value = "Google5", Text = "谷歌" },
                new() { Value = "Google6", Text = "谷歌" },
                new() { Value = "Google7", Text = "谷歌" },
                new() { Value = "Google8", Text = "谷歌" },
                new() { Value = "Google9", Text = "谷歌" },
                new() { Value = "Googlea", Text = "谷歌" },
                new() { Value = "Googleb", Text = "谷歌" },
                new() { Value = "Googlec", Text = "谷歌" },
                new() { Value = "Googled", Text = "谷歌" },
                new() { Value = "Googlee", Text = "谷歌" },
                new() { Value = "Googlef", Text = "谷歌" },
                new() { Value = "Googlei", Text = "谷歌" },
                new() { Value = "Googlej", Text = "谷歌" },
                new() { Value = "Googlek", Text = "谷歌" },
            };
            }
            else if (key == nameof(AspNetUsers.UserName))
            {
                items = fsql.Select<AspNetUsers>().Distinct().ToList(a => a.UserName).Select(a => new SelectedItem() { Value = a, Text = a }).ToList();
            }
            else if (key == nameof(AspNetUserRoles.RoleId))
            {
                items = fsql.Select<AspNetUserRoles>()
                    .WhereIf(LookupKey != null, a => a.UserId == LookupKey)
                    .Distinct()
                    .ToList(a => new { a.RoleId, a.AspNetRoless.Name })
                    .Select(a => new SelectedItem() { Value = a.RoleId, Text = a.Name })
                    .ToList();
            }
            else if (key != null && key.StartsWith("SetLookupKey:"))
            {
                LookupKey = key.Replace("SetLookupKey:", "");
            }
            if (items != null) cache.Add(cacheKey, items, cacheItemPolicy);
            return items;
        }
    }
}
