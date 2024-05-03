using FreeSql; 

namespace Densen.DataAcces;

/// <summary>
/// FreeSql 多库操作 , 为 FreeSql 提供跨数据库访问
/// </summary>
/// <remarks>为 FreeSql 提供跨数据库访问，分布式事务TCC、SAGA解决方案
/// <para>开源地址：https://github.com/2881099/FreeSql.Cloud</para></remarks>
public class FsqlCloud : FreeSqlCloud<string>
{
    public FsqlCloud() : base(null) { }

    public FsqlCloud(string distributekey) : base(distributekey) { }

    //fsql.Change(DbEnum.db2).Select<T>();
    ////同一线程，或异步await 后续 fsql.Select/Insert/Update/Delete 操作是 db2

    //fsql.Use(DbEnum.db2).Select<T>();
    ////单次有效
    
}

public static class FsqlCloudExtensions
{
    public static FsqlCloud AddFreeSqlCloud(Action<FreeSqlBuilder> optionsAction, Action<IFreeSql>? configureAction = null,bool ConfigEntityPropertyImage=false)
    {
        var fsqlCloud = new FsqlCloud();
#if DEBUG
        fsqlCloud.DistributeTrace += log => System.Console.WriteLine(log.Split('\n')[0].Trim());
#endif
            fsqlCloud.Register("main", () =>
            {
                var builder = new FreeSqlBuilder();
                optionsAction(builder);
                var instance = builder.Build();
                instance.UseJsonMap();
                if ( ConfigEntityPropertyImage  )
                {
                    instance.Aop.AuditValue += AuditValue;
                    instance.Aop.ConfigEntityProperty += ConfigEntityProperty;
                }
                configureAction?.Invoke(instance);
                return instance;
            });
            //临时访问其他数据库表，使用 FreeSqlCloud 对象 Use("db3").Select<T>().ToList()
            fsqlCloud.Use("main");

         return fsqlCloud;
    }

    #region 自定义Guid支持
    /// <summary>
    /// 审计属性值 , 实现插入/更新时统一处理某些值，比如某属性的雪花算法值、创建时间值、甚至是业务值。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void AuditValue(object? sender, FreeSql.Aop.AuditValueEventArgs? e)
    {
        if (e!.Column.CsType == typeof(Guid) && e.Column.Attribute.MapType == typeof(string) && e.Value?.ToString() == Guid.Empty.ToString())
        {
            e.Value = FreeUtil.NewMongodbId();
        }
    }
    #endregion 

    #region 自定义实体特性
    /// <summary>
    /// 自定义实体特性 : 自定义Enum支持和Image支持
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void ConfigEntityProperty(object? sender, FreeSql.Aop.ConfigEntityPropertyEventArgs? e)
    {

        if (e!.Property.PropertyType.IsEnum)
        {
            e.ModifyResult.MapType = typeof(int);
        }
        else if (e.Property.PropertyType == typeof(byte[]))
        {
            var orm = sender as IFreeSql;
            switch (orm?.Ado.DataType)
            {
                case DataType.SqlServer:
                    e.ModifyResult.DbType = "image";
                    break;
                case DataType.MySql:
                    e.ModifyResult.DbType = "longblob";
                    break;
                case DataType.PostgreSQL:
                    e.ModifyResult.DbType = "bytea";
                    break;
            }
        }
    }
    #endregion 


}
