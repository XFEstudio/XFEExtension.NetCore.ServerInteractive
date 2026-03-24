namespace XFEExtension.NetCore.ServerInteractive.Options;

/// <summary>
/// XFE客户端请求器选项
/// </summary>
public class XFEClientRequesterOptions
{
    /// <summary>
    /// 请求地址
    /// </summary>
    public string RequestAddress { get; set; } = "http://localhost:3300/";
    /// <summary>
    /// 本次会话Session
    /// </summary>
    public string Session { get; set; } = string.Empty;
    /// <summary>
    /// 电脑信息
    /// </summary>
    public string ComputerInfo { get; set; } = string.Empty;
    /// <summary>
    /// 自动反转义响应内容（针对XFERequestService和XFEClientInstanceRequest的响应内容进行反转义处理，默认为true）
    /// </summary>
    public bool AutoUnescapeResponse { get; set; } = true;
}
