﻿using System;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置属性。
    /// </summary>
    public sealed class NameValueSection : ConfigSection
    {
        private readonly XElement _content;
        private readonly NameValueSectionPropertySet _propertys;

        /// <summary>
        /// 包含的配置属性集合。
        /// </summary>
        public NameValueSectionPropertySet Propertys => _propertys;

        #region Constructor

        /// <summary>
        /// 创建 NameValueSection 的新实例。
        /// </summary>
        /// <param name="value">配置属性的值。</param>
        public NameValueSection(NameValueCollection value) : base("System.Configuration.NameValueSectionHandler")
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            XElement content = new XElement("newSection");
            XValueHelper.SetNameValueSection(value, content);
            _propertys = new NameValueSectionPropertySet(content, null);
            _content = content;
        }

        internal NameValueSection(XElement content, ISavable savable) : base("System.Configuration.NameValueSectionHandler")
        {
            _propertys = new NameValueSectionPropertySet(content, savable);
            _content = content;
        }

        #endregion Constructor

        public override bool Equals(object obj)
        {
            return obj is NameValueSection other && _content.Equals(other._content);
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