namespace LH.Configuration
{
    internal interface ISavable
    {
        /// <summary>
        /// 获取自动保存选项。
        /// </summary>
        bool AutoSave { get; }

        /// <summary>
        /// 保存到创建 <see cref="ConfigurationManager"/> 实例时指定的文件。
        /// </summary>
        void Save();
    }
}