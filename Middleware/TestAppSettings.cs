using LH.Configuration;
using System.Reflection;
using System.Text;

namespace Middleware
{
    /// <summary>
    /// 标准格式的 "appSettings" 节点。
    /// </summary>
    public static class TestAppSettings
    {
        public static void Create()
        {
            //
            // 使用 .NET 程序的默认配置文件
            //
            string filePath = Assembly.GetEntryAssembly().Location + ".config";
            using (ConfigurationManager manager = new ConfigurationManager(filePath))
            {
                //
                // 直接赋值等同于 AddOrUpdate 方法
                //
                manager.AppSettings.Properties.AddOrUpdate("prop1", Common.Random.NextDouble().ToString());
                manager.AppSettings.Properties["prop2"] = Common.Random.NextDouble().ToString();
                manager.AppSettings.Properties["prop3"] = "等待移除";
                //
                // 移除属性的方法
                //
                manager.AppSettings.Properties.AddOrUpdate("prop3", null);
                manager.AppSettings.Properties["prop3"] = null;
                manager.AppSettings.Properties.Remove("prop3");
                //
                // 保存到创建实例时指定的文件
                //
                manager.Save();
            }
        }

        public static string Load()
        {
            StringBuilder result = new StringBuilder();
            //
            // 使用 .NET 程序的默认配置文件
            //
            string filePath = Assembly.GetEntryAssembly().Location + ".config";
            using (ConfigurationManager manager = new ConfigurationManager(filePath))
            {
                //
                // 取出属性
                //
                if (manager.AppSettings.Properties.TryGetValue("prop1", out string value))
                {
                    result.AppendLine(value);
                }
                value = manager.AppSettings.Properties["prop2"];
                result.AppendLine(value);
            }
            return result.ToString();
        }
    }
}