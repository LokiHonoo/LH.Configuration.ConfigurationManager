namespace LH.Configuration
{
    /// <summary>
    /// 本地化错误消息。
    /// </summary>
    public static class ExceptionMessages
    {
        /// <summary>
        /// 表示在一个列表中，遇到重复键时引发的错误。
        /// </summary>
        public static ExceptionMessage DuplicateKey { get; } = new ExceptionMessage("The specified key already exists.", "The specified key already exists.");

        /// <summary>
        /// 表示指定了无效的键时引发的错误。
        /// </summary>
        public static ExceptionMessage InvalidKey { get; } = new ExceptionMessage("The invalid key.", "The invalid key.");

        /// <summary>
        /// 表示指定了无效的类型或类型枚举时引发的错误。
        /// </summary>
        public static ExceptionMessage InvalidType { get; } = new ExceptionMessage("The invalid type.", "The invalid type.");
    }
}