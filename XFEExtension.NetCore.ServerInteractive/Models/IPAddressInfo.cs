namespace XFEExtension.NetCore.ServerInteractive.Models;

/// <summary>
/// IP地址信息
/// </summary>
public class IpAddressInfo
{
    /// <summary>
    /// IP地址
    /// </summary>
    public required string IpAddress { get; set; }
    /// <summary>
    /// IP地址备注
    /// </summary>
    public string? Notes { get; set; }
    /// <summary>
    /// 封禁开始时间
    /// </summary>
    public DateTime BannedTime { get; set; } = DateTime.Now;
}
