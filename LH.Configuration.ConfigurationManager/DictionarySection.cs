using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置属性。
    /// </summary>
    public sealed class DictionarySection : ConfigSection
    {
        private readonly XElement _content;
        private readonly DictionarySectionPropertySet _propertys;

        /// <summary>
        /// 包含的配置属性集合。
        /// </summary>
        public DictionarySectionPropertySet Propertys => _propertys;

        #region Constructor

        /// <summary>
        /// 创建 DictionarySection 的新实例。
        /// </summary>
        /// <param name="value">配置属性的值。</param>
        public DictionarySection(IDictionary<string, object> value) : base("System.Configuration.DictionarySectionHandler")
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            XElement content = new XElement("newSection");
            XValueHelper.SetDictionarySection(value, content);
            _propertys = new DictionarySectionPropertySet(content, null);
            _content = content;
        }

        internal DictionarySection(XElement content, ISavable savable) : base("System.Configuration.DictionarySectionHandler")
        {
            _propertys = new DictionarySectionPropertySet(content, savable);
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
            return obj is DictionarySection other && _content.Equals(other._content);
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