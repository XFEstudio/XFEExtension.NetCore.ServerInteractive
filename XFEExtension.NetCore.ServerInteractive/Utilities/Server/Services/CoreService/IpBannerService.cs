using System.Net;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Profiles;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension.Json;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// IP封禁服务
/// </summary>
public class IpBannerService : ServerCoreUserServiceBase
{
    /// <summary>
    /// 获取权限
    /// </summary>
    public int GetPermission { get; set; } = 0;
    /// <summary>
    /// 添加权限
    /// </summary>
    public int AddPermission { get; set; } = 0;
    /// <summary>
    /// 移除权限
    /// </summary>
    public int RemovePermission { get; set; } = 0;
    /// <inheritdoc/>
    public override async Task StandardRequestReceived()
    {
        switch (Execute)
        {
            case "get_bannedIpList":
                Console.Write($"【{ReturnArgs!.Args.ClientIP}】获取禁止的IP地址列表请求");
                UserHelper.ValidatePermission(Json!["session"], Json!["computerInfo"], ReturnArgs.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                await Close(ServerBaseProfile.BannedIPAddressList.ToJson());
                break;
            case "add_bannedIp":
                Console.WriteLine($"【{ReturnArgs!.Args.ClientIP}】添加禁止的IP地址请求 添加：{Json!["bannedIp"]}");
                UserHelper.ValidatePermission(Json!["session"], Json!["computerInfo"], ReturnArgs.Args.ClientIP, AddPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                if (Json!["bannedIp"] is null) Error("无IP地址传入");
                ServerBaseProfile.BannedIPAddressList.Add(new()
                {
                    IPAddress = Json!["bannedIp"],
                    Notes = Json!["notes"]
                });
                OK();
                break;
            case "remove_bannedIp":
                Console.WriteLine($"【{ReturnArgs!.Args.ClientIP}】删除禁止的IP地址请求 移除：{Json!["bannedIp"]}");
                UserHelper.ValidatePermission(Json!["session"], Json!["computerInfo"], ReturnArgs.Args.ClientIP, RemovePermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                var targetIp = ServerBaseProfile.BannedIPAddressList.FirstOrDefault(ip => ip.IPAddress == Json!["bannedIp"].ToString()) ?? throw ReturnArgs.GetError("无IP地址传入", HttpStatusCode.BadRequest);
                await Close(ServerBaseProfile.BannedIPAddressList.Remove(targetIp).ToString());
                break;
            default:
                break;
        }
    }
}
