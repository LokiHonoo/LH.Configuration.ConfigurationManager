using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 映射到标准格式的 "configSections" 节点。
    /// </summary>
    public sealed class ConfigSections
    {
        private readonly XElement _content;
        private readonly ConfigSectionGroupSet _groups;
        private readonly ConfigSectionSet _sections;

        /// <summary>
        /// 包含的配置组集合。
        /// </summary>
        public ConfigSectionGroupSet Groups => _groups;

        /// <summary>
        /// 包含的配置容器集合。
        /// </summary>
        public ConfigSectionSet Sections => _sections;

        #region Constructor

        internal ConfigSections(XElement root, ISavable savable)
        {
            _content = root.Element("configSections");
            if (_content is null)
            {
                _content = new XElement("configSections");
                root.AddFirst(_content);
            }
            _groups = new ConfigSectionGroupSet(_content, root, savable);
            _sections = new ConfigSectionSet(_content, root, savable);
        }

        #endregion Constructor

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is ConfigSections other && _content.Equals(other._content);
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