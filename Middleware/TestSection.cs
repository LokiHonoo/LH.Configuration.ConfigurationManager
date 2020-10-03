using LH.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Middleware
{
    /// <summary>
    /// 标准格式的 "configSections" 节点。
    /// </summary>
    public static class TestSection
    {
        public static void Create()
        {
            //
            // 使用 .NET 程序的默认配置文件。
            //
            string filePath = Assembly.GetEntryAssembly().Location + ".config";
            using (ConfigurationManager manager = new ConfigurationManager(filePath))
            {
                //
                // 支持三种标准类型的创建。
                // System.Configuration.SingleTagSectionHandler
                // System.Configuration.NameValueSectionHandler
                // System.Configuration.DictionarySectionHandler
                //
                // 直接赋值等同于 AddOrUpdate 方法。
                //
                SingleTagSection section1 = (SingleTagSection)manager.ConfigSections.Sections.GetOrAdd("section1", ConfigSectionType.SingleTagSection);
                section1.Properties.AddOrUpdate("prop1", Common.Random.NextDouble().ToString());
                section1.Properties["prop2"] = Common.Random.NextDouble().ToString();
                NameValueSection section2 = (NameValueSection)manager.ConfigSections.Sections.GetOrAdd("section2", ConfigSectionType.NameValueSection);
                section2.Properties.AddOrUpdate("prop1", Common.Random.NextDouble().ToString());
                section2.Properties["prop2"] = Common.Random.NextDouble().ToString();
                //
                ConfigSectionGroup group = manager.ConfigSections.Groups.GetOrAdd("sectionGroup1");
                DictionarySection section3 = (DictionarySection)group.Sections.GetOrAdd("section3", ConfigSectionType.DictionarySection);
                section3.Properties.AddOrUpdate("prop1", true);
                section3.Properties.AddOrUpdate("prop2", sbyte.MaxValue);
                section3.Properties.AddOrUpdate("prop3", byte.MaxValue);
                section3.Properties.AddOrUpdate("prop4", short.MaxValue);
                section3.Properties.AddOrUpdate("prop5", ushort.MaxValue);
                section3.Properties.AddOrUpdate("prop6", int.MaxValue);
                section3.Properties.AddOrUpdate("prop7", uint.MaxValue);
                section3.Properties["prop8"] = long.MaxValue;
                section3.Properties["prop9"] = ulong.MaxValue;
                section3.Properties["prop10"] = float.MaxValue / 2; // 避免浮点数溢出
                section3.Properties["prop11"] = double.MaxValue / 2; // 避免浮点数溢出
                section3.Properties["prop12"] = decimal.MaxValue;
                section3.Properties["prop13"] = (char)Common.Random.Next(65, 91);
                section3.Properties["prop14"] = new byte[] { 0x01, 0x02, 0x03, 0x0A, 0x0B, 0x0C };
                section3.Properties["prop15"] = "支持 15 种单值类型";
                //
                // 支持自定义类型的创建，需要检查是否已存在。
                //
                manager.ConfigSections.Sections.Remove("section4");
                manager.ConfigSections.Sections.AddCustumSection("section4", "This is a custom section.", "<arbitrarily>任意文本内容或 XML 内容</arbitrarily><arbitrarily>任意文本内容或 XML 内容</arbitrarily>");
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
                if (manager.ConfigSections.Sections.TryGetValue("section1", out SingleTagSection section1))
                {
                    foreach (KeyValuePair<string, string> prop in section1.Properties)
                    {
                        result.AppendLine(prop.Value);
                    }
                }
                if (manager.ConfigSections.Sections.TryGetValue("section2", out NameValueSection section2))
                {
                    foreach (KeyValuePair<string, string> prop in section2.Properties)
                    {
                        result.AppendLine(prop.Value);
                    }
                }
                if (manager.ConfigSections.Groups.TryGetValue("sectionGroup1", out ConfigSectionGroup group))
                {
                    if (group.Sections.TryGetValue("section3", out DictionarySection section3))
                    {
                        // 根据 type 参数返回强类型值。如果没有 type 参数，以 string 类型处理。
                        foreach (KeyValuePair<string, object> prop in section3.Properties)
                        {
                            result.AppendLine($"{prop.Value.GetType().Name,-10}{prop.Value}");
                        }
                    }
                }
                //
                // 如果是自定义格式，可取出 xml 文本处理。
                //
                if (manager.ConfigSections.Sections.TryGetValue("section4", out CustumSection section4))
                {
                    result.AppendLine(section4.TypeName);
                    result.AppendLine(section4.XmlString);
                }
            }
            return result.ToString();
        }
    }
}