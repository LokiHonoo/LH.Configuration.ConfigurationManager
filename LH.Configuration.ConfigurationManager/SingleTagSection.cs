using System;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置属性。
    /// </summary>
    public sealed class SingleTagSection : ConfigSection
    {
        private readonly XElement _content;
        private readonly SingleTagSectionPropertySet _propertys;

        /// <summary>
        /// 包含的配置属性集合。
        /// </summary>
        public SingleTagSectionPropertySet Propertys => _propertys;

        #region Constructor

        /// <summary>
        /// 创建 SingleTagSection 的新实例。
        /// </summary>
        /// <param name="value">配置属性的值。</param>
        public SingleTagSection(StringDictionary value) : base("System.Configuration.SingleTagSectionHandler")
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            XElement content = new XElement("newSection");
            XValueHelper.SetSingleTagSection(value, content);
            _propertys = new SingleTagSectionPropertySet(content, null);
            _content = content;
        }

        internal SingleTagSection(XElement content, ISavable savable) : base("System.Configuration.SingleTagSectionHandler")
        {
            _propertys = new SingleTagSectionPropertySet(content, savable);
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
        /// 方法已重写。返回节点的 XML 文本。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _content.ToString();
        }
    }
}