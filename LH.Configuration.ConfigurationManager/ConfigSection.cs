using System.Collections.Generic;
using System.Collections.Specialized;

namespace LH.Configuration
{
    /// <summary>
    /// 配置属性。
    /// </summary>
    public abstract class ConfigSection
    {
        private readonly string _typeName;

        /// <summary>
        /// 获取节点的类型的文本表示。
        /// </summary>
        public string TypeName => _typeName;

        #region Constructor

        internal ConfigSection(string typeName)
        {
            _typeName = typeName;
        }

        #endregion Constructor

        /// <summary>
        /// 创建 DictionarySection 的新实例。
        /// </summary>
        /// <param name="value">配置属性的值。</param>
        /// <returns></returns>
        public static DictionarySection Create(IDictionary<string, object> value)
        {
            return new DictionarySection(value);
        }

        /// <summary>
        /// 创建 NameValueSection 的新实例。
        /// </summary>
        /// <param name="value">配置属性的值。</param>
        /// <returns></returns>
        public static NameValueSection Create(NameValueCollection value)
        {
            return new NameValueSection(value);
        }

        /// <summary>
        /// 创建 SingleTagSection 的新实例。
        /// </summary>
        /// <param name="value">配置属性的值。</param>
        /// <returns></returns>
        public static SingleTagSection Create(StringDictionary value)
        {
            return new SingleTagSection(value);
        }
    }
}