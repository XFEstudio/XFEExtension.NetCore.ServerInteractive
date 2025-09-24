namespace XFEExtension.NetCore.ServerInteractive.Interfaces;

/// <summary>
/// 参数服务接口
/// </summary>
public interface IParameterService
{
    /// <summary>
    /// 参数字典
    /// </summary>
    Dictionary<string, object> ParameterDictionary { get; set; }

    /// <summary>
    /// 添加参数列表
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="value">数值</param>
    /// <returns></returns>
    IParameterService AddParameter(string name, object value)
    {
        ParameterDictionary.Add(name, value);
        return this;
    }

    /// <summary>
    /// 添加参数列表
    /// </summary>
    /// <param name="keyValuePairList"></param>
    /// <returns></returns>
    IParameterService AddParameterList(params KeyValuePair<string, object>[] keyValuePairList)
    {
        foreach (var keyValuePair in keyValuePairList)
            ParameterDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        return this;
    }

    /// <summary>
    /// 应用参数
    /// </summary>
    /// <param name="service"></param>
    void ApplyParameter(object service) => ApplyParameter(ParameterDictionary, service);

    /// <summary>
    /// 应用参数
    /// </summary>
    /// <param name="serviceParameterCacheDictionary"></param>
    /// <param name="service"></param>
    public static void ApplyParameter(Dictionary<string, object> serviceParameterCacheDictionary, object service)
    {
        var parameterList = service.GetType().GetProperties();
        foreach (var parameter in serviceParameterCacheDictionary)
        {
            foreach (var targetParameter in parameterList)
            {
                if (parameter.Key == targetParameter.Name)
                {
                    targetParameter.SetValue(service, parameter.Value);
                }
            }
        }
    }
}
