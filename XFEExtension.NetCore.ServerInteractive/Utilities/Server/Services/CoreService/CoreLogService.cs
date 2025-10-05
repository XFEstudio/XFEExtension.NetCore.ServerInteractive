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
    public override async Task StandardRequestReceived(string execute, QueryableJsonNode queryableJsonNode, ServerCoreReturnArgs r)
    {
        switch (execute)
        {
            case "get_log":
                UserHelper.ValidatePermission(queryableJsonNode["session"], queryableJsonNode["computerInfo"], r.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), r);
                if (!DateTime.TryParse(queryableJsonNode["startDateTime"], out var startDatetime)) r.Error("起始日期格式不正确", HttpStatusCode.BadRequest);
                if (!DateTime.TryParse(queryableJsonNode["endDateTime"], out var endDatetime)) r.Error("结束日期格式不正确", HttpStatusCode.BadRequest);
                await r.Args.ReplyAndClose(XFEConsole.XFEConsole.Log.Export(startDatetime, endDatetime));
                break;
            case "clear_log":
                Console.Write($"【{r.Args.ClientIP}】清除服务器日志请求");
                UserHelper.ValidatePermission(queryableJsonNode["session"], queryableJsonNode["computerInfo"], r.Args.ClientIP, ClearPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), r);
                if (File.Exists("server.log"))
                    File.Delete("server.log");
                XFEConsole.XFEConsole.Log.Clear();
                r.Args.Close();
                break;
            default:
                break;
        }
    }
}
