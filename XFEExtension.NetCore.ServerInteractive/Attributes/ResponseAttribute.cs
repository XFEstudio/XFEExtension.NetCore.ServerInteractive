namespace XFEExtension.NetCore.ServerInteractive.Attributes;

/// <summary>
/// 响应特性，用于标记IStandardRequestService中解析响应的方法
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ResponseAttribute : Attribute
{
    /// <summary>
    /// 响应路径
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 创建响应特性
    /// </summary>
    /// <param name="path">响应路径（例如：user/login）</param>
    public ResponseAttribute(string path)
    {
        Path = path;
    }
}
