using System.ComponentModel;

namespace LH.Configuration
{
    /// <summary>
    /// 配置容器的类型。
    /// </summary>
    public enum ConfigSectionType
    {
        /// <summary>
        /// System.Configuration.DictionarySectionHandler 类型。
        /// </summary>
        [Description("System.Configuration.DictionarySectionHandler")]
        DictionarySectionHandler = 1,

        /// <summary>
        /// System.Configuration.NameValueSectionHandler 类型。
        /// </summary>
        [Description("System.Configuration.NameValueSectionHandler")]
        NameValueSectionHandler = 2,

        /// <summary>
        /// System.Configuration.NameValueSectionHandler 类型。
        /// </summary>
        [Description("System.Configuration.NameValueSectionHandler")]
        SingleTagSectionHandler = 3,
    }
}