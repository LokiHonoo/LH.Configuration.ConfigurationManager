using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 配置属性集合。
    /// </summary>
    public sealed class SingleTagSectionPropertySet : IEnumerable
    {
        private readonly XElement _content;
        private readonly ISavable _savable;
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

        internal SingleTagSectionPropertySet(XElement content, ISavable savable)
        {
            _content = content;
            _savable = savable;
            if (content.HasAttributes)
            {
                foreach (XAttribute attribute in content.Attributes())
                {
                    _values.Add(attribute.Name.LocalName, attribute.Value);
                }
            }
        }

        #endregion Constructor

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
                    _content.Attribute(key).Remove();
                    if (_savable.AutoSave)
                    {
                        _savable.Save();
                    }
                }
            }
            else
            {
                if (_values.ContainsKey(key))
                {
                    _content.SetAttributeValue(key, value);
                    _values[key] = value;
                }
                else
                {
                    _content.SetAttributeValue(key, value);
                    _values.Add(key, value);
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
            _content.RemoveAttributes();
            if (_savable.AutoSave)
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
                _content.Attribute(key).Remove();
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
        /// 获取与指定键关联的配置属性的值。
        /// </summary>
        /// <param name="key">配置属性的键。</param>
        /// <param name="value">配置属性的值。</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out string value)
        {
            return _values.TryGetValue(key, out value);
        }
    }
}