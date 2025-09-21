using System.Net;
using System.Text.Json;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;
using XFEExtension.NetCore.StringExtension.Json;

namespace XFEExtension.NetCore.ServerInteractive.Utilities;

public class ServerInteractive
{
    JsonSerializerOptions jsonSerializerOptions = new();
    /// <summary>
    /// 请求消息返回事件
    /// </summary>
    public event XFEEventHandler<object?, ServerInteractiveEventArgs>? MessageReceived;
    /// <summary>
    /// 表格请求器
    /// </summary>
    public TableRequester TableRequester { get; set; }

    public ServerInteractive(string requsetAddress, string computerInfo, string session)
    {
        jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
        TableRequester = new()
        {
            RequestAddress = requsetAddress,
            ComputerInfo = computerInfo,
            Session = session,
            JsonSerializerOptions = jsonSerializerOptions
        };
        TableRequester.MessageReceived += TableRequester_MessageReceived;
    }

    private void TableRequester_MessageReceived(object? sender, ServerInteractiveEventArgs e) => MessageReceived?.Invoke(sender, e);

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<string> Login(string account, string password)
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "login",
                account,
                password,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return response;
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return string.Empty;
        }
    }

    /// <summary>
    /// 登录校验
    /// </summary>
    /// <returns></returns>
    public async Task<string> ReLogin()
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "relogin",
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return response;
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return string.Empty;
        }
    }

    /// <summary>
    /// 获取禁止的IP地址列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<IPAddressInfo>> GetBannedIpAddressList()
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "get_bannedIpList",
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return JsonSerializer.Deserialize<List<IPAddressInfo>>(response) ?? [];
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return [];
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return [];
        }
    }

    /// <summary>
    /// 获取服务器日志
    /// </summary>
    /// <param name="startDateTime"></param>
    /// <param name="endDateTime"></param>
    /// <returns></returns>
    public async Task<string> GetServerLog(DateTime startDateTime, DateTime endDateTime)
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "get_log",
                startDateTime,
                endDateTime,
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return response;
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return string.Empty;
        }
    }

    /// <summary>
    /// 清空服务器日志
    /// </summary>
    /// <returns></returns>
    public async Task<string> ClearServerLog()
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "clear_log",
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return response;
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return string.Empty;
        }
    }

    /// <summary>
    /// 获取配置文件
    /// </summary>
    /// <param name="profileName"></param>
    /// <returns></returns>
    public async Task<string> GetProfile(string profileName)
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "get_profile",
                profileName,
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return response;
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return string.Empty;
        }
    }

    /// <summary>
    /// 更改配置文件
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="profile"></param>
    /// <returns></returns>
    public async Task ChangeProfile(string profileName, string profile)
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "change_profile",
                profileName,
                profile,
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
        }
    }

    /// <summary>
    /// 添加禁止的IP地址
    /// </summary>
    /// <param name="bannedIp"></param>
    /// <param name="notes"></param>
    /// <returns></returns>
    public async Task AddBannedIpAddress(string bannedIp, string notes = "")
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "add_bannedIp",
                bannedIp,
                notes,
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
        }
    }

    /// <summary>
    /// 移除禁止的IP地址
    /// </summary>
    /// <param name="bannedIp"></param>
    /// <returns></returns>
    public async Task RemoveBannedIpAddress(string bannedIp)
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "remove_bannedIp",
                bannedIp,
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
        }
    }

    /// <summary>
    /// 管理员更改账户请求
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedAccount"></param>
    /// <returns></returns>
    public async Task<bool> AdminChangeAccount(string id, string changedAccount)
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "change_account",
                changedAccount,
                id,
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return true;
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return false;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return false;
        }
    }

    /// <summary>
    /// 管理员更改密码请求
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changedPassword"></param>
    /// <returns></returns>
    public async Task<bool> AdminChangePassword(string id, string changedPassword)
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(TableRequester.RequestAddress, new
            {
                execute = "change_password",
                id,
                changedPassword,
                session = TableRequester.Session,
                computerInfo = TableRequester.ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return true;
            }
            else
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                return false;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return false;
        }
    }
}
