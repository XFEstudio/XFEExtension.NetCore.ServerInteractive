namespace XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;

/// <summary>
/// 标准客户端实例请求
/// </summary>
public class StandardClientInstanceRequest
{
    /// <summary>
    /// 构建请求体
    /// </summary>
    public Func<string, string, object[], object> ConstructBody { get; set; } = (_, _, _) => null!;
    /// <summary>
    /// 处理请求
    /// </summary>
    public Func<string, object>? ProcessResponse { get; set; } = response => response;
}
