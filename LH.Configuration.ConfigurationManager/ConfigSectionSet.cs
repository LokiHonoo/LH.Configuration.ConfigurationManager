using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置属性集合。
    /// </summary>
    public sealed class ConfigSectionSet : IEnumerable
    {
        private readonly IDictionary<string, XElement> _contents = new Dictionary<string, XElement>();
        private readonly XElement _contentSuperior;
        private readonly IDictionary<string, XElement> _declarations = new Dictionary<string, XElement>();
        private readonly XElement _declarationSuperior;
        private readonly ISavable _savable;
        private readonly IDictionary<string, ConfigSection> _values = new Dictionary<string, ConfigSection>();

        /// <summary>
        /// 获取配置属性集合中包含的元素数。
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// 获取配置属性集合的名称的集合。
        /// </summary>
        public ICollection<string> Names => _values.Keys;

        /// <summary>
        /// 获取配置属性集合的值的集合。
        /// </summary>
        public ICollection<ConfigSection> Values => _values.Values;

        /// <summary>
        /// 获取或设置具有指定名称的配置属性的值。直接赋值等同于 AddOrUpdate 方法。添加为值的副本。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
        /// <returns></returns>
        public ConfigSection this[string name]
        {
            get => _values.ContainsKey(name) ? _values[name] : null;
            set { AddOrUpdate(name, value); }
        }

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
        /// 添加或更新一个配置属性。添加为配置属性的副本。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
        /// <param name="value">配置属性的值。</param>
        /// <exception cref="Exception"/>
        public void AddOrUpdate(string name, ConfigSection value)
        {
            if (value is null)
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
                }
            }
            else
            {
                switch (value.TypeName)
                {
                    case "System.Configuration.DictionarySectionHandler, System":
                    case "System.Configuration.DictionarySectionHandler":
                        AddOrUpdate(name, ((DictionarySection)value).Properties.GetInternalValues());
                        break;

                    case "System.Configuration.NameValueSectionHandler, System":
                    case "System.Configuration.NameValueSectionHandler":
                        AddOrUpdateNameValueSection(name, ((NameValueSection)value).Properties.GetInternalValues());
                        break;

                    case "System.Configuration.SingleTagSectionHandler, System":
                    case "System.Configuration.SingleTagSectionHandler":
                        AddOrUpdateSingleTagSection(name, ((SingleTagSection)value).Properties.GetInternalValues());
                        break;

                    default: throw new TypeLoadException(value.TypeName);
                }
            }
        }

        /// <summary>
        /// 添加或更新一个配置属性。添加为值的副本。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
        /// <param name="value">配置属性的值。</param>
        public void AddOrUpdate(string name, IDictionary<string, object> value)
        {
            if (value is null)
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
                }
            }
            else
            {
                if (_values.ContainsKey(name))
                {
                    XElement declaration = _declarations[name];
                    string typeName = "System.Configuration.DictionarySectionHandler";
                    declaration.SetAttributeValue("type", typeName);
                    XElement content = _contents[name];
                    XValueHelper.SetDictionarySection(value, content);
                    _values[name] = new DictionarySection(content, _savable);
                }
                else
                {
                    XElement declaration = new XElement("section");
                    declaration.SetAttributeValue("name", name);
                    string typeName = "System.Configuration.DictionarySectionHandler";
                    declaration.SetAttributeValue("type", typeName);
                    XElement content = new XElement(name);
                    XValueHelper.SetDictionarySection(value, content);
                    _values.Add(name, new DictionarySection(content, _savable));
                    _contents.Add(name, content);
                    _contentSuperior.Add(content);
                    _declarations.Add(name, declaration);
                    _declarationSuperior.Add(declaration);
                }
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
            }
        }

        /// <summary>
        /// 添加或更新一个配置属性。添加为值的副本。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
        /// <param name="value">配置属性的值。</param>
        public void AddOrUpdate(string name, NameValueCollection value)
        {
            if (value is null)
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
                }
            }
            else
            {
                if (_values.ContainsKey(name))
                {
                    XElement declaration = _declarations[name];
                    string typeName = "System.Configuration.NameValueSectionHandler";
                    declaration.SetAttributeValue("type", typeName);
                    XElement content = _contents[name];
                    XValueHelper.SetNameValueSection(value, content);
                    _values[name] = new NameValueSection(content, _savable);
                }
                else
                {
                    XElement declaration = new XElement("section");
                    declaration.SetAttributeValue("name", name);
                    string typeName = "System.Configuration.NameValueSectionHandler";
                    declaration.SetAttributeValue("type", typeName);
                    XElement content = new XElement(name);
                    XValueHelper.SetNameValueSection(value, content);
                    _values.Add(name, new NameValueSection(content, _savable));
                    _contents.Add(name, content);
                    _contentSuperior.Add(content);
                    _declarations.Add(name, declaration);
                    _declarationSuperior.Add(declaration);
                }
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
            }
        }

        /// <summary>
        /// 添加或更新一个配置属性。添加为值的副本。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
        /// <param name="value">配置属性的值。</param>
        public void AddOrUpdate(string name, StringDictionary value)
        {
            if (value is null)
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
                }
            }
            else
            {
                if (_values.ContainsKey(name))
                {
                    XElement declaration = _declarations[name];
                    string typeName = "System.Configuration.SingleTagSectionHandler";
                    declaration.SetAttributeValue("type", typeName);
                    XElement content = _contents[name];
                    XValueHelper.SetSingleTagSection(value, content);
                    _values[name] = new SingleTagSection(content, _savable);
                }
                else
                {
                    XElement declaration = new XElement("section");
                    declaration.SetAttributeValue("name", name);
                    string typeName = "System.Configuration.SingleTagSectionHandler";
                    declaration.SetAttributeValue("type", typeName);
                    XElement content = new XElement(name);
                    XValueHelper.SetSingleTagSection(value, content);
                    _values.Add(name, new SingleTagSection(content, _savable));
                    _contents.Add(name, content);
                    _contentSuperior.Add(content);
                    _declarations.Add(name, declaration);
                    _declarationSuperior.Add(declaration);
                }
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
            }
        }

        /// <summary>
        /// 从配置属性集合中移除所有配置属性。
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
        /// 确定配置属性集合是否包含带有指定名称的配置属性。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
        /// <returns></returns>
        public bool ContainsName(string name)
        {
            return _values.ContainsKey(name);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigSection> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        /// <summary>
        /// 从配置属性集合中移除带有指定名称的配置属性。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
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
        /// 获取与指定名称关联的配置属性的值。
        /// </summary>
        /// <param name="name">配置属性的名称。</param>
        /// <param name="value">配置属性的值。</param>
        /// <returns></returns>
        public bool TryGetValue(string name, out ConfigSection value)
        {
            return _values.TryGetValue(name, out value);
        }

        private void AddOrUpdateNameValueSection(string name, IDictionary<string, string> value)
        {
            if (_values.ContainsKey(name))
            {
                XElement declaration = _declarations[name];
                string typeName = "System.Configuration.NameValueSectionHandler";
                declaration.SetAttributeValue("type", typeName);
                XElement content = _contents[name];
                XValueHelper.SetNameValueSection(value, content);
                _values[name] = new NameValueSection(content, _savable);
            }
            else
            {
                XElement declaration = new XElement("section");
                declaration.SetAttributeValue("name", name);
                string typeName = "System.Configuration.NameValueSectionHandler";
                declaration.SetAttributeValue("type", typeName);
                XElement content = new XElement(name);
                XValueHelper.SetNameValueSection(value, content);
                _values.Add(name, new NameValueSection(content, _savable));
                _contents.Add(name, content);
                _contentSuperior.Add(content);
                _declarations.Add(name, declaration);
                _declarationSuperior.Add(declaration);
            }
            if (_savable.AutoSave)
            {
                _savable.Save();
            }
        }

        private void AddOrUpdateSingleTagSection(string name, IDictionary<string, string> value)
        {
            if (_values.ContainsKey(name))
            {
                XElement declaration = _declarations[name];
                string typeName = "System.Configuration.SingleTagSectionHandler";
                declaration.SetAttributeValue("type", typeName);
                XElement content = _contents[name];
                XValueHelper.SetSingleTagSection(value, content);
                _values[name] = new SingleTagSection(content, _savable);
            }
            else
            {
                XElement declaration = new XElement("section");
                declaration.SetAttributeValue("name", name);
                string typeName = "System.Configuration.SingleTagSectionHandler";
                declaration.SetAttributeValue("type", typeName);
                XElement content = new XElement(name);
                XValueHelper.SetSingleTagSection(value, content);
                _values.Add(name, new SingleTagSection(content, _savable));
                _contents.Add(name, content);
                _contentSuperior.Add(content);
                _declarations.Add(name, declaration);
                _declarationSuperior.Add(declaration);
            }
            if (_savable.AutoSave)
            {
                _savable.Save();
            }
        }
    }
}