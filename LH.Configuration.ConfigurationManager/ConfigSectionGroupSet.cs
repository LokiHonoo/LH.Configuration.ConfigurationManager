using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    ///  配置组集合。
    /// </summary>
    public sealed class ConfigSectionGroupSet : IEnumerable<KeyValuePair<string, ConfigSectionGroup>>, IEnumerable
    {
        private readonly IDictionary<string, XElement> _contents = new Dictionary<string, XElement>();
        private readonly XElement _contentSuperior;
        private readonly IDictionary<string, XElement> _declarations = new Dictionary<string, XElement>();
        private readonly XElement _declarationSuperior;
        private readonly ISavable _savable;
        private readonly IDictionary<string, ConfigSectionGroup> _values = new Dictionary<string, ConfigSectionGroup>();

        /// <summary>
        /// 获取配置组集合中包含的元素数。
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// 获取配置组集合的名称的集合。
        /// </summary>
        public ICollection<string> Names => _values.Keys;

        /// <summary>
        /// 获取配置组集合。
        /// </summary>
        public ICollection<ConfigSectionGroup> Values => _values.Values;

        /// <summary>
        /// 获取具有指定名称的配置组的值。
        /// </summary>
        /// <param name="name">配置组的名称。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public ConfigSectionGroup this[string name] => _values.ContainsKey(name) ? _values[name] : null;

        #region Constructor

        internal ConfigSectionGroupSet(XElement declarationSuperior, XElement contentSuperior, ISavable savable)
        {
            _declarationSuperior = declarationSuperior;
            _contentSuperior = contentSuperior;
            _savable = savable;
            if (declarationSuperior.HasElements)
            {
                foreach (XElement declaration in declarationSuperior.Elements("sectionGroup"))
                {
                    string name = declaration.Attribute("name").Value;
                    XElement content = contentSuperior.Element(name);
                    ConfigSectionGroup value = new ConfigSectionGroup(declaration, content, savable);
                    _values.Add(name, value);
                    _contents.Add(name, content);
                    _declarations.Add(name, declaration);
                }
            }
        }

        #endregion Constructor

        /// <summary>
        /// 从配置组集合中移除所有配置组。
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
        /// 确定配置组集合是否包含带有指定名称的配置组。
        /// </summary>
        /// <param name="name">配置组的名称。</param>
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
        public IEnumerator<KeyValuePair<string, ConfigSectionGroup>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// 获取与指定名称关联的配置组的值。如果不存在，添加一个配置组并返回值。
        /// </summary>
        /// <param name="name">配置组的名称。</param>
        /// <exception cref="Exception"/>
        public ConfigSectionGroup GetOrAdd(string name)
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
                XElement declaration = new XElement("sectionGroup");
                declaration.SetAttributeValue("name", name);
                XElement content = new XElement(name);
                ConfigSectionGroup value = new ConfigSectionGroup(declaration, content, _savable);
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
        /// 从配置组集合中移除带有指定名称的配置组。
        /// </summary>
        /// <param name="name">配置组的名称。</param>
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
        /// 获取与指定名称关联的配置组的值。
        /// </summary>
        /// <param name="name">配置组的名称。</param>
        /// <param name="value">配置组的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out ConfigSectionGroup value)
        {
            return _values.TryGetValue(name, out value);
        }
    }
}