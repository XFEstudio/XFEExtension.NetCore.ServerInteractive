namespace XFEExtension.NetCore.ServerInteractive.Attributes;

/// <summary>
/// 请求特性，用于标记IStandardRequestService中构造请求体的方法
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class RequestAttribute : Attribute
{
    /// <summary>
    /// 请求路径
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 创建请求特性
    /// </summary>
    /// <param name="path">请求路径（例如：user/login）</param>
    public RequestAttribute(string path)
    {
        Path = path;
    }
}
