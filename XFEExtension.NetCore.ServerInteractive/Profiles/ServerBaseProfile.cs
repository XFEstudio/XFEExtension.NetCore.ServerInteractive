using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.Profiles;

public partial class ServerBaseProfile : XFEProfile
{
    /// <summary>
    /// 服务器入口点
    /// </summary>
    [ProfileProperty]
    private string _entryPoint = "api";
    /// <summary>
    /// 封禁的IP地址列表
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current._bannedIpAddressList.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current._bannedIpAddressList")]
    private ProfileList<IpAddressInfo> _bannedIpAddressList = [];
    /// <summary>
    /// 服务器上次绑定的IP地址字典
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current._serverLastBindingAddressDictionary.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current._serverLastBindingAddressDictionary")]
    private ProfileDictionary<string, string> _serverLastBindingAddressDictionary = [];
}
