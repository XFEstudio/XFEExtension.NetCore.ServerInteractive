namespace XFEExtension.NetCore.ServerInteractive.Attributes;

/// <summary>
/// 响应特性，用于标记IStandardRequestService中解析响应的方法
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class ResponseAttribute : Attribute
{
    /// <summary>
    /// 响应路径
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 响应名称（可选），使用请求器时可通过名称代替路径进行请求
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 创建响应特性
    /// </summary>
    /// <param name="path">响应路径（例如：user/login）</param>
    public ResponseAttribute(string path)
    {
        Path = path;
    }
}
