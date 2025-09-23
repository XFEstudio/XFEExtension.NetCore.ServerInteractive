namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 请求结果接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRequestResult<T> : IRequestResultBase
{
    /// <summary>
    /// 结果
    /// </summary>
    new T Result { get; set; }
}
