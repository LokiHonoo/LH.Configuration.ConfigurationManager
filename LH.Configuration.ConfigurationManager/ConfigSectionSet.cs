using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置容器集合。
    /// </summary>
    public sealed class ConfigSectionSet : IEnumerable<KeyValuePair<string, ConfigSection>>, IEnumerable
    {
        private readonly IDictionary<string, XElement> _contents = new Dictionary<string, XElement>();
        private readonly XElement _contentSuperior;
        private readonly IDictionary<string, XElement> _declarations = new Dictionary<string, XElement>();
        private readonly XElement _declarationSuperior;
        private readonly ISavable _savable;
        private readonly IDictionary<string, ConfigSection> _values = new Dictionary<string, ConfigSection>();

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
        public ICollection<ConfigSection> Values => _values.Values;

        /// <summary>
        /// 获取具有指定名称的配置容器的值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <returns></returns>
        public ConfigSection this[string name] => _values.ContainsKey(name) ? _values[name] : null;

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
                    ConfigSection value;
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

                        default: value = new CustumSection(typeName, content); break;
                    }
                    _values.Add(name, value);
                    _contents.Add(name, content);
                    _declarations.Add(name, declaration);
                }
            }
        }

        #endregion Constructor

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
        public bool ContainsName(string name)
        {
            return _values.ContainsKey(name);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, ConfigSection>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// 获取与指定名称关联的配置容器的值。如果不存在，添加一个配置容器并返回值。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <param name="type">配置容器的类型。</param>
        /// <exception cref="Exception"/>
        public ConfigSection GetOrAdd(string name, ConfigSectionType type)
        {
            if (_values.TryGetValue(name, out ConfigSection value))
            {
                switch (type)
                {
                    case ConfigSectionType.DictionarySection: return (DictionarySection)value;
                    case ConfigSectionType.NameValueSection: return (NameValueSection)value;
                    case ConfigSectionType.SingleTagSection: return (SingleTagSection)value;
                    default: throw new ArgumentException(null, nameof(type));
                }
            }
            else
            {
                XElement declaration = new XElement("section");
                declaration.SetAttributeValue("name", name);
                XElement content = new XElement(name);
                ConfigSection section;
                switch (type)
                {
                    case ConfigSectionType.DictionarySection:
                        declaration.SetAttributeValue("type", "System.Configuration.DictionarySectionHandler");
                        section = new DictionarySection(content, _savable);
                        break;

                    case ConfigSectionType.NameValueSection:
                        declaration.SetAttributeValue("type", "System.Configuration.NameValueSectionHandler");
                        section = new NameValueSection(content, _savable);
                        break;

                    case ConfigSectionType.SingleTagSection:
                        declaration.SetAttributeValue("type", "System.Configuration.SingleTagSectionHandler");
                        section = new SingleTagSection(content, _savable);
                        break;

                    default: throw new ArgumentException(nameof(value));
                }
                _values.Add(name, section);
                _contents.Add(name, content);
                _contentSuperior.Add(content);
                _declarations.Add(name, declaration);
                _declarationSuperior.Add(declaration);
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
                return section;
            }
        }

        /// <summary>
        /// 从配置容器集合中移除带有指定名称的配置容器。
        /// </summary>
        /// <param name="name">配置容器的名称。</param>
        /// <returns></returns>
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
        public bool TryGetValue(string name, out CustumSection value)
        {
            if (_values.TryGetValue(name, out ConfigSection val))
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
        public bool TryGetValue(string name, out DictionarySection value)
        {
            if (_values.TryGetValue(name, out ConfigSection val))
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
        public bool TryGetValue(string name, out NameValueSection value)
        {
            if (_values.TryGetValue(name, out ConfigSection val))
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
        public bool TryGetValue(string name, out SingleTagSection value)
        {
            if (_values.TryGetValue(name, out ConfigSection val))
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
        public bool TryGetValue(string name, out ConfigSection value)
        {
            return _values.TryGetValue(name, out value);
        }
    }
}