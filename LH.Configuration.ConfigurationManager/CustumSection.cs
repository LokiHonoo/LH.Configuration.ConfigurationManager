using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置容器。
    /// </summary>
    public sealed class CustumSection : ConfigSection
    {
        private readonly string _xmlString;

        /// <summary>
        /// 获取节点的 Xml 文本。
        /// </summary>
        public string XmlString => _xmlString;

        #region Constructor

        internal CustumSection(string typeName, XElement content) : base(typeName)
        {
            _xmlString = content.ToString();
        }

        #endregion Constructor

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is CustumSection other && _xmlString.Equals(other._xmlString);
        }

        /// <summary>
        /// 作为默认哈希函数。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _xmlString.GetHashCode();
        }

        /// <summary>
        /// 方法已重写。返回节点的 XML 文本。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _xmlString;
        }
    }
}