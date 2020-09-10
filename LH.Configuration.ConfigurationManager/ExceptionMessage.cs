namespace LH.Configuration
{
    /// <summary>
    /// 错误消息。
    /// </summary>
    public class ExceptionMessage
    {
        /// <summary>
        /// 信息的描述。
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 消息的本地化内容。
        /// </summary>
        public string Message { get; set; }

        #region Constructor

        internal ExceptionMessage(string description, string message)
        {
            Description = description;
            Message = message;
        }

        #endregion Constructor
    }
}