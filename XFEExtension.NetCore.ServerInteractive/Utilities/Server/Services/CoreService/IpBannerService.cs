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
                UserHelper.ValidatePermission(QueryableJsonNode!["session"], QueryableJsonNode!["computerInfo"], ReturnArgs.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                await ReturnArgs.Args.ReplyAndClose(ServerBaseProfile.BannedIPAddressList.ToJson());
                break;
            case "add_bannedIp":
                Console.WriteLine($"【{ReturnArgs!.Args.ClientIP}】添加禁止的IP地址请求 添加：{QueryableJsonNode!["bannedIp"]}");
                UserHelper.ValidatePermission(QueryableJsonNode!["session"], QueryableJsonNode!["computerInfo"], ReturnArgs.Args.ClientIP, AddPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                if (QueryableJsonNode!["bannedIp"] is null) Close("无IP地址传入", HttpStatusCode.BadRequest);
                ServerBaseProfile.BannedIPAddressList.Add(new()
                {
                    IPAddress = QueryableJsonNode!["bannedIp"],
                    Notes = QueryableJsonNode!["notes"]
                });
                ReturnArgs.Args.Close();
                break;
            case "remove_bannedIp":
                Console.WriteLine($"【{ReturnArgs!.Args.ClientIP}】删除禁止的IP地址请求 移除：{QueryableJsonNode!["bannedIp"]}");
                UserHelper.ValidatePermission(QueryableJsonNode!["session"], QueryableJsonNode!["computerInfo"], ReturnArgs.Args.ClientIP, RemovePermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                var targetIp = ServerBaseProfile.BannedIPAddressList.FirstOrDefault(ip => ip.IPAddress == QueryableJsonNode!["bannedIp"].ToString()) ?? throw ReturnArgs.GetError("无IP地址传入", HttpStatusCode.BadRequest);
                await ReturnArgs.Args.ReplyAndClose(ServerBaseProfile.BannedIPAddressList.Remove(targetIp).ToString());
                break;
            default:
                break;
        }
    }
}
