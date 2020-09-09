using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置容器。
    /// </summary>
    public sealed class SingleTagSection : ConfigSection
    {
        private readonly XElement _content;
        private readonly SingleTagSectionPropertySet _properties;

        /// <summary>
        /// 获取配置属性集合。
        /// </summary>
        public SingleTagSectionPropertySet Properties => _properties;

        #region Constructor

        internal SingleTagSection(XElement content, ISavable savable) : base("System.Configuration.SingleTagSectionHandler")
        {
            _properties = new SingleTagSectionPropertySet(content, savable);
            _content = content;
        }

        #endregion Constructor

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is SingleTagSection other && _content.Equals(other._content);
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
        /// 方法已重写。返回节点的缩进 XML 文本。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _content.ToString();
        }
    }
}