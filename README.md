# LH.Configuration.ConfigurationManager

<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [LH.Configuration.ConfigurationManager](#lhconfigurationconfigurationmanager)
  - [简介](#简介)
  - [快速上手](#快速上手)
    - [NuGet](#nuget)
    - [引用命名空间](#引用命名空间)
    - [appSettings](#appsettings)
    - [connectionStrings](#connectionstrings)
    - [sectionGroup/section](#sectiongroupsection)
    - [自动保存](#自动保存)
    - [在 UWP 项目中使用](#在-uwp-项目中使用)
  - [版权](#版权)

<!-- /code_chunk_output -->

## 简介

此项目是 System.Configuration.ConfigurationManager 的简单替代。

开发用于 .NET Framework 4.0+/.NET Standard 2.0+/UWP 中读写配置文件。

提供对 appSettings、connectionStrings、configSections 节点的有限读写支持。

## 快速上手

### NuGet

<https://www.nuget.org/packages/LH.Configuration.ConfigurationManager/>

### 引用命名空间

```c#

using LH.Configuration;

```

### appSettings

```c#

public static void Create()
{
    //
    // 使用 .NET 程序的默认配置文件。
    //
    string filePath = Assembly.GetEntryAssembly().Location + ".config";
    using (ConfigurationManager manager = new ConfigurationManager(filePath))
    {
        //
        // 直接赋值等同于 AddOrUpdate 方法。
        //
        manager.AppSettings.Properties.AddOrUpdate("prop1", Common.Random.NextDouble().ToString());
        manager.AppSettings.Properties["prop2"] = Common.Random.NextDouble().ToString();
        manager.AppSettings.Properties["prop3"] = "等待移除";
        //
        // 移除属性的方法。选择其一。
        //
        manager.AppSettings.Properties.AddOrUpdate("prop3", null);
        manager.AppSettings.Properties["prop3"] = null;
        manager.AppSettings.Properties.Remove("prop3");
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
        if (manager.AppSettings.Properties.TryGetValue("prop1", out string value))
        {
            result.AppendLine(value);
        }
        value = manager.AppSettings.Properties["prop2"];
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
        manager.ConnectionStrings.Properties.AddOrUpdate("prop4", conn2.ConnectionString, typeof(MySqlConnection).AssemblyQualifiedName);
        //
        // 不设置引擎参数，读取时不能直接创建连接实例。
        //
        manager.ConnectionStrings.Properties["prop5"] = new ConnectionStringsValue(conn2.ConnectionString, string.Empty);
        //
        // 移除属性的方法。选择其一。
        //
        manager.ConnectionStrings.Properties.AddOrUpdate("prop5", (DbConnection)null);
        manager.ConnectionStrings.Properties["prop5"] = null;
        manager.ConnectionStrings.Properties.Remove("prop5");
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
        if (manager.ConnectionStrings.Properties.TryGetValue("prop1", out ConnectionStringsValue value))
        {
            result.AppendLine(value.Connection.ConnectionString);
        }
        //
        // 不访问 Connection，属性内部没有实例化 Connection。项目没有引用相关数据库引擎时使用。
        //
        string connectionString = manager.ConnectionStrings.Properties["prop2"].ConnectionString;
        result.AppendLine(connectionString);
        DbConnection connection = manager.ConnectionStrings.Properties["prop3"].Connection;
        result.AppendLine(connection.ConnectionString);

        MySqlConnection mysql = (MySqlConnection)manager.ConnectionStrings.Properties["prop4"].Connection;
        result.AppendLine(mysql.ConnectionString);
    }
    return result.ToString();
}

```

### sectionGroup/section

```c#

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
        CustumSection cs = manager.ConfigSections.Sections.AddCustumSection("section4", "This is a custom section.", "任意文本内容或 XML 内容");
        cs.Modify("This is a custom section.", "<arbitrarily>任意文本内容或 XML 内容</arbitrarily><arbitrarily>任意文本内容或 XML 内容</arbitrarily>");
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

```

### 自动保存

如果在创建 LH.Configuration.ConfigurationManager 实例时没有指定文件路径，此选项无效。默认值是 false。

```c#
manager.AutoSave = true;
```

### 在 UWP 项目中使用

必须使用流方式。

```c#

public static async void Test()
{
    using (var read = await storageFile.OpenStreamForReadAsync())
    {
        using (ConfigurationManager manager = new ConfigurationManager(read))
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
