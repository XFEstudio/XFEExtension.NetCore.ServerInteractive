using System.Net;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;

/// <summary>
/// 客户端请求结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class ClientRequestResult<T> : IRequestResult<T>
{
    /// <inheritdoc/>
    public T? Result { get; set; }
    /// <inheritdoc/>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    /// <inheritdoc/>
    public string Message { get; set; } = string.Empty;
    /// <inheritdoc/>
    object? IRequestResultBase.Result { get => Result; set => Result = value is not null ? (T)value : default; }

    /// <summary>
    /// 转换为指定泛型
    /// </summary>
    /// <typeparam name="TF">指定泛型</typeparam>
    /// <returns></returns>
    public ClientRequestResult<TF> TryConvertTo<TF>() => new()
    {
        Message = Message,
        StatusCode = StatusCode,
        Result = Result is not null ? (TF)(object)Result : default
    };
}
