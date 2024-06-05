BootstrapBlazor的FreeSql数据注入服务扩展包

1. [注入服务](https://github.com/densen2014/Densen.Freesql/blob/master/Demo/BlazorApp1/Program.cs)
    
    ```
    //添加FreeSql服务
    builder.Services.AddFreeSql(option =>
    {
        option.UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=demo.db;")
    #if DEBUG
             //开发环境:自动同步实体
             .UseAutoSyncStructure(true)
             .UseNoneCommandParameter(true)
            //调试sql语句输出
             .UseMonitorCommand(cmd => System.Console.WriteLine(cmd.CommandText + Environment.NewLine))
    #endif
        ;
    });

    //全功能版
    builder.Services.AddTransient(typeof(FreeSqlDataService<>));
    ```

    数据服务支持的方法:

    - IncludeByPropertyNames: 附加IncludeByPropertyName查询条件, 单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. (暂时只支持一个then附加)
    - OrderByPropertyName: 强制排序,但是手动排序优先, ["len(CustomerID)", "CustomerID"]
    - LeftJoinString: 左联查询，使用原生sql语法
    - WhereCascade: 附加查询条件使用and结合
    - WhereCascadeOr: 附加查询条件使用or结合
    - WhereLamda: 查询条件，Where(a => a.Id > 10)，支持导航对象查询


2. [TableAmePro 组件使用示例](https://github.com/densen2014/Densen.Freesql/tree/master/Demo/BlazorApp1)
 
    - 通过注入数据服务直接操作实体类的CRUD操作, 无需编写任何后端代码, 无需编写任何前端代码, 无需编写任何SQL语句
    - 增删改查, 导出Excel, 导出Word
    - 支持多表联查
    - 支持多库切换
    - 保存时级联保存, 默认 false
    - 自动保存当前页码, 默认 false
    - Excel模式切换
    - 虚拟滚动/分页模式切换
    

    ```
    <h4>用户表</h4>

    <TableAmePro TItem="AspNetUsers" ItemDetails="NullClass" ItemDetailsII="NullClass" ItemDetailsIII="NullClass" ShowColumnList />

    <TableAmePro TItem="AspNetUsers"
        IncludeByPropertyNames="@IncludeAspNetUsers"
        ItemDetails="AspNetUserRoles"
        SubIncludeByPropertyNames="@SubIncludeByPropertyNames"
        ItemDetailsII="NullClass"
        ItemDetailsIII="NullClass"
        ShowColumnList
        ShowExportButton
        ShowDetailRowS
        Field="@nameof(AspNetUsers.Id)"
        FieldD="@nameof(AspNetUserRoles.UserId)"
        ExportToStream="false"
        ExportBasePath="temp"/>

    @code{

        //通过 UserId 联表读取角色表 AspNetUserRoles 指定用户数据, 但是AspNetUsers表主键是 Id 字段, 详表指定 FieldD="UserId"

        //附加导航IncludeByPropertyName查询条件
        List<string> IncludeAspNetUsers
        {
            get => new List<string> {
                    $"{nameof(AspNetUsers.AspNetUserRoless)},{nameof(AspNetUserRoles.AspNetRoless)}" ,
            };
        }

        List<string> SubIncludeByPropertyNames = [nameof(AspNetUserRoles.AspNetRoless)];


    }

3. [FreeSql.Cloud 多库操作服务](https://github.com/densen2014/Densen.Freesql/wiki/%E5%85%B3%E4%BA%8E-FreeSql.Cloud-%E5%A4%9A%E5%BA%93%E6%93%8D%E4%BD%9C%E6%9C%8D%E5%8A%A1) (AddFreeSqlCloud), 提供跨数据库访问




## 更新日志

2024-2-9
- 增加 [FreeSql.Cloud 多库操作服务](https://github.com/densen2014/Densen.Freesql/wiki/%E5%85%B3%E4%BA%8E-FreeSql.Cloud-%E5%A4%9A%E5%BA%93%E6%93%8D%E4%BD%9C%E6%9C%8D%E5%8A%A1) (AddFreeSqlCloud), 提供跨数据库访问，分布式事务TCC、SAGA解决方案
- TableAmePro 组件使用 ConnectionString 作为key,在服务里 fsql = fsqlCloud.Use(connectionString) 获取当前库的实例
- [FreeSql.Cloud](https://github.com/2881099/FreeSql.Cloud)

2024-2-5
- 添加 SubIsSimpleUI 详表简化UI , IsReadonly 双击弹窗只读
- 添加 SubIsReadonly 详表只读

2024-2-1
- 添加 FooterContent

2024-1-18
- 缓存当前编辑实体
- 多主键实体,保存前先删除(利用缓存实体比对)
- 去除内存排序和搜索功能, 业务层不封装

2024-1-12
- 修复分页数量由大切小导致不刷新项目的错误
- DataAcces.FreeSql 支持更多功能

2024-1-3
- TableAmePro 添加主表只读 IsReadonly 属性, 详表组件工作模式为 Excel 模式SubIsExcel

2023-09-30
- TableAmePro 添加参数 自动保存当前页码 AutoSavePageIndex, 当前页码 PageIndex

2023-6-3
- 添加 ItemDetailsIII , 选项卡3, 附加查询条件III
