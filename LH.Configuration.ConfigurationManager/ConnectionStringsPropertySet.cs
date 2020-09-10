using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;

namespace LH.Configuration
{
    /// <summary>
    /// 连接属性集合。
    /// </summary>
    public sealed class ConnectionStringsPropertySet : IEnumerable<KeyValuePair<string, ConnectionStringsValue>>, IEnumerable
    {
        private readonly IDictionary<string, XElement> _contents = new Dictionary<string, XElement>();
        private readonly ISavable _savable;
        private readonly XElement _superior;
        private readonly IDictionary<string, ConnectionStringsValue> _values = new Dictionary<string, ConnectionStringsValue>();

        /// <summary>
        /// 获取连接属性集合中包含的元素数。
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// 获取连接属性集合的名称的集合。
        /// </summary>
        public ICollection<string> Names => _values.Keys;

        /// <summary>
        /// 获取连接属性集合的值的集合。
        /// </summary>
        public ICollection<ConnectionStringsValue> Values => _values.Values;

        /// <summary>
        /// 获取或设置具有指定名称的连接属性的值。直接赋值等同于 AddOrUpdate 方法。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public ConnectionStringsValue this[string name]
        {
            get => _values.ContainsKey(name) ? _values[name] : null;
            set { AddOrUpdate(name, value); }
        }

        #region Constructor

        internal ConnectionStringsPropertySet(XElement superior, ISavable savable)
        {
            _superior = superior;
            _savable = savable;
            if (superior.HasElements)
            {
                foreach (XElement content in superior.Elements("add"))
                {
                    string name = content.Attribute("name").Value;
                    ConnectionStringsValue value = new ConnectionStringsValue(content);
                    _values.Add(name, value);
                    _contents.Add(name, content);
                }
            }
        }

        #endregion Constructor

        /// <summary>
        /// 添加或更新一个连接属性。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <param name="value">连接属性的值。</param>
        /// <exception cref="Exception"/>
        public void AddOrUpdate(string name, ConnectionStringsValue value)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (value is null)
            {
                AddOrUpdate(name, null, null);
            }
            else
            {
                AddOrUpdate(name, value.ConnectionString, value.ProviderName);
            }
        }

        /// <summary>
        /// 添加或更新一个连接属性。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <param name="connection">数据库连接实例。</param>
        /// <exception cref="Exception"/>
        public void AddOrUpdate(string name, DbConnection connection)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (connection is null)
            {
                AddOrUpdate(name, null, null);
            }
            else
            {
                AddOrUpdate(name, connection.ConnectionString, connection.GetType().Namespace);
            }
        }

        /// <summary>
        /// 添加或更新一个连接属性。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="providerName">数据库引擎的文本名称。</param>
        /// <exception cref="Exception"/>
        public void AddOrUpdate(string name, string connectionString, string providerName)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (connectionString is null && providerName is null)
            {
                if (_values.Remove(name))
                {
                    _contents[name].Remove();
                    _contents.Remove(name);
                    if (_savable.AutoSave)
                    {
                        _savable.Save();
                    }
                }
            }
            else if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            else
            {
                if (_values.ContainsKey(name))
                {
                    XElement content = _contents[name];
                    content.SetAttributeValue("connectionString", connectionString);
                    content.SetAttributeValue("providerName", providerName);
                    _values[name] = new ConnectionStringsValue(content);
                }
                else
                {
                    XElement content = new XElement("add");
                    content.SetAttributeValue("name", name);
                    content.SetAttributeValue("connectionString", connectionString);
                    content.SetAttributeValue("providerName", providerName);
                    ConnectionStringsValue value = new ConnectionStringsValue(content);
                    _values.Add(name, value);
                    _contents.Add(name, content);
                    _superior.Add(content);
                }
                if (_savable.AutoSave)
                {
                    _savable.Save();
                }
            }
        }

        /// <summary>
        /// 从连接属性集合中移除所有连接属性。
        /// </summary>
        public void Clear()
        {
            _values.Clear();
            _contents.Clear();
            _superior.RemoveNodes();
            if (_savable.AutoSave)
            {
                _savable.Save();
            }
        }

        /// <summary>
        /// 确定连接属性集合是否包含带有指定名称的连接属性。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
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
        public IEnumerator<KeyValuePair<string, ConnectionStringsValue>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// 从连接属性集合中移除带有指定名称的连接属性。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool Remove(string name)
        {
            if (_values.Remove(name))
            {
                _contents[name].Remove();
                _contents.Remove(name);
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
        /// 获取与指定名称关联的连接属性的值。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <param name="value">连接属性的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out ConnectionStringsValue value)
        {
            return _values.TryGetValue(name, out value);
        }

        /// <summary>
        /// 获取与指定名称关联的连接属性的值。如果连接属性中没有数据库引擎参数，将引发异常。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <param name="connection">连接属性的值。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out DbConnection connection)
        {
            if (_values.TryGetValue(name, out ConnectionStringsValue value))
            {
                connection = value.Connection;
                return true;
            }
            else
            {
                connection = null;
                return false;
            }
        }

        /// <summary>
        /// 获取与指定名称关联的连接属性的值。
        /// </summary>
        /// <param name="name">连接属性的名称。</param>
        /// <param name="connectionString">连接字符串。</param>
        /// <param name="providerName">数据库引擎的文本名称。</param>
        /// <returns></returns>
        /// <exception cref="Exception"/>
        public bool TryGetValue(string name, out string connectionString, out string providerName)
        {
            if (_values.TryGetValue(name, out ConnectionStringsValue value))
            {
                connectionString = value.ConnectionString;
                providerName = value.ProviderName;
                return true;
            }
            else
            {
                connectionString = null;
                providerName = null;
                return false;
            }
        }
    }
}