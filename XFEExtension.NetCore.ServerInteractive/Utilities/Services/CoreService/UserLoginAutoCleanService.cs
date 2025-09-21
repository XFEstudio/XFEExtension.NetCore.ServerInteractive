using System.Text.Json;
using System.Timers;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

/// <summary>
/// 用户登录到期清除服务
/// </summary>
public class UserLoginAutoCleanService : ServerCoreRegisterServiceBase, IUserServiceBase
{
    /// <inheritdoc/>
    public Func<IEnumerable<User>> GetUserFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Func<IEnumerable<EncryptedUserLoginModel>> GetEncryptedUserLoginModelFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Action<EncryptedUserLoginModel> AddEncryptedUserLoginModelFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public Action<EncryptedUserLoginModel> RemoveEncryptedUserLoginModelFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public Action<User> AddUserFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public Func<int> GetLoginKeepDays { get; set; } = () => 7;
    /// <inheritdoc/>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();

    /// <inheritdoc/>
    public UserLoginAutoCleanService() => JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <inheritdoc/>
    public override void RequestReceived(object? sender, CyberCommRequestEventArgs e) { }

    /// <inheritdoc/>
    public override void ServerStarted(object? sender, EventArgs e)
    {
        Console.WriteLine("[DEBUG]用户登录到期清除服务已启动！");
        var serverTimer = new System.Timers.Timer(TimeSpan.FromHours(12))
        {
            AutoReset = true
        };
        serverTimer.Elapsed += ServerTimer_Elapsed;
        serverTimer.Start();
    }

    private void ServerTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Console.WriteLine("[DEBUG]正在清理过期的Session...");
        try
        {
            var expiredSessions = GetEncryptedUserLoginModelFunction().Where(user => user.UserLoginModel.EndDateTime < DateTime.Now).ToList();
            if (expiredSessions.Count > 0)
            {
                foreach (var expiredSession in expiredSessions)
                {
                    Console.WriteLine($"[DEBUG]用户 {expiredSession.UserLoginModel.UID} 的登录已过期，正在清理...");
                    RemoveEncryptedUserLoginModelFunction(expiredSession);
                }
                Console.WriteLine("[DEBUG]过期的Session已清理完毕！");
            }
            else
            {
                Console.WriteLine("[DEBUG]没有过期的Session需要清理。");
            }
            Console.WriteLine("[DEBUG]正在保存日志...");
            XFEConsole.XFEConsole.Log.Export().WriteIn("server.log");
            Console.WriteLine("[DEBUG]日志已保存！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]清理过期Session时发生错误：{ex.Message}");
            Console.WriteLine($"[TRACE]{ex.StackTrace}");
        }
    }
}
