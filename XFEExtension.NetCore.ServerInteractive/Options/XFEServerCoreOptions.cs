using XFEExtension.NetCore.CyberComm;

namespace XFEExtension.NetCore.ServerInteractive.Options;

/// <summary>
/// XFE服务器核心选项
/// </summary>
public class XFEServerCoreOptions
{
    /// <summary>
    /// 绑定的IP地址（服务初始化执行完成后才会刷新）
    /// </summary>
    public List<string> BindingIPAddress { get; set; } = [];
    /// <summary>
    /// 核心服务器名称
    /// </summary>
    public string ServerCoreName { get; set; } = string.Empty;
    /// <summary>
    /// 是否自动对Json字符串进行反转义处理，默认为true
    /// </summary>
    public bool AutoUnescapeJson { get; set; } = true;
    /// <summary>
    /// 是否接收非标准的Json字符串，默认为true
    /// </summary>
    public bool AcceptNonStandardJson { get; set; } = true;
    /// <summary>
    /// 获取IP地址的函数，默认为从请求事件参数中获取客户端IP地址
    /// </summary>
    public Func<CyberCommRequestEventArgs, string> GetIPFunction { get; set; } = args => args.ClientIP;

    /// <summary>
    /// 绑定IP地址
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public XFEServerCoreOptions BindIP(string ipAddress)
    {
        BindingIPAddress.Add(ipAddress);
        return this;
    }
}
