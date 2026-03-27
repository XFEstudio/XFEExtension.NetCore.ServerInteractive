using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

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
    public override async Task RequestReceiveAsync()
    {
        switch (Execute)
        {
            case "get_log":
                Console.Write("获取服务器日志请求");
                UserHelper.ValidatePermission(Json?["session"], Json?["deviceInfo"], ReturnArgs.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                if (!DateTime.TryParse(Json?["startDateTime"], out var startDatetime)) throw Error("起始日期格式不正确");
                if (!DateTime.TryParse(Json?["endDateTime"], out var endDatetime)) throw Error("结束日期格式不正确");
                await Close(XFEConsole.XFEConsole.Log.Export(startDatetime, endDatetime));
                break;
            case "clear_log":
                Console.Write("清除服务器日志请求");
                UserHelper.ValidatePermission(Json?["session"], Json?["deviceInfo"], ReturnArgs.Args.ClientIP, ClearPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                if (File.Exists("server.log"))
                    File.Delete("server.log");
                XFEConsole.XFEConsole.Log.Clear();
                OK();
                break;
        }
    }
}
