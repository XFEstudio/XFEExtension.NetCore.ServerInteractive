using System.Net;
using System.Text;
using System.Text.Json;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension.Json;

namespace XFEExtension.NetCore.ServerInteractive;

public class TableRequester
{
    /// <summary>
    /// 请求地址
    /// </summary>
    public required string RequestAddress { get; set; }
    /// <summary>
    /// 电脑信息
    /// </summary>
    public required string ComputerInfo { get; set; }
    /// <summary>
    /// 用户登录Session
    /// </summary>
    public required string Session { get; set; }
    /// <summary>
    /// Json序列化选项
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
    /// <summary>
    /// 请求消息返回事件
    /// </summary>
    public event XFEEventHandler<object?, ServerInteractiveEventArgs>? MessageReceived;

    /// <summary>
    /// 获取请求名称
    /// </summary>
    /// <typeparam name="T">请求数据类型</typeparam>
    /// <returns>请求时所用的名称</returns>
    public static string GetRequestName<T>() => $"{typeof(T).Name[0]}".ToLower() + typeof(T).Name[1..];

    /// <summary>
    /// 获取Table列表
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="tableName">表名称</param>
    /// <returns>表内容</returns>
    public async Task<List<T>> Get<T>(string tableName) where T : IIDModel
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress, new
            {
                execute = $"get_{tableName}",
                session = Session,
                computerInfo = ComputerInfo
            }.ToJson());
            if (code == HttpStatusCode.OK)
            {
                MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                return JsonSerializer.Deserialize<List<T>>(response, JsonSerializerOptions) ?? [];
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
    /// 获取Table列表
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns>表内容</returns>
    public async Task<List<T>> Get<T>() where T : IIDModel => await Get<T>(GetRequestName<T>());

    /// <summary>
    /// 向表中添加一个数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="tableName">表名称</param>
    /// <param name="data">数据</param>
    /// <returns>是否添加成功</returns>
    public async Task<bool> Add<T>(string tableName, T data) where T : IIDModel
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress, new
            {
                execute = $"add_{tableName}",
                data = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, JsonSerializerOptions))),
                session = Session,
                computerInfo = ComputerInfo
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
    /// 向表中添加一个数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">数据</param>
    /// <returns>是否添加成功</returns>
    public async Task<bool> Add<T>(T data) where T : IIDModel => await Add(GetRequestName<T>(), data);

    /// <summary>
    /// 从表中删除一个数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="tableName">表名称</param>
    /// <param name="id">元素的ID</param>
    /// <returns>是否删除成功</returns>
    public async Task<bool> Remove<T>(string tableName, string id) where T : IIDModel
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress, new
            {
                execute = $"remove_{tableName}",
                id,
                session = Session,
                computerInfo = ComputerInfo
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
    /// 从表中删除一个数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="id">元素的ID</param>
    /// <returns>是否删除成功</returns>
    public async Task<bool> Remove<T>(string id) where T : IIDModel => await Remove<T>(GetRequestName<T>(), id);

    /// <summary>
    /// 更改表中的数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="tableName">表名称</param>
    /// <param name="data">数据</param>
    /// <returns></returns>
    public async Task<bool> Change<T>(string tableName, T data) where T : IIDModel
    {
        try
        {
            var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress, new
            {
                execute = $"change_{tableName}",
                data = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, JsonSerializerOptions))),
                session = Session,
                computerInfo = ComputerInfo
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
    /// 更改表中的数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">数据</param>
    /// <returns>是否修改成功</returns>
    public async Task<bool> Change<T>(T data) where T : IIDModel => await Change<T>(GetRequestName<T>(), data);
}
