BootstrapBlazor 的 Table 扩展

### 示例1：PinPaiName=品牌商1

```
@using FreeSql.Internal.Model

<TableAdv DynamicFilterInfo="DynamicFilterInfo" />

@code {
    DynamicFilterInfo DynamicFilterInfo =>
        new DynamicFilterInfo()
        {
            Field = nameof(XianMu.PinPaiName),
            Operator = DynamicFilterOperator.Equal,
            Value = "品牌商1"
        };
}
```

### 示例2：(PinPaiName=品牌商1 && QuDaoShangName=渠道商2) || TuiGuangShangName=推广商3

```
@using FreeSql.Internal.Model

<TableAdv DynamicFilterInfo="DynamicFilterInfo" />

@code {
    DynamicFilterInfo DynamicFilterInfo => GenDynamicFilterInfo();

    DynamicFilterInfo GenDynamicFilterInfo()
    {
        //第一层
        var filters = new List<DynamicFilterInfo>();

        filters.Add(new DynamicFilterInfo()
        {
            Field = nameof(XianMu.PinPaiName),
            Operator = DynamicFilterOperator.Equal,
            Value = "品牌商1"
        });


        filters.Add(new DynamicFilterInfo()
        {
            Field = nameof(XianMu.QuDaoShangName),
            Operator = DynamicFilterOperator.Equal,
            Value = "渠道商2"
        });

        DynamicFilterInfo filterWhereCascadeAnd = new DynamicFilterInfo()
        {
            Logic = DynamicFilterLogic.And,
            Filters = filters
        };


        //第二层
        var filters2 = new DynamicFilterInfo()
        {
            Field = nameof(XianMu.TuiGuangShangName),
            Operator = DynamicFilterOperator.Equal,
            Value = "推广商3"
        };

        //生成带预设条件的复合查询
        var filtersWhereCascade = new List<DynamicFilterInfo>();
        filtersWhereCascade.Add(filterWhereCascadeAnd);
        filtersWhereCascade.Add(filters2);

        DynamicFilterInfo filterWhereCascadeOr = new DynamicFilterInfo()
        {
            Logic = DynamicFilterLogic.Or,
            Filters = filtersWhereCascade
        };

        return filterWhereCascadeOr;
    }
}
```