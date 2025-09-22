using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

/// <summary>
/// 构建器帮助类
/// </summary>
public static class BuilderHelper
{
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
