namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 请求服务
/// </summary>
public interface IRequestService : IRequestServiceBase
{
    /// <summary>
    /// 请求
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task<T> Request<T>(params object[] parameters);
}
