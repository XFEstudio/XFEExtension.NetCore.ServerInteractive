using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 请求服务
/// </summary>
public interface IRequestService : IRequestServiceBase
{
    /// <summary>
    /// 请求
    /// </summary>
    /// <typeparam name="T">请求结果泛型</typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task<ClientRequestResult<T>> Request<T>(params object[] parameters);
}
