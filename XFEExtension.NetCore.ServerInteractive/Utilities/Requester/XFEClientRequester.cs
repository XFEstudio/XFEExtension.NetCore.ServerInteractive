using System.Net;
using System.Text.Json;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;
using XFEExtension.NetCore.StringExtension.Json;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

/// <summary>
/// XFE客户端请求器
/// </summary>
[CreateImpl]
public abstract class XFEClientRequester
{
    readonly JsonSerializerOptions jsonSerializerOptions = new();
    internal Dictionary<string, IRequestService> requestServiceDictionary = [];
    internal Dictionary<string, IXFERequestService> xFERequestServiceDictionary = [];
    /// <summary>
    /// 请求地址
    /// </summary>
    public string RequestAddress { get; set; } = string.Empty;
    /// <summary>
    /// 请求消息返回事件
    /// </summary>
    public event XFEEventHandler<object?, ServerInteractiveEventArgs>? MessageReceived;
    /// <summary>
    /// 表格请求器
    /// </summary>
    public TableRequester TableRequester { get; set; } = new();

    /// <summary>
    /// XFE客户端请求器
    /// </summary>
    public XFEClientRequester()
    {
        jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
        TableRequester.MessageReceived += TableRequester_MessageReceived;
    }

    private void TableRequester_MessageReceived(object? sender, ServerInteractiveEventArgs e) => MessageReceived?.Invoke(sender, e);

    /// <summary>
    /// 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="XFERequesterException"></exception>
    public async Task<T> Request<T>(string serviceName, params object[] parameters)
    {
        try
        {
            if (requestServiceDictionary.TryGetValue(serviceName, out var service))
            {
                return await service.Request<T>(parameters);
            }
            else if (xFERequestServiceDictionary.TryGetValue(serviceName, out var xFEService))
            {
                var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress, xFEService.PostRequest(parameters).ToJson());
                if (code == HttpStatusCode.OK)
                {
                    var result = await xFEService.AnalyzeResponse<T>(response);
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                    return result;
                }
                else
                {
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                    return default!;
                }
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return default!;
        }
        throw new XFERequesterException("请求的方法未注册");
    }
}
