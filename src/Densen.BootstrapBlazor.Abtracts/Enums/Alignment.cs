﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

#if NET20_OR_GREATER

using System.ComponentModel;

namespace BootstrapBlazor.Components;

/// <summary>
/// 对齐方式枚举类型
/// </summary>
public enum Alignment
{
    /// <summary>
    /// 未设置
    /// </summary>
    None,

    /// <summary>
    /// 左对齐
    /// </summary>
    [Description("start")]
    Left,

    /// <summary>
    /// 居中对齐
    /// </summary>
    [Description("center")]
    Center,

    /// <summary>
    /// 右对齐
    /// </summary>
    [Description("end")]
    Right
}
#endif
