using FreeSql;

namespace Densen.DataAcces.FreeSql;

/// <summary>
/// FreeSql 多库操作 , 为 FreeSql 提供跨数据库访问
/// </summary>
/// <remarks>为 FreeSql 提供跨数据库访问，分布式事务TCC、SAGA解决方案
/// <para>开源地址：https://github.com/2881099/FreeSql.Cloud</para></remarks>
public class FsqlCloud : FreeSqlCloud<string>
{
    public FsqlCloud() : base(null) { }

    public FsqlCloud(string distributekey) : base(distributekey) { }
}
 
