using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.Profiles;

public partial class ServerBaseProfile : XFEProfile
{
    /// <summary>
    /// 服务器入口点
    /// </summary>
    [ProfileProperty]
    private string entryPoint = "api";
    /// <summary>
    /// 封禁的IP地址列表
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current.bannedIPAddressList.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.bannedIPAddressList")]
    private ProfileList<IPAddressInfo> bannedIPAddressList = [];
    /// <summary>
    /// 服务器上次绑定的IP地址字典
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current.serverLastBindingAddressDictionary.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.serverLastBindingAddressDictionary")]
    private ProfileDictionary<string, string> serverLastBindingAddressDictionary = [];
}
