// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;


namespace Densen.DataAcces.FreeSql;

/// <summary>
/// 
/// </summary>
public static partial class DataTableExpression
{           
    public static Action<DataTableDynamicContext, ITableColumn> NumberFormatter()
    {
        return (context, col) =>
        {
            // 设置 数字列
            if (col.PropertyType.IsNumberType())
            {
                col.FormatString = col.PropertyType == typeof(int) ? "" : "N2";
                col.Align = Alignment.Center;
            }
            if (col.PropertyType == typeof(DateTime))
            {
                col.FormatString = "yyyy-MM-dd";
            }
        };
    }
}

