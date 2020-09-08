using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 映射到标准格式的 "connectionStrings" 节点。
    /// </summary>
    public sealed class ConnectionStrings
    {
        private readonly XElement _content;
        private readonly ConnectionStringsPropertySet _properties;

        /// <summary>
        /// 包含的连接属性集合。
        /// </summary>
        public ConnectionStringsPropertySet Properties => _properties;

        #region Constructor

        internal ConnectionStrings(XElement root, ISavable savable)
        {
            _content = root.Element("connectionStrings");
            if (_content is null)
            {
                _content = new XElement("connectionStrings");
                root.Add(_content);
            }
            _properties = new ConnectionStringsPropertySet(_content, savable);
        }

        #endregion Constructor

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is ConnectionStrings other && _content.Equals(other._content);
        }

        /// <summary>
        /// 作为默认哈希函数。
        /// </summary>
        /// <returns></returns>
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