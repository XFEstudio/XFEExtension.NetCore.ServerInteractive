namespace XFEExtension.NetCore.ServerInteractive.Attributes;

/// <summary>
/// 请求特性，用于标记IStandardRequestService中构造请求体的方法
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class RequestAttribute : Attribute
{
    /// <summary>
    /// 请求路径
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 请求名称（可选），使用请求器时可通过名称代替路径进行请求
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 创建请求特性
    /// </summary>
    /// <param name="path">请求路径（例如：user/login）</param>
    public RequestAttribute(string path)
    {
        Path = path;
    }
}
