﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER

namespace BootstrapBlazor.Components;

/// <summary>
/// 过滤器接口
/// </summary>
public interface IFilter
{
    /// <summary>
    /// 获得/设置 本过滤器相关 IFilterAction 实例
    /// </summary>
    [NotNull]
    IFilterAction? FilterAction { get; set; }
}

#endif
