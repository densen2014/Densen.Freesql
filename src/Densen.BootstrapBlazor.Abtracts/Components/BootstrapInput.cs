// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace BootstrapBlazor.Components;


/// <summary>
/// BootstrapInput 组件
/// </summary>
public partial class BootstrapInput<TValue>
{
}

/// <summary>
/// 可为空布尔值组件
/// </summary>
public partial class NullSwitch
{
}

/// <summary>
/// DateTimePicker 组件
/// </summary>
public partial class DateTimePicker<TValue>
{
}

/// <summary>
/// An input component for editing numeric values.
/// Supported numeric types are <see cref="int"/>, <see cref="long"/>, <see cref="short"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>.
/// </summary>
public partial class BootstrapInputNumber<TValue>
{
}
/// <summary>
/// Select 组件实现类
/// </summary>
/// <typeparam name="TValue"></typeparam>
public partial class Select<TValue> : ISelect, ILookup
{
    IEnumerable<SelectedItem>? ILookup.Lookup { get; set; }
    StringComparison ILookup.LookupStringComparison { get; set; }
    string? ILookup.LookupServiceKey { get; set; }
    object? ILookup.LookupServiceData { get; set; }
    ILookupService? ILookup.LookupService { get; set; }

    public void Add(SelectedItem item)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// ISelect 接口
/// </summary>
public interface ISelect
{
    /// <summary>
    /// 增加 SelectedItem 项方法
    /// </summary>
    /// <param name="item"></param>
    void Add(SelectedItem item);
}
/// <summary>
/// Lookup 接口定义
/// </summary>
public interface ILookup
{
    /// <summary>
    /// 获得/设置 数据集用于 CheckboxList Select 组件 通过 Value 显示 Text 使用 默认 null
    /// <para>设置 <see cref="Lookup"/> 参收后，<see cref="LookupServiceKey"/> 和 <see cref="LookupServiceData"/> 两个参数均失效</para>
    /// </summary>
    IEnumerable<SelectedItem>? Lookup { get; set; }

    /// <summary>
    /// 获得/设置 字典数据源字符串比较规则 默认 <see cref="StringComparison.OrdinalIgnoreCase" /> 大小写不敏感 
    /// </summary>
    StringComparison LookupStringComparison { get; set; }

    /// <summary>
    /// 获得/设置 <see cref="ILookupService"/> 服务获取 Lookup 数据集合键值 常用于外键自动转换为名称操作，可以通过 <see cref="LookupServiceData"/> 传递自定义数据
    /// <para>未设置 <see cref="Lookup"/> 时生效</para>
    /// </summary>
    string? LookupServiceKey { get; set; }

    /// <summary>
    /// 获得/设置 <see cref="ILookupService"/> 服务获取 Lookup 数据集合键值自定义数据，通过 <see cref="LookupServiceKey"/> 指定键值
    /// <para>未设置 <see cref="Lookup"/> 时生效</para>
    /// </summary>
    object? LookupServiceData { get; set; }

    /// <summary>
    /// 获得/设置 <see cref="ILookupService"/> 服务实例
    /// </summary>
    ILookupService? LookupService { get; set; }
}

/// <summary>
/// ILookupService 接口
/// </summary>
public interface ILookupService
{
    /// <summary>
    /// 根据指定键值获取 Lookup 集合方法
    /// </summary>
    /// <param name="key">获得 Lookup 数据集合键值</param>
    [Obsolete("已弃用，请使用 data 参数重载方法；Deprecated, please use the data parameter method")]
    [ExcludeFromCodeCoverage]
    IEnumerable<SelectedItem>? GetItemsByKey(string? key);

    /// <summary>
    /// 根据指定键值获取 Lookup 集合方法
    /// </summary>
    /// <param name="key">获得 Lookup 数据集合键值</param>
    /// <param name="data">Lookup 键值附加数据</param>
    IEnumerable<SelectedItem>? GetItemsByKey(string? key, object? data);

    /// <summary>
    /// 根据指定键值获取 Lookup 集合异步方法
    /// </summary>
    /// <param name="key">获得 Lookup 数据集合键值</param>
    /// <param name="data">Lookup 键值附加数据</param>
    Task<IEnumerable<SelectedItem>?> GetItemsByKeyAsync(string? key, object? data);
}
#endif
