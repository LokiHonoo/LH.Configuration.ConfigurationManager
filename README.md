# LH.Configuration.ConfigurationManager

<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [LH.Configuration.ConfigurationManager](#lhconfigurationconfigurationmanager)
  - [简介](#简介)
  - [快速上手](#快速上手)
    - [添加 NuGet 包](#添加-nuget-包)
    - [引用命名空间](#引用命名空间)
    - [appSettings](#appsettings)
    - [connectionStrings](#connectionstrings)
    - [sectionGroup/section](#sectiongroupsection)
    - [自动保存](#自动保存)
    - [在 UWP 项目中使用](#在-uwp-项目中使用)
  - [版权](#版权)

<!-- /code_chunk_output -->

## 简介

LH.Configuration.ConfigurationManager 是 System.Configuration.ConfigurationManager 的简单替代。

LH.Configuration.ConfigurationManager 开发的主要目的是在 NET Framework 4.0 中读写配置文件。

提供对 appSettings、connectionStrings、configSections 节点的有限读写支持。

## 快速上手

### 添加 NuGet 包

```commandline
PM> Install-Package LH.Configuration.ConfigurationManager
```

### 引用命名空间

```c#
using LH.Configuration;
```

### appSettings

```c#
public static void Create()
{
    //
    // 使用 .NET 程序的默认配置文件
    //
    using (ConfigurationManager manager = new ConfigurationManager(Assembly.GetEntryAssembly().Location + ".config"))
    {
        //
        // 直接赋值等同于 AddOrUpdate 方法
        //
        manager.AppSettings.Propertys.AddOrUpdate("prop1", Common.Random.NextDouble().ToString());
        manager.AppSettings.Propertys["prop2"] = Common.Random.NextDouble().ToString();
        manager.AppSettings.Propertys["prop3"] = "等待移除";
        //
        // 移除属性的方法
        //
        manager.AppSettings.Propertys.AddOrUpdate("prop3", null);
        manager.AppSettings.Propertys["prop3"] = null;
        manager.AppSettings.Propertys.Remove("prop3");
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
    using (ConfigurationManager manager = new ConfigurationManager(Assembly.GetEntryAssembly().Location + ".config"))
    {
        //
        // 取出属性
        //
        if (manager.AppSettings.Propertys.TryGetValue("prop1", out string value))
        {
            result.AppendLine(value);
        }
        value = manager.AppSettings.Propertys["prop2"];
        result.AppendLine(value);
    }
    return result.ToString();
}
```

### connectionStrings

```c#

public static void Create()
{
    //
    // 使用 .NET 程序的默认配置文件
    //
    using (ConfigurationManager manager = new ConfigurationManager(Assembly.GetEntryAssembly().Location + ".config"))
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
        // 直接赋值等同于 AddOrUpdate 方法
        //
        manager.ConnectionStrings.Propertys.AddOrUpdate("prop1", conn1);
        manager.ConnectionStrings.Propertys["prop2"] = new ConnectionStringsValue(conn1);
        manager.ConnectionStrings.Propertys.AddOrUpdate("prop3", conn2.ConnectionString, typeof(MySqlConnection).Namespace);
        //
        // 不设置引擎参数，读取时不能直接创建连接实例
        //
        manager.ConnectionStrings.Propertys["prop4"] = new ConnectionStringsValue(conn2.ConnectionString, string.Empty);
        //
        // 移除属性的方法。
        //
        manager.ConnectionStrings.Propertys.AddOrUpdate("prop4", (DbConnection)null);
        manager.ConnectionStrings.Propertys["prop4"] = null;
        manager.ConnectionStrings.Propertys.Remove("prop4");
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
    using (ConfigurationManager manager = new ConfigurationManager(Assembly.GetEntryAssembly().Location + ".config"))
    {
        //
        // 取出属性
        //
        if (manager.ConnectionStrings.Propertys.TryGetValue("prop1", out ConnectionStringsValue property))
        {
            result.AppendLine(property.Connection.ConnectionString);
        }
        DbConnection connection = manager.ConnectionStrings.Propertys["prop2"].Connection;
        result.AppendLine(connection.ConnectionString);
        //
        // 不访问 Connection，属性内部没有实例化 Connection。项目没有引用相关数据库引擎时使用。
        //
        string connectionString = manager.ConnectionStrings.Propertys["prop3"].ConnectionString;
        result.AppendLine(connectionString);
    }
    return result.ToString();
}

```

### sectionGroup/section

```c#

public static void Create()
{
    //
    // 使用 .NET 程序的默认配置文件
    //
    using (ConfigurationManager manager = new ConfigurationManager(Assembly.GetEntryAssembly().Location + ".config"))
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
        manager.ConfigSections.Sections.AddOrUpdate("section1", ConfigSection.CreateSingleTagSection(props1));
        manager.ConfigSections.Sections.AddOrUpdate("section2", props2);
        ConfigSectionGroup group = manager.ConfigSections.Groups.GetOrAdd("sectionGroup1");
        group.Sections.AddOrUpdate("section3", props3);
        manager.ConfigSections.Sections["section4"] = ConfigSection.CreateSingleTagSection(props1);
        //
        // 可修改集合
        //
        DictionarySectionPropertySet props = ((DictionarySection)group.Sections["section3"]).Propertys;
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
    using (ConfigurationManager manager = new ConfigurationManager(Assembly.GetEntryAssembly().Location + ".config"))
    {
        //
        // 取出属性
        //
        if (manager.ConfigSections.Sections.TryGetValue("section1", out ConfigSection section))
        {
            foreach (KeyValuePair<string, string> prop in ((SingleTagSection)section).Propertys)
            {
                result.AppendLine(prop.Value);
            }
        }
        if (manager.ConfigSections.Sections.TryGetValue("section2", out section))
        {
            foreach (KeyValuePair<string, string> prop in ((NameValueSection)section).Propertys)
            {
                result.AppendLine(prop.Value);
            }
        }
        if (manager.ConfigSections.Groups.TryGetValue("sectionGroup1", out ConfigSectionGroup group))
        {
            if (group.Sections.TryGetValue("section3", out section))
            {
                // 根据 type 参数返回强类型值。如果没有 type 参数，以 string 类型处理。
                foreach (KeyValuePair<string, object> prop in ((DictionarySection)section).Propertys)
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

```

### 自动保存

开启自动保存。默认是 false。

```c#
manager.AutoSave = true;
```

### 在 UWP 项目中使用

必须使用流方式。
不支持自动保存。

UWP 项目推荐使用内置的 ApplicationData 配置方案。

```c#

public static async void Test()
{
    using (var read = await storageFile.OpenStreamForReadAsync())
    {
        using (ConfigurationManager manager = new ConfigurationManager(read)
        {
            using (var write = await storageFile.OpenStreamForWriteAsync())
            {
                manager.Save(write);
            }
        }
    }
}

```

## 版权

LH.Configuration.ConfigurationManager 的开发和发布基于 MIT 协议。