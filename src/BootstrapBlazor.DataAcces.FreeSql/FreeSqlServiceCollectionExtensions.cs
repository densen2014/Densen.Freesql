// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using Densen.DataAcces;
using Densen.DataAcces.FreeSql;
using Densen.Service;
using FreeSql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// BootstrapBlazor 服务扩展类
/// </summary>
public static class FreeSqlServiceCollectionExtensions
{
    /// <summary>
    /// 增加 FreeSql 数据库操作服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <param name="configureAction"></param>
    /// <param name="configEntityPropertyImage"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddFreeSql(this IServiceCollection services, Action<FreeSqlBuilder> optionsAction, Action<IFreeSql>? configureAction = null, bool configEntityPropertyImage = false, FreeSqlServiceOptions? configureOptions = null)
    {
        configureOptions ??= new FreeSqlServiceOptions(configEntityPropertyImage);
        services.AddSingleton(sp =>
        {
            var builder = new FreeSqlBuilder();
            optionsAction(builder);
            var instance = builder.Build();
            instance.UseJsonMap();
            if (configEntityPropertyImage || (configureOptions?.ConfigEntityPropertyImage ?? false))
            {
                instance.Aop.AuditValue += AuditValue;
                instance.Aop.ConfigEntityProperty += ConfigEntityProperty;
            }
            configureAction?.Invoke(instance);
            return instance;
        });

        services.AddTransient(typeof(IDataService<>), typeof(FreeSqlDataService<>));
        services.TryAddTransient<IImportExport, ImportExportsMiniService>();
        services.AddSingleton(configureOptions);
        return services;
    }

    /// <summary>
    /// 增加 FreeSql.Cloud 多库操作服务, 提供跨数据库访问，分布式事务TCC、SAGA解决方案
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <param name="configureAction"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddFreeSqlCloud(this IServiceCollection services, Action<FreeSqlBuilder> optionsAction, Action<IFreeSql>? configureAction = null, FreeSqlServiceOptions? configureOptions = null)
    {
        configureOptions ??= new FreeSqlServiceOptions();
        var fsqlCloud = new FsqlCloud();
#if DEBUG
        if (configureOptions.DistributeTrace)
        {
            fsqlCloud.DistributeTrace += log => System.Console.WriteLine(log.Split('\n')[0].Trim());
        }
#endif
        services.AddSingleton(fsqlCloud);
        services.AddScoped(sp =>
        {
            fsqlCloud.Register("main", () =>
            {
                var builder = new FreeSqlBuilder();
                optionsAction(builder);
                var instance = builder.Build();
                instance.UseJsonMap();
                if (configureOptions.ConfigEntityPropertyImage)
                {
                    instance.Aop.AuditValue += AuditValue;
                    instance.Aop.ConfigEntityProperty += ConfigEntityProperty;
                }
                configureAction?.Invoke(instance);
                return instance;
            });
            //临时访问其他数据库表，使用 FreeSqlCloud 对象 Use("db3").Select<T>().ToList()
            return fsqlCloud.Use("main");
        });

        services.AddTransient(typeof(IDataService<>), typeof(FreeSqlDataService<>));
        //附加查询条件数据库操作服务
        services.AddTransient(typeof(FreeSqlDataService<>));
        services.TryAddTransient<IImportExport, ImportExportsMiniService>();
        services.AddSingleton(configureOptions);
        return services;
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

