// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER 
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace BootstrapBlazor.Components;

/// <summary>
/// Utility 帮助类
/// </summary>
public static class Utility
{
    /// <summary>
    /// 判断当前 <see cref="ILookup"/> 实例是否配置 Lookup 数据
    /// </summary>
    /// <param name="lookup"></param>
    /// <returns></returns>
    public static bool IsLookup(this ILookup lookup) => lookup.Lookup != null || !string.IsNullOrEmpty(lookup.LookupServiceKey);

    /// <summary>
    /// 检查是否为 Number 数据类型
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsNumber(this Type t)
    {
        var targetType = Nullable.GetUnderlyingType(t) ?? t;
        return targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(short) ||
            targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal);
    }

    /// <summary>
    /// 检查是否为 Boolean 数据类型
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsBoolean(this Type t)
    {
        var targetType = Nullable.GetUnderlyingType(t) ?? t;
        return targetType == typeof(Boolean);
    }

    /// <summary>
    /// 检查是否为 DateTime 数据类型
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsDateTime(this Type t)
    {
        var targetType = Nullable.GetUnderlyingType(t) ?? t;
        var check = targetType == typeof(DateTime) || targetType == typeof(DateTimeOffset);
        return check;
    }

    /// <summary>
    /// 检查是否为 TimeSpan 数据类型
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsTimeSpan(this Type t)
    {
        var targetType = Nullable.GetUnderlyingType(t) ?? t;
        var check = targetType == typeof(TimeSpan);
        return check;
    }

    /// <summary>
    /// 获得 当前输入语言小数点分隔符
    /// </summary>
    private static string NumberDecimalSeparator => CultureInfo.CurrentCulture?.NumberFormat?.NumberDecimalSeparator ?? ".";

    /// <summary>
    /// 通过指定类型生成组件类型
    /// </summary>
    /// <param name="item"></param>
    public static Type GenerateComponentType(IEditorItem item)
    {
        var fieldType = item.PropertyType;
        Type? ret = null;
        var type = (Nullable.GetUnderlyingType(fieldType) ?? fieldType);
        if (type.IsEnum)
        {
            ret = typeof(Select<>).MakeGenericType(fieldType);
        }
        else if (fieldType == typeof(bool?))
        {
            ret = typeof(NullSwitch);
        }
        else if (fieldType.IsNumber() && NumberDecimalSeparator == ".")
        {
            ret = typeof(BootstrapInputNumber<>).MakeGenericType(fieldType);
        }
        else if (fieldType.IsDateTime())
        {
            ret = typeof(DateTimePicker<>).MakeGenericType(fieldType);
        }
        else if (fieldType.IsBoolean())
        {
            ret = typeof(Switch);
        }
        else if (fieldType == typeof(string))
        {
            ret = item.Rows > 0 ? typeof(Textarea) : typeof(BootstrapInput<>).MakeGenericType(typeof(string));
        }
        return ret ?? typeof(BootstrapInput<>).MakeGenericType(fieldType);
    }
}
#endif
