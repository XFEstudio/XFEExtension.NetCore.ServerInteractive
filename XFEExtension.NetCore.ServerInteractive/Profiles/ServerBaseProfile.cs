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
    [ProfilePropertyAddGet("Current._bannedIPAddressList.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current._bannedIPAddressList")]
    private ProfileList<IPAddressInfo> _bannedIPAddressList = [];
}
