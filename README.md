## BootstrapBlazor.Table.Freesql:
Table 扩展

## BootstrapBlazor.Table.ImportExportsService :
Table 数据导入导出服务扩展包

## Densen.FreeSql.Extensions.BootstrapBlazor:
### BootstrapBlazor的FreeSql数据注入服务扩展包

1. 注入服务

```
builder.Services.AddFreeSql(option =>
{
    option.UseConnectionString(FreeSql.DataType.Sqlite, builder.Configuration.GetConnectionString("IdsSQliteConnection"))  //也可以写到配置文件中
#if DEBUG
         //开发环境:自动同步实体
         .UseAutoSyncStructure(true)
         .UseNoneCommandParameter(true)
    //调试sql语句输出
         .UseMonitorCommand(cmd => System.Console.WriteLine(cmd.CommandText + Environment.NewLine))
#endif
    ;
})
```

2. FreeSql ORM 的 IDataService 数据注入服务接口实现

``` 
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public override Task<QueryData<TModel>> QueryAsync(QueryPageOptions option)


        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="option"></param>
        /// <param name="WhereCascade">附加查询条件使用and结合</param>
        /// <param name="IncludeByPropertyNames">附加IncludeByPropertyName查询条件, 单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. (暂时只支持一个then附加)</param>
        /// <param name="LeftJoinString">左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")</param>
        /// <param name="OrderByPropertyName">强制排序,但是手动排序优先</param>
        /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
        /// <param name="WhereLamda">查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")</param>
        /// <returns></returns>
        public Task<QueryData<TModel>> QueryAsyncWithWhereCascade(
                    QueryPageOptions option,
                    DynamicFilterInfo? WhereCascade = null,
                    List<string>? IncludeByPropertyNames = null,
                    string? LeftJoinString = null,
                    List<string>? OrderByPropertyName = null,
                    List<string>? WhereCascadeOr = null,
                    Expression<Func<TModel, bool>>? WhereLamda = null)

        /// <summary>
        /// 全部记录
        /// </summary>
        /// <param name="WhereCascade">附加查询条件使用and结合</param>
        /// <param name="IncludeByPropertyNames">附加IncludeByPropertyName查询条件, 单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. (暂时只支持一个then附加)</param>
        /// <param name="LeftJoinString">左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")</param>
        /// <param name="OrderByPropertyName">强制排序,但是手动排序优先</param>
        /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
        /// <param name="WhereLamda">查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")</param>
        /// <returns></returns>
        public List<TModel>? GetAllItems(
                    DynamicFilterInfo? WhereCascade = null,
                    List<string>? IncludeByPropertyNames = null,
                    string? LeftJoinString = null,
                    List<string>? OrderByPropertyName = null,
                    List<string>? WhereCascadeOr = null,
                    Expression<Func<TModel, bool>>? WhereLamda = null)

```

3. FreeSql ORM 查询工具类 

FsqlUtil 

```
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="options">查询条件</param>
        /// <param name="optionsLast">缓存查询条件</param>
        /// <param name="TotalCount"></param> 
        /// <param name="fsql"></param>
        /// <param name="WhereCascade">附加查询条件使用and结合</param>
        /// <param name="IncludeByPropertyNames">附加IncludeByPropertyName查询条件.<para></para>
        /// 其中单项可逗号隔开附加查询条件的第二个参数 then，可以进行二次查询前的修饰工作. <para></para>
        /// 单项第二个逗号隔开可第三层then附加</param>
        /// <param name="LeftJoinString">左联查询，使用原生sql语法，LeftJoin("type b on b.id = a.id")</param>
        /// <param name="OrderByPropertyName">强制排序,但是手动排序优先</param>
        /// <param name="WhereCascadeOr">附加查询条件使用or结合</param>
        /// <param name="forceAllItems">附加查询条件使用or结合</param>
        /// <param name="WhereLamda">查询条件，Where(a => a.Id > 10)，支持导航对象查询，Where(a => a.Author.Email == "2881099@qq.com")</param>
        public static QueryData<TModel> Fetch<TModel>(
                                QueryPageOptions options,
                                QueryPageOptions? optionsLast,
                                long? TotalCount,
                                IFreeSql fsql,
                                DynamicFilterInfo? WhereCascade = null,
                                List<string>? IncludeByPropertyNames = null,
                                string? LeftJoinString = null,
                                List<string>? OrderByPropertyName = null,
                                List<string>? WhereCascadeOr = null,
                                bool forceAllItems = false,
                                Expression<Func<TModel, bool>>? WhereLamda = null) where TModel : class, new()

```

4. TablePollo 组件

```
        <h4>用户表</h4>

        <TablePollo TItem="AspNetUsers"/>

        <TablePollo TItem="AspNetUsers"
                    IncludeByPropertyNames="@IncludeAspNetUsers"
                    ItemDetails="AspNetUserRoles"
                    SubIncludeByPropertyNames="@SubIncludeByPropertyNames"
                    ItemDetailsII="NullClass"
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

            List<string> SubIncludeByPropertyNames = new List<string> {
                nameof(AspNetUserRoles.AspNetRoless) ,
            };


        }
```
