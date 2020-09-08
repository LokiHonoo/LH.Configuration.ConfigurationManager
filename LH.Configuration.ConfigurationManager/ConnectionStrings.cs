using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 映射到标准格式的 "connectionStrings" 节点。
    /// </summary>
    public sealed class ConnectionStrings
    {
        private readonly XElement _content;
        private readonly ConnectionStringsPropertySet _propertys;

        /// <summary>
        /// 包含的配置属性集合。
        /// </summary>
        public ConnectionStringsPropertySet Propertys => _propertys;

        #region Constructor

        internal ConnectionStrings(XElement root, ISavable savable)
        {
            _content = root.Element("connectionStrings");
            if (_content is null)
            {
                _content = new XElement("connectionStrings");
                root.Add(_content);
            }
            _propertys = new ConnectionStringsPropertySet(_content, savable);
        }

        #endregion Constructor

        public override bool Equals(object obj)
        {
            return obj is ConnectionStrings other && _content.Equals(other._content);
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