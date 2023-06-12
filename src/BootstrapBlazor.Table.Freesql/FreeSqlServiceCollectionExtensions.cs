// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using Densen.DataAcces.FreeSql;
using Densen.Service;
using FreeSql;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// BootstrapBlazor 服务扩展类
    /// </summary>
    public static class FreeSqlServiceCollectionExtensions
    {
        /// <summary>
        /// 增加 FreeSql 数据库操作服务+Table 扩展
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddFreeSqlTables(this IServiceCollection services, Action<FreeSqlBuilder> optionsAction, Action<IFreeSql>? configureAction = null)
        {
            services.AddSingleton<IFreeSql>(sp =>
            {
                var builder = new FreeSqlBuilder();
                optionsAction(builder);
                var instance = builder.Build();
                instance.UseJsonMap();
                configureAction?.Invoke(instance);
                return instance;
            });

            services.AddSingleton(typeof(IDataService<>), typeof(FreeSqlDataService<>));
            services.AddSingleton(typeof(FreeSqlDataService<>));
            //导入导出服务
            //services.AddTransient<IImportExport, ImportExportsMiniService>();
            services.AddTransient<IImportExport, ImportExportsService>();
            return services;
        }
    }
}
