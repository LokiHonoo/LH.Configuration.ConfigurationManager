using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置属性集合。
    /// </summary>
    public sealed class NameValueSectionPropertySet : IEnumerable
    {
        private readonly IDictionary<string, XElement> _contents = new Dictionary<string, XElement>();
        private readonly ISavable _savable;
        private readonly XElement _superior;
        private readonly IDictionary<string, string> _values = new Dictionary<string, string>();

        /// <summary>
        /// 获取配置属性集合中包含的元素数。
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// 获取配置属性集合的键的集合。
        /// </summary>
        public ICollection<string> Keys => _values.Keys;

        /// <summary>
        /// 获取配置属性集合的值的集合。
        /// </summary>
        public ICollection<string> Values => _values.Values;

        /// <summary>
        /// 获取或设置具有指定键的配置属性的值。直接赋值等同于 AddOrUpdate 方法。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <returns></returns>
        public string this[string key]
        {
            get => _values.ContainsKey(key) ? _values[key] : null;
            set { AddOrUpdate(key, value); }
        }

        #region Constructor

        internal NameValueSectionPropertySet(XElement superior, ISavable savable)
        {
            _superior = superior;
            _savable = savable;
            if (superior.HasElements)
            {
                foreach (XElement content in superior.Elements("add"))
                {
                    string key = content.Attribute("key").Value;
                    string value = content.Attribute("value").Value;
                    _values.Add(key, value);
                    _contents.Add(key, content);
                }
            }
        }

        #endregion Constructor

        /// <summary>
        /// 添加或合并一个配置属性。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <param name="value">配置属性的值。合并到已存在的字符串中，以逗号 "," 连接为一个字符串。</param>
        public void AddOrMerge(string key, string value)
        {
            if (value is null)
            {
                if (_values.Remove(key))
                {
                    _contents[key].Remove();
                    _contents.Remove(key);
                    if (!(_savable is null) && _savable.AutoSave)
                    {
                        _savable.Save();
                    }
                }
            }
            else
            {
                if (_values.TryGetValue(key, out string old))
                {
                    string merge = $"{old},{value}";
                    _contents[key].SetAttributeValue("value", merge);
                    _values[key] = merge;
                }
                else
                {
                    XElement content = new XElement("add");
                    content.SetAttributeValue("key", key);
                    content.SetAttributeValue("value", value);
                    _values.Add(key, value);
                    _contents.Add(key, content);
                    _superior.Add(content);
                }
                if (!(_savable is null) && _savable.AutoSave)
                {
                    _savable.Save();
                }
            }
        }

        /// <summary>
        /// 添加或更新一个配置属性。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <param name="value">配置属性的值。</param>
        public void AddOrUpdate(string key, string value)
        {
            if (value is null)
            {
                if (_values.Remove(key))
                {
                    _contents[key].Remove();
                    _contents.Remove(key);
                    if (!(_savable is null) && _savable.AutoSave)
                    {
                        _savable.Save();
                    }
                }
            }
            else
            {
                if (_values.ContainsKey(key))
                {
                    _contents[key].SetAttributeValue("value", value);
                    _values[key] = value;
                }
                else
                {
                    XElement content = new XElement("add");
                    content.SetAttributeValue("key", key);
                    content.SetAttributeValue("value", value);
                    _values.Add(key, value);
                    _contents.Add(key, content);
                    _superior.Add(content);
                }
                if (!(_savable is null) && _savable.AutoSave)
                {
                    _savable.Save();
                }
            }
        }

        /// <summary>
        /// 添加或更新一个配置属性。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <param name="value">配置属性的值。以逗号 "," 连接为一个字符串。</param>
        public void AddOrUpdate(string key, string[] value)
        {
            if (value is null)
            {
                if (_values.Remove(key))
                {
                    _contents[key].Remove();
                    _contents.Remove(key);
                    if (!(_savable is null) && _savable.AutoSave)
                    {
                        _savable.Save();
                    }
                }
            }
            else
            {
                if (_values.ContainsKey(key))
                {
                    string merge = string.Join(",", value);
                    _contents[key].SetAttributeValue("value", merge);
                    _values[key] = merge;
                }
                else
                {
                    XElement content = new XElement("add");
                    content.SetAttributeValue("key", key);
                    string merge = string.Join(",", value);
                    content.SetAttributeValue("value", merge);
                    _values.Add(key, merge);
                    _contents.Add(key, content);
                    _superior.Add(content);
                }
                if (!(_savable is null) && _savable.AutoSave)
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
            _superior.RemoveNodes();
            if (!(_savable is null) && _savable.AutoSave)
            {
                _savable.Save();
            }
        }

        /// <summary>
        /// 确定配置属性集合是否包含带有指定键的配置属性。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

        /// <summary>
        /// 支持在泛型集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// 从配置属性集合中移除带有指定键的配置属性。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (_values.Remove(key))
            {
                _contents[key].Remove();
                _contents.Remove(key);
                if (!(_savable is null) && _savable.AutoSave)
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
        /// 获取与指定键关联的配置属性的值。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <param name="value">配置属性的值。</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out string value)
        {
            return _values.TryGetValue(key, out value);
        }

        /// <summary>
        /// 获取与指定键关联的配置属性的值。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <param name="value">配置属性的值。以逗号 "," 分割字符串。</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out string[] value)
        {
            if (_values.TryGetValue(key, out string val))
            {
                value = val.Split(',');
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        internal IDictionary<string, string> GetInternalValues()
        {
            return _values;
        }
    }
}