using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;

/// <summary>
/// XFE客户端请求器构建器扩展
/// </summary>
public static class XFEClientRequesterBuilderExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new();

    static XFEClientRequesterBuilderExtensions() => JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <param name="xFEClientRequesterBuilder"></param>
    extension(XFEClientRequesterBuilder xFEClientRequesterBuilder)
    {
        /// <summary>
        /// 添加连接检查请求
        /// </summary>
        /// <returns></returns>
        public XFEClientRequesterBuilder AddCheckConnectRequest() => xFEClientRequesterBuilder.AddRequest("check_connect", static (_, _, _) => new
        {
            execute = "check_connect"
        }, response => DateTime.Parse(response));

        /// <summary>
        /// 添加日志请求
        /// </summary>
        /// <returns></returns>
        public XFEClientRequesterBuilder AddLogRequest() => xFEClientRequesterBuilder.AddRequest("get_log", static (session, deviceInfo, parameters) => new
        {
            execute = "get_log",
            session,
            deviceInfo,
            startDateTime = parameters[0],
            endDateTime = parameters[1]
        }, static response => response).AddRequest("clear_log", static (session, deviceInfo, _) => new
        {
            execute = "clear_log",
            session,
            deviceInfo
        }, static response => response);

        /// <summary>
        /// 添加IP封禁请求
        /// </summary>
        /// <returns></returns>
        public XFEClientRequesterBuilder AddBannedIPRequest() => xFEClientRequesterBuilder.AddRequest("get_bannedIPList", static (session, deviceInfo, _) => new
        {
            execute = "get_bannedIPList",
            session,
            deviceInfo
        }, static response => JsonSerializer.Deserialize<List<IPAddressInfo>>(response, JsonSerializerOptions)!).AddRequest("add_bannedIP", static (session, deviceInfo, parameters) => new
        {
            execute = "add_bannedIP",
            session,
            deviceInfo,
            bannedIP = parameters[0],
            notes = parameters[1]
        }, static response => response).AddRequest("remove_bannedIP", static (session, deviceInfo, parameters) => new
        {
            execute = "remove_bannedIP",
            session,
            deviceInfo,
            bannedIP = parameters[0]
        }, static response => bool.Parse(response));

        /// <summary>
        /// 添加登录服务
        /// </summary>
        /// <typeparam name="T">登录返回用户接口类型</typeparam>
        /// <returns></returns>
        public XFEClientRequesterBuilder AddLoginRequest<T>() where T : IUserFaceInfo => xFEClientRequesterBuilder.AddRequest<LoginRequestService<T>>("login").AddRequest<ReloginRequestService<T>>("relogin");

        /// <summary>
        /// 使用XFE标准服务器服务请求
        /// </summary>
        /// <typeparam name="T">登录返回用户接口类型</typeparam>
        /// <returns></returns>
        public XFEClientRequesterBuilder UseXFEStandardRequest<T>() where T : IUserFaceInfo => xFEClientRequesterBuilder.AddLoginRequest<T>()
            .AddBannedIPRequest()
            .AddLogRequest()
            .AddCheckConnectRequest();
    }
}
