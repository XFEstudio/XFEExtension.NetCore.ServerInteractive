using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 请求器基接口
/// </summary>
public interface IRequesterBase
{
    /// <summary>
    /// 请求地址
    /// </summary>
    string RequestAddress { get; set; }
    /// <summary>
    /// 本次会话Session
    /// </summary>
    string Session { get; set; }
    /// <summary>
    /// 电脑信息
    /// </summary>
    string ComputerInfo { get; set; }
    /// <summary>
    /// 请求消息返回事件
    /// </summary>
    public event XFEEventHandler<object?, ServerInteractiveEventArgs>? MessageReceived;
}
