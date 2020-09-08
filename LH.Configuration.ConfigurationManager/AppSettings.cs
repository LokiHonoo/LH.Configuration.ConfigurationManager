using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 映射到标准格式的 "appSettings" 节点。
    /// </summary>
    public sealed class AppSettings
    {
        private readonly XElement _content;
        private readonly AppSettingsPropertySet _propertys;

        /// <summary>
        /// 包含的配置数据属性集合。
        /// </summary>
        public AppSettingsPropertySet Propertys => _propertys;

        #region Constructor

        internal AppSettings(XElement root, ISavable savable)
        {
            _content = root.Element("appSettings");
            if (_content is null)
            {
                _content = new XElement("appSettings");
                root.Add(_content);
            }
            _propertys = new AppSettingsPropertySet(_content, savable);
        }

        #endregion Constructor

        public override bool Equals(object obj)
        {
            return obj is AppSettings other && _content.Equals(other._content);
        }

        public override int GetHashCode()
        {
            return _content.GetHashCode();
        }

        /// <summary>
        /// 方法已重写。返回节点的 XML 文本。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _content.ToString();
        }
    }
}