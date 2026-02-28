using System.Net;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 核心日志服务
/// </summary>
public class CoreLogService : ServerCoreUserServiceBase
{
    /// <summary>
    /// 获取日志权限
    /// </summary>
    public int GetPermission { get; set; }
    /// <summary>
    /// 清理日志权限
    /// </summary>
    public int ClearPermission { get; set; }

    /// <inheritdoc/>
    public override async Task StandardRequestReceived()
    {
        switch (Execute)
        {
            case "get_log":
                Console.Write($"【{ReturnArgs!.Args.ClientIP}】获取服务器日志请求");
                UserHelper.ValidatePermission(QueryableJsonNode!["session"], QueryableJsonNode!["computerInfo"], ReturnArgs.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                if (!DateTime.TryParse(QueryableJsonNode!["startDateTime"], out var startDatetime)) Close("起始日期格式不正确", HttpStatusCode.BadRequest);
                if (!DateTime.TryParse(QueryableJsonNode!["endDateTime"], out var endDatetime)) Close("结束日期格式不正确", HttpStatusCode.BadRequest);
                await ReturnArgs.Args.ReplyAndClose(XFEConsole.XFEConsole.Log.Export(startDatetime, endDatetime));
                break;
            case "clear_log":
                Console.Write($"【{ReturnArgs!.Args.ClientIP}】清除服务器日志请求");
                UserHelper.ValidatePermission(QueryableJsonNode!["session"], QueryableJsonNode!["computerInfo"], ReturnArgs.Args.ClientIP, ClearPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                if (File.Exists("server.log"))
                    File.Delete("server.log");
                XFEConsole.XFEConsole.Log.Clear();
                ReturnArgs.Args.Close();
                break;
            default:
                break;
        }
    }
}
