using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;

/// <summary>
/// 客户端请求器构建器扩展
/// </summary>
public static class ClientRequesterBuilderExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new();

    static ClientRequesterBuilderExtensions() => JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <param name="xFEClientRequesterBuilder"></param>
    extension(ClientRequesterBuilder xFEClientRequesterBuilder)
    {
        /// <summary>
        /// 添加连接检查请求
        /// </summary>
        /// <returns></returns>
        public ClientRequesterBuilder AddCheckConnectRequest() => xFEClientRequesterBuilder.AddRequest("check_connect", static (_, _, _) => new
        {
            execute = "check_connect"
        }, response => DateTime.Parse(response));

        /// <summary>
        /// 添加日志请求
        /// </summary>
        /// <returns></returns>
        public ClientRequesterBuilder AddLogRequest() => xFEClientRequesterBuilder.AddRequest<LogRequestService>();

        /// <summary>
        /// 添加IP封禁请求
        /// </summary>
        /// <returns></returns>
        public ClientRequesterBuilder AddBannedIPRequest() => xFEClientRequesterBuilder.AddRequest("get_bannedIPList", static (session, deviceInfo, _) => new
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
        public ClientRequesterBuilder AddLoginRequest<T>() where T : IUserFaceInfo => xFEClientRequesterBuilder.AddRequest<UserRequestService<T>>();

        /// <summary>
        /// 使用XFE标准服务器服务请求
        /// </summary>
        /// <typeparam name="T">登录返回用户接口类型</typeparam>
        /// <returns></returns>
        public ClientRequesterBuilder UseXFEStandardRequest<T>() where T : IUserFaceInfo => xFEClientRequesterBuilder.AddLoginRequest<T>()
            .AddBannedIPRequest()
            .AddLogRequest()
            .AddCheckConnectRequest();
    }
}
