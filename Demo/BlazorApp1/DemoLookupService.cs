// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components; 

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 演示网站示例数据注入服务实现类
/// </summary>
internal class DemoLookupService : ILookupService
{
    private IServiceProvider Provider { get; }

    public DemoLookupService(IServiceProvider provider) => Provider = provider;

    public IEnumerable<SelectedItem>? GetItemsByKey(string? key)
    {
        IEnumerable<SelectedItem>? items = null;
        if (key == "Provideres")
        {
            items = new List<SelectedItem>()
            {
                new() { Value = "True", Text = "真真" },
                new() { Value = "False", Text = "假假" },
                new() { Value = "Google", Text = "谷歌" },
            };
        }
        return items;
    }
}
