using LH.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            // 使用 .NET 程序的默认配置文件
            //
            string filePath = Assembly.GetEntryAssembly().Location + ".config";
            using (ConfigurationManager manager = new ConfigurationManager(filePath))
            {
                //
                // 支持三种标准类型的创建
                // System.Configuration.SingleTagSectionHandler，输入值类型是 StringDictionary
                // System.Configuration.NameValueSectionHandler，输入值类型是 NameValueCollection
                // System.Configuration.DictionarySectionHandler，输入值类型是 IDictionary<string, object>
                //
                var props1 = new StringDictionary
                {
                    { "section_prop1", Common.Random.NextDouble().ToString() },
                    { "section_prop2", Common.Random.NextDouble().ToString() },
                    { "section_prop3", Common.Random.NextDouble().ToString() }
                };
                var props2 = new NameValueCollection
                {
                    { "section_prop1", Common.Random.NextDouble().ToString() },
                    { "section_prop2", Common.Random.NextDouble().ToString() },
                    { "section_prop3", Common.Random.NextDouble().ToString() }
                };
                var props3 = new Dictionary<string, object>
                {
                    { "section_prop1", true },
                    { "section_prop2", sbyte.MaxValue },
                    { "section_prop3", byte.MaxValue },
                    { "section_prop4", short.MaxValue },
                    { "section_prop5", ushort.MaxValue },
                    { "section_prop6", int.MaxValue },
                    { "section_prop7", uint.MaxValue },
                    { "section_prop8", long.MaxValue },
                    { "section_prop9", ulong.MaxValue },
                    { "section_prop10", float.MaxValue / 2 }, // 防止 NET40 浮点数精度问题导致的溢出
                    { "section_prop11", double.MaxValue / 2 }, // 防止 NET40 浮点数精度问题导致的溢出
                    { "section_prop12", decimal.MaxValue },
                    { "section_prop13", (char)Common.Random.Next(65, 91) },
                    { "section_prop14", new byte[] { 0x01, 0x01, 0x0A, 0x0B, 0x0C } },
                    { "section_prop15", "支持 15 种单值类型" }
                };
                //
                // 直接赋值等同于 AddOrUpdate 方法。添加为配置属性或集合的副本。
                //
                manager.ConfigSections.Sections.AddOrUpdate("section1", ConfigSection.Create(props1));
                manager.ConfigSections.Sections.AddOrUpdate("section2", props2);
                ConfigSectionGroup group = manager.ConfigSections.Groups.GetOrAdd("sectionGroup1");
                group.Sections.AddOrUpdate("section3", props3);
                manager.ConfigSections.Sections["section4"] = ConfigSection.Create(props1);
                //
                // 可修改集合
                //
                DictionarySectionPropertySet props = ((DictionarySection)group.Sections["section3"]).Properties;
                props.AddOrUpdate("section_prop15_1", "强类型存储");
                //
                // 移除属性的方法
                //
                manager.ConfigSections.Sections.AddOrUpdate("section4", (ConfigSection)null);
                manager.ConfigSections.Sections["section4"] = null;
                manager.ConfigSections.Sections.Remove("section4");
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
                if (manager.ConfigSections.Sections.TryGetValue("section1", out ConfigSection section))
                {
                   
                    foreach (KeyValuePair<string, string> prop in ((SingleTagSection)section).Properties)
                    {
                        result.AppendLine(prop.Value);
                    }
                }
                if (manager.ConfigSections.Sections.TryGetValue("section2", out section))
                {
                    foreach (KeyValuePair<string, string> prop in ((NameValueSection)section).Properties)
                    {
                        result.AppendLine(prop.Value);
                    }
                }
                if (manager.ConfigSections.Groups.TryGetValue("sectionGroup1", out ConfigSectionGroup group))
                {
                    if (group.Sections.TryGetValue("section3", out section))
                    {
                        // 根据 type 参数返回强类型值。如果没有 type 参数，以 string 类型处理。
                        foreach (KeyValuePair<string, object> prop in ((DictionarySection)section).Properties)
                        {
                            result.AppendLine($"{prop.Value.GetType().Name,-10}{prop.Value}");
                        }
                    }
                }
                //
                // 如果是自定义格式，可取出 xml 文本处理。
                //
                if (manager.ConfigSections.Sections.TryGetValue("section4", out section))
                {
                    var typeName = section.TypeName;
                    var xml = ((CustumSection)section).XmlString;
                }
            }
            return result.ToString();
        }
    }
}