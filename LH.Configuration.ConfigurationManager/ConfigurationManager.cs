using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置管理器。
    /// </summary>
    public sealed class ConfigurationManager : IDisposable, ISavable
    {
        private readonly string _filePath;
        private AppSettings _appSettings;
        private bool _autoSave;
        private ConfigSections _configSections;
        private ConnectionStrings _connectionStrings;
        private bool _disposed;
        private XElement _root;

        /// <summary>
        /// 映射到标准格式的 "appSettings" 节点。
        /// </summary>
        public AppSettings AppSettings
        {
            get
            {
                if (_appSettings is null)
                {
                    _appSettings = new AppSettings(_root, this);
                }
                return _appSettings;
            }
        }

        /// <summary>
        /// 获取或设置自动保存选项。如果在创建 <see cref="ConfigurationManager"/> 实例时没有指定文件路径，此选项无效。默认值是 false。
        /// </summary>
        public bool AutoSave { get => _autoSave; set => _autoSave = value; }

        /// <summary>
        /// 映射到标准格式的 "configSections" 节点。忽略 extensionSettings 描述。
        /// </summary>
        public ConfigSections ConfigSections
        {
            get
            {
                if (_configSections is null)
                {
                    _configSections = new ConfigSections(_root, this);
                }
                return _configSections;
            }
        }

        /// <summary>
        /// 映射到标准格式的 "connectionStrings" 节点。
        /// </summary>
        public ConnectionStrings ConnectionStrings
        {
            get
            {
                if (_connectionStrings is null)
                {
                    _connectionStrings = new ConnectionStrings(_root, this);
                }
                return _connectionStrings;
            }
        }

        /// <summary>
        /// 获取映射的文件路径。
        /// </summary>
        public string FilePath => _filePath;

        #region Constructor

        /// <summary>
        /// 创建 ConfigurationManager 的新实例。
        /// </summary>
        public ConfigurationManager()
        {
            _root = new XElement("configuration");
            _filePath = string.Empty;
        }

        /// <summary>
        /// 创建 ConfigurationManager 的新实例。
        /// </summary>
        /// <param name="filePath">指定配置文件的源流，从中读取配置。</param>
        public ConfigurationManager(Stream stream)
        {
            if (stream is null || stream.Length == 0)
            {
                _root = new XElement("configuration");
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                _root = XElement.Load(stream);
            }
            _filePath = string.Empty;
        }

        /// <summary>
        /// 创建 ConfigurationManager 的新实例。
        /// </summary>
        /// <param name="filePath">指定配置文件路径。要启用自动保存必须指定文件路径。</param>
        public ConfigurationManager(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            if (info.Exists && info.Length > 0)
            {
                _root = XElement.Load(filePath);
            }
            else
            {
                _root = new XElement("configuration");
            }
            _filePath = filePath;
        }

        ~ConfigurationManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _root = null;
                }

                _disposed = true;
            }
        }

        #endregion Constructor

        /// <summary>
        /// 创建映射到默认配置文件的 <see cref="ConfigurationManager"/> 实例。文件名形如 *.exe.config。
        /// </summary>
        public static ConfigurationManager CreateStandard()
        {
            return new ConfigurationManager(Assembly.GetEntryAssembly().Location + ".config");
        }

        public override bool Equals(object obj)
        {
            return obj is ConfigurationManager other && _root.Equals(other._root);
        }

        public override int GetHashCode()
        {
            return _root.GetHashCode();
        }

        /// <summary>
        /// 保存到创建 <see cref="ConfigurationManager"/> 实例时指定的文件。
        /// </summary>
        public void Save()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                _root.Save(_filePath);
            }
        }

        /// <summary>
        /// 保存到指定的流。
        /// </summary>
        public void Save(Stream stream)
        {
            stream.SetLength(0);
            _root.Save(stream);
        }

        /// <summary>
        /// 保存到指定的文件。
        /// </summary>
        /// <param name="filePath">文件路径。</param>
        public void Save(string filePath)
        {
            _root.Save(filePath);
        }

        /// <summary>
        /// 方法已重写。返回配置的 XML 文本。不包括文档描述。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _root.ToString();
        }
    }
}