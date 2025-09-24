namespace XFEExtension.NetCore.ServerInteractive.Interfaces;

/// <summary>
/// 参数服务接口基类
/// </summary>
public interface IParameterServiceBase
{
    /// <summary>
    /// 参数字典
    /// </summary>
    Dictionary<string, object> ParameterDictionary { get; set; }

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
