using XFEExtension.NetCore.ServerInteractive.Interfaces;

namespace XFEExtension.NetCore.ServerInteractive.Implements;

/// <summary>
/// 构建器基类
/// </summary>
public abstract class XFEBuilderBase : IParameterService
{
    /// <inheritdoc/>
    public Dictionary<string, object> ParameterDictionary { get; set; } = [];

    IParameterService IParameterService.AddParameter(string name, object value)
    {
        
    }
}
