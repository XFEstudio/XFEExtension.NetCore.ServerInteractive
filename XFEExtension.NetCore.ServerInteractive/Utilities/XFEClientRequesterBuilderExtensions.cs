using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Serviecs;

namespace XFEExtension.NetCore.ServerInteractive.Utilities;

/// <summary>
/// XFE客户端请求器构建器扩展
/// </summary>
public static class XFEClientRequesterBuilderExtensions
{
    readonly static JsonSerializerOptions jsonSerializerOptions = new();

    static XFEClientRequesterBuilderExtensions() => jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <summary>
    /// 添加连接检查请求
    /// </summary>
    /// <param name="xFEClientRequesterBuilder"></param>
    /// <returns></returns>
    public static XFEClientRequesterBuilder AddCheckConnectRequest(this XFEClientRequesterBuilder xFEClientRequesterBuilder) => xFEClientRequesterBuilder.AddRequest("check_connect", static (_, _, _) => new
    {
        execute = "check_connect"
    }, response => DateTime.Parse(response));

    /// <summary>
    /// 添加日志请求
    /// </summary>
    /// <param name="xFEClientRequesterBuilder"></param>
    /// <returns></returns>
    public static XFEClientRequesterBuilder AddLogRequest(this XFEClientRequesterBuilder xFEClientRequesterBuilder) => xFEClientRequesterBuilder.AddRequest("get_log", static (session, computerInfo, parameters) => new
    {
        execute = "get_log",
        session,
        computerInfo,
        startDateTime = parameters[0],
        endDateTime = parameters[1]
    }, static response => response).AddRequest("clear_log", static (session, computerInfo, parameters) => new
    {
        execute = "clear_log",
        session,
        computerInfo
    }, static response => response);

    /// <summary>
    /// 添加IP封禁请求
    /// </summary>
    /// <param name="xFEClientRequesterBuilder"></param>
    /// <returns></returns>
    public static XFEClientRequesterBuilder AddBannedIpRequest(this XFEClientRequesterBuilder xFEClientRequesterBuilder) => xFEClientRequesterBuilder.AddRequest("get_bannedIpList", static (session, computerInfo, parameters) => new
    {
        execute = "get_bannedIpList",
        session,
        computerInfo
    }, static response => JsonSerializer.Deserialize<List<IPAddressInfo>>(response, jsonSerializerOptions)!).AddRequest("add_bannedIp", static (session, computerInfo, parameters) => new
    {
        execute = "add_bannedIp",
        session,
        computerInfo,
        bannedIp = parameters[0],
        notes = parameters[1]
    }, static response => response).AddRequest("remove_bannedIp", static (session, computerInfo, parameters) => new
    {
        execute = "remove_bannedIp",
        session,
        computerInfo,
        bannedIp = parameters[0]
    }, static response => bool.Parse(response));

    /// <summary>
    /// 添加登录服务
    /// </summary>
    /// <typeparam name="T">登录返回用户接口类型</typeparam>
    /// <param name="xFEClientRequesterBuilder"></param>
    /// <returns></returns>
    public static XFEClientRequesterBuilder AddLoginRequest<T>(this XFEClientRequesterBuilder xFEClientRequesterBuilder) where T : IUserFaceInfo => xFEClientRequesterBuilder.AddXFERequest<LoginRequestService<T>>("login").AddXFERequest<ReloginRequestService<T>>("relogin");

    /// <summary>
    /// 使用XFE标准服务器服务请求
    /// </summary>
    /// <typeparam name="T">登录返回用户接口类型</typeparam>
    /// <param name="xFEClientRequesterBuilder"></param>
    /// <returns></returns>
    public static XFEClientRequesterBuilder UseXFEStandardRequest<T>(this XFEClientRequesterBuilder xFEClientRequesterBuilder) where T : IUserFaceInfo => xFEClientRequesterBuilder.AddLoginRequest<T>()
        .AddBannedIpRequest()
        .AddLogRequest()
        .AddCheckConnectRequest();
}
