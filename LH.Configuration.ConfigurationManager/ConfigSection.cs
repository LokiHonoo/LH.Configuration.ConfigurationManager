namespace LH.Configuration
{
    /// <summary>
    /// 配置容器。
    /// </summary>
    public abstract class ConfigSection
    {
        private readonly string _typeName;

        /// <summary>
        /// 获取配置容器的类型的文本表示。
        /// </summary>
        public string TypeName => _typeName;

        #region Constructor

        internal ConfigSection(string typeName)
        {
            _typeName = typeName;
        }

        #endregion Constructor
    }
}