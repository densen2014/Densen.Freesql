// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Densen.Models.ids;
using FreeSql;

var fsql = new FreeSqlBuilder().UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=../../../../BlazorApp1/demo.db;")
         //开发环境:自动同步实体
         .UseAutoSyncStructure(true)
         .UseNoneCommandParameter(true)
         //调试sql语句输出
         .UseMonitorCommand(cmd => System.Console.WriteLine(cmd.CommandText + Environment.NewLine))
         .Build();

List<AspNetUsers>? users = new List<AspNetUsers>();

////列出用户带角色名称
//users = fsql.Select<AspNetUsers>().Where(a => a.Email == "test@test.com").IncludeMany(a=>a.AspNetUserRoless,then =>then.Include(c=>c.AspNetRoless)).ToList();
//users.ForEach(a => System.Console.WriteLine($"\n用户 {a.Email} 角色名称 {a.RoleName}\n" ));

//users = fsql.Select<AspNetUsers>().Where(a => a.Email == "test@test.com").IncludeMany(a => a.AspNetUserRoless, then => then.IncludeByPropertyName(nameof(AspNetUserRoles.AspNetRoless))).ToList();
//users.ForEach(a => System.Console.WriteLine($"\n用户 {a.Email} 角色名称 {a.RoleName}\n"));

//users = fsql.Select<AspNetUsers>().Where(a => a.Email == "test@test.com").IncludeByPropertyName(nameof(AspNetUsers.AspNetUserRoless),then => then.IncludeByPropertyName(nameof(AspNetUserRoles.AspNetRoless))).ToList();
//users.ForEach(a => System.Console.WriteLine($"\n用户 {a.Email} 角色名称 {a.RoleName}\n"));

////列出Admin的用户
users = fsql.Select<AspNetUsers>().IncludeMany(a => a.AspNetUserRoless, then => then.Include(c => c.AspNetRoless).Where(d => d.AspNetRoless.Name == "Admin")).ToList();
users.ForEach(a => System.Console.WriteLine($"\n用户 {a.Email} 角色名称 {a.RoleName}\n"));

////var users3 = fsql.Select<AspNetUsers>().IncludeMany(a => a.AspNetUserRoless, then => then.Include(c => c.AspNetRoless)).Where (d=>d.AspNetUserRoless.First().RoleName == "Admin").ToSql ();

//Expression<Func<AspNetUserRoles, bool>>? where = null;

//where = where.And(d => d.AspNetRoless.Name == "Admin"); 

//var dywhere = new DynamicFilterInfo { Field = "AspNetRoless.Name", Operator = DynamicFilterOperator.Equal, Value = "Admin" };

//users = fsql.Select<AspNetUsers>().IncludeByPropertyName("AspNetUserRoless", then => then.WhereDynamicFilter(dywhere)).Where(a => a.AspNetUserRoless.Any()).ToList();

//users.ForEach(a => System.Console.WriteLine($"\n用户 {a.Email} 角色名称 {a.RoleName}\n"));
