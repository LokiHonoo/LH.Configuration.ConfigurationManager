using LH.Configuration;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Middleware
{
    /// <summary>
    /// 标准格式的 "connectionStrings" 节点。
    /// </summary>
    public static class TestConnectionStrings
    {
        public static void Create()
        {
            //
            // 使用 .NET 程序的默认配置文件。
            //
            string filePath = Assembly.GetEntryAssembly().Location + ".config";
            using (ConfigurationManager manager = new ConfigurationManager(filePath))
            {
                SqlConnectionStringBuilder builder1 = new SqlConnectionStringBuilder()
                {
                    DataSource = "127.0.0.1",
                    InitialCatalog = "DemoCatalog",
                    UserID = "sa",
                    Password = Common.Random.Next(1000000, 9999999).ToString()
                };
                SqlConnection conn1 = new SqlConnection(builder1.ConnectionString);
                MySqlConnectionStringBuilder builder2 = new MySqlConnectionStringBuilder()
                {
                    Server = "127.0.0.1",
                    Database = "DemoDB",
                    UserID = "root",
                    Password = Common.Random.Next(1000000, 9999999).ToString()
                };
                MySqlConnection conn2 = new MySqlConnection(builder2.ConnectionString);
                //
                // 直接赋值等同于 AddOrUpdate 方法。
                //
                manager.ConnectionStrings.Properties.AddOrUpdate("prop1", conn1);
                manager.ConnectionStrings.Properties["prop2"] = new ConnectionStringsValue(conn1);
                manager.ConnectionStrings.Properties.AddOrUpdate("prop3", conn2.ConnectionString, typeof(MySqlConnection).Namespace);
                //
                // 不设置引擎参数，读取时不能直接创建连接实例。
                //
                manager.ConnectionStrings.Properties["prop4"] = new ConnectionStringsValue(conn2.ConnectionString, string.Empty);
                //
                // 移除属性的方法。选择其一。
                //
                manager.ConnectionStrings.Properties.AddOrUpdate("prop4", (DbConnection)null);
                manager.ConnectionStrings.Properties["prop4"] = null;
                manager.ConnectionStrings.Properties.Remove("prop4");
                //
                // 保存到创建实例时指定的文件。
                //
                manager.Save();
            }
        }

        public static string Load()
        {
            StringBuilder result = new StringBuilder();
            //
            // 使用 .NET 程序的默认配置文件。
            //
            string filePath = Assembly.GetEntryAssembly().Location + ".config";
            using (ConfigurationManager manager = new ConfigurationManager(filePath))
            {
                //
                // 取出属性。
                //
                if (manager.ConnectionStrings.Properties.TryGetValue("prop1", out ConnectionStringsValue property))
                {
                    result.AppendLine(property.Connection.ConnectionString);
                }
                DbConnection connection = manager.ConnectionStrings.Properties["prop2"].Connection;
                result.AppendLine(connection.ConnectionString);
                //
                // 不访问 Connection，属性内部没有实例化 Connection。项目没有引用相关数据库引擎时使用。
                //
                string connectionString = manager.ConnectionStrings.Properties["prop3"].ConnectionString;
                result.AppendLine(connectionString);
            }
            return result.ToString();
        }
    }
}