using System;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置容器。
    /// </summary>
    public sealed class CustumSection : IConfigSection
    {
        private readonly XElement _content;
        private readonly XElement _declaration;
        private readonly ISavable _savable;

        /// <summary>
        /// 获取配置容器的类型的文本表示。
        /// </summary>
        public string TypeName => _declaration.Attribute("type").Value;

        /// <summary>
        /// 获取配置容器的串联内容。节点缩进已经移除。
        /// </summary>
        public string XmlString => _content.ToString(SaveOptions.DisableFormatting)
            .Replace($"<{_content.Name}>", string.Empty)
            .Replace($"</{_content.Name}>", string.Empty)
            .Replace($"<{_content.Name} />", string.Empty);

        #region Constructor

        internal CustumSection(XElement declaration, XElement content, ISavable savable)
        {
            _declaration = declaration;
            _content = content;
            _savable = savable;
        }

        #endregion Constructor

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is CustumSection other && _declaration.Equals(other._declaration) && _content.Equals(other._content);
        }

        /// <summary>
        /// 作为默认哈希函数。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _declaration.GetHashCode() ^ _content.GetHashCode();
        }

        /// <summary>
        /// 修改内容。内容不能是 null。
        /// </summary>
        /// <param name="typeName">配置容器的类型。</param>
        /// <param name="xmlString">配置容器的串联内容。</param>
        /// <exception cref="Exception"/>
        public void Modify(string typeName, string xmlString)
        {
            if (typeName is null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }
            if (xmlString is null)
            {
                throw new ArgumentNullException(nameof(xmlString));
            }
            _declaration.SetAttributeValue("type", typeName);
            bool converted = false;
            if (xmlString.Trim().StartsWith("<", StringComparison.InvariantCulture))
            {
                try
                {
                    XElement tmp = XElement.Parse($"<tmp>{xmlString}</tmp>");
                    if (tmp.HasElements)
                    {
                        _content.ReplaceAll(tmp.Elements());
                        converted = true;
                    }
                }
                catch { }
            }
            if (!converted)
            {
                _content.SetValue(xmlString);
            }
            if (_savable.AutoSave)
            {
                _savable.Save();
            }
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