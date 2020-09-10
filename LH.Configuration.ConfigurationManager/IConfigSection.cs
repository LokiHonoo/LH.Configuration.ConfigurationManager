namespace LH.Configuration
{
    /// <summary>
    /// 配置容器。
    /// </summary>
    public interface IConfigSection
    {
        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns></returns>
        bool Equals(object obj);

        /// <summary>
        /// 作为默认哈希函数。
        /// </summary>
        /// <returns></returns>
        int GetHashCode();

        /// <summary>
        /// 方法已重写。返回节点的缩进 XML 文本。
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}