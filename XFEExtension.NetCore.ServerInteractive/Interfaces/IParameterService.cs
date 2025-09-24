namespace XFEExtension.NetCore.ServerInteractive.Interfaces;

/// <summary>
/// 参数服务接口
/// </summary>
public interface IParameterService<T> where T : IParameterServiceBase
{

    /// <summary>
    /// 添加参数列表
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="value">数值</param>
    /// <returns></returns>
    T AddParameter(string name, object value);

    /// <summary>
    /// 添加参数列表
    /// </summary>
    /// <param name="keyValuePairList"></param>
    /// <returns></returns>
    T AddParameterList(params KeyValuePair<string, object>[] keyValuePairList);

    /// <summary>
    /// 应用参数
    /// </summary>
    /// <param name="service"></param>
    protected void ApplyParameter(object service);
}
