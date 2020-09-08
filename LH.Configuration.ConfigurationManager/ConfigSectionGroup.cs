using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置容器。
    /// </summary>
    public sealed class ConfigSectionGroup
    {
        private readonly XElement _content;
        private readonly ConfigSectionGroupSet _groups;
        private readonly ConfigSectionSet _sections;

        /// <summary>
        /// 包含的配置容器集合。
        /// </summary>
        public ConfigSectionGroupSet Groups => _groups;

        /// <summary>
        /// 包含的配置属性集合。
        /// </summary>
        public ConfigSectionSet Sections => _sections;

        #region Constructor

        internal ConfigSectionGroup(XElement declaration, XElement content, ISavable savable)
        {
            _content = content;
            _groups = new ConfigSectionGroupSet(declaration, content, savable);
            _sections = new ConfigSectionSet(declaration, content, savable);
        }

        #endregion Constructor

        public override bool Equals(object obj)
        {
            return obj is ConfigSectionGroup other && _content.Equals(other._content);
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