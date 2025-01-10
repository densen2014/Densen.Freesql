// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using FreeSql.Internal.Model;

namespace Densen.DataAcces.FreeSql;

/// <summary>
/// 
/// </summary>
public static partial class FreeSqlUtil
{

    private static DynamicFilterInfo ToDynamicFilter(this IFilterAction filter)
    {
        var actions = filter.GetFilterConditions();
        var item = new DynamicFilterInfo();

        if (actions.Filters != null)
        {
            item.Logic = actions.FilterLogic.ToDynamicFilterLogic();
            item.Filters = actions.Filters.Select(i => new DynamicFilterInfo()
            {
                Field = i.FieldKey,
                Value = i.FieldValue,
                Operator = i.FilterAction.ToDynamicFilterOperator()
            }).ToList();
        }
        return item;
    }

    private static DynamicFilterLogic ToDynamicFilterLogic(this FilterLogic logic) => logic switch
    {
        FilterLogic.And => DynamicFilterLogic.And,
        _ => DynamicFilterLogic.Or
    };

    private static DynamicFilterOperator ToDynamicFilterOperator(this FilterAction action) => action switch
    {
        FilterAction.Equal => DynamicFilterOperator.Equal,
        FilterAction.NotEqual => DynamicFilterOperator.NotEqual,
        FilterAction.Contains => DynamicFilterOperator.Contains,
        FilterAction.NotContains => DynamicFilterOperator.NotContains,
        FilterAction.GreaterThan => DynamicFilterOperator.GreaterThan,
        FilterAction.GreaterThanOrEqual => DynamicFilterOperator.GreaterThanOrEqual,
        FilterAction.LessThan => DynamicFilterOperator.LessThan,
        FilterAction.LessThanOrEqual => DynamicFilterOperator.LessThanOrEqual,
        _ => throw new System.NotSupportedException()
    };

}

