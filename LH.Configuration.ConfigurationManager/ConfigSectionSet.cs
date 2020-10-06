using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置容器集合。
    /// </summary>
    public sealed class ConfigSectionSet : IEnumerable<KeyValuePair<string, IConfigSection>>, IEnumerable
    {
        private readonly IDictionary<string, XElement> _contents = new Dictionary<string, XElement>();
        private readonly XElement _contentSuperior;
        private readonly IDictionary<string, XElement> _declarations = new Dictionary<string, XElement>();
        private readonly XElement _declarationSuperior;
        private readonly ISavable _savable;
        private readonly IDictionary<string, IConfigSection> _values = new Dictionary<string, IConfigSection>();

        /// <summary>
        /// 获取配置容器集合中包含的元素数。
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// 获取配置容器集合的名称的集合。
        /// </summary>
        public ICollection<string> Names => _values.Keys;

        /// <summary>
        /// 获取配置容器集合。
        /// </summary>
        public ICollection<IConfigSection> Values => _values.Values;

        /// <summary>
        /// 获取具有指定名称的配置容器的值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public IConfigSection this[string name] => _values.ContainsKey(name) ? _values[name] : null;

        #region Constructor

        internal ConfigSectionSet(XElement declarationSuperior, XElement contentSuperior, ISavable savable)
        {
            _declarationSuperior = declarationSuperior;
            _contentSuperior = contentSuperior;
            _savable = savable;
            if (declarationSuperior.HasElements)
            {
                foreach (XElement declaration in declarationSuperior.Elements("section"))
                {
                    string name = declaration.Attribute("name").Value;
                    string typeName = declaration.Attribute("type").Value;
                    XElement content = contentSuperior.Element(name);
                    IConfigSection value;
                    switch (typeName)
                    {
                        case "System.Configuration.DictionarySectionHandler, System":
                        case "System.Configuration.DictionarySectionHandler":
                            value = new DictionarySection(content, savable);
                            break;

                        case "System.Configuration.NameValueSectionHandler, System":
                        case "System.Configuration.NameValueSectionHandler":
                            value = new NameValueSection(content, savable);
                            break;

                        case "System.Configuration.SingleTagSectionHandler, System":
                        case "System.Configuration.SingleTagSectionHandler":
                            value = new SingleTagSection(content, savable);
                            break;

                        default: value = new CustumSection(declaration, content, savable); break;
                    }
                    _values.Add(name, value);
                    _contents.Add(name, content);
                    _declarations.Add(name, declaration);
                }
            }
        }

        #endregion Constructor

        /// <summary>
        /// 添加一个自定义配置容器并返回值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="typeName">配置容器的类型。</param>
        /// <param name="xmlString">配置容器的串联内容。</param>
        /// <exception cref="Exception"/>
        public CustumSection AddCustumSection(string name, string typeName, string xmlString)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (name.Contains(" "))
            {
                throw new ArgumentException(ExceptionMessages.InvalidKey.Message + " - " + nameof(name));
            }
            if (typeName is null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }
            if (xmlString is null)
            {
                throw new ArgumentNullException(nameof(xmlString));
            }
            if (_values.ContainsKey(name))
            {
                throw new ArgumentException(ExceptionMessages.DuplicateKey.Message + " - " + nameof(name));
            }
            else
            {
                XElement declaration = new XElement("section");
                declaration.SetAttributeValue("name", name);
                declaration.SetAttributeValue("type", typeName);
                XElement content = XElement.Parse($"<{name}>{xmlString}</{name}>");
                CustumSection value = new CustumSection(declaration, content, _savable);
                _values.Add(name, value);
                _contents.Add(name, content);
                _contentSuperior.Add(content);
                _declarations.Add(name, declaration);
                _declarationSuperior.Add(declaration);
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
                return value;
            }
        }

        /// <summary>
        /// 从配置容器集合中移除所有配置容器。
        /// </summary>
        public void Clear()
        {
            _values.Clear();
            _contents.Clear();
            _contentSuperior.RemoveNodes();
            _declarations.Clear();
            _declarationSuperior.RemoveNodes();
            if (_savable.AutoSave)
            {
                _savable.Save();
            }
        }

        /// <summary>
        /// 确定配置容器集合是否包含带有指定名称的配置容器。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool ContainsName(string name)
        {
            return _values.ContainsKey(name);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, IConfigSection>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// 获取与指定名称关联的配置容器的值。如果不存在，添加一个配置容器并返回值。不能使用此方法获取或添加 CustumSection 类型。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="type">配置容器的类型。</param>
        /// <exception cref="Exception"/>
        public IConfigSection GetOrAdd(string name, ConfigSectionType type)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (name.Contains(" "))
            {
                throw new ArgumentException(ExceptionMessages.InvalidKey.Message + " - " + nameof(name));
            }
            if (_values.ContainsKey(name))
            {
                return _values[name];
            }
            else
            {
                XElement declaration = new XElement("section");
                declaration.SetAttributeValue("name", name);
                XElement content = new XElement(name);
                IConfigSection value;
                switch (type)
                {
                    case ConfigSectionType.DictionarySection:
                        declaration.SetAttributeValue("type", "System.Configuration.DictionarySectionHandler");
                        value = new DictionarySection(content, _savable);
                        break;

                    case ConfigSectionType.NameValueSection:
                        declaration.SetAttributeValue("type", "System.Configuration.NameValueSectionHandler");
                        value = new NameValueSection(content, _savable);
                        break;

                    case ConfigSectionType.SingleTagSection:
                        declaration.SetAttributeValue("type", "System.Configuration.SingleTagSectionHandler");
                        value = new SingleTagSection(content, _savable);
                        break;

                    default: throw new ArgumentException(ExceptionMessages.InvalidType.Message + " - " + nameof(type));
                }
                _values.Add(name, value);
                _contents.Add(name, content);
                _contentSuperior.Add(content);
                _declarations.Add(name, declaration);
                _declarationSuperior.Add(declaration);
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
                return value;
            }
        }

        /// <summary>
        /// 从配置容器集合中移除带有指定名称的配置容器。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(string name)
        {
            if (_values.Remove(name))
            {
                _contents[name].Remove();
                _contents.Remove(name);
                _declarations[name].Remove();
                _declarations.Remove(name);
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取与指定名称关联的配置容器的值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="value">配置容器的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out CustumSection value)
        {
            if (_values.TryGetValue(name, out IConfigSection val))
            {
                value = (CustumSection)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定名称关联的配置容器的值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="value">配置容器的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out DictionarySection value)
        {
            if (_values.TryGetValue(name, out IConfigSection val))
            {
                value = (DictionarySection)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定名称关联的配置容器的值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="value">配置容器的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out NameValueSection value)
        {
            if (_values.TryGetValue(name, out IConfigSection val))
            {
                value = (NameValueSection)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定名称关联的配置容器的值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="value">配置容器的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out SingleTagSection value)
        {
            if (_values.TryGetValue(name, out IConfigSection val))
            {
                value = (SingleTagSection)val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定名称关联的配置容器的值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="value">配置容器的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out IConfigSection value)
        {
            return _values.TryGetValue(name, out value);
        }
    }
}