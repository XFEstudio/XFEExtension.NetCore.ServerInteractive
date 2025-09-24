using XFEExtension.NetCore.ServerInteractive.Interfaces;

namespace XFEExtension.NetCore.ServerInteractive.Implements;

/// <summary>
/// 构建器基类
/// </summary>
public abstract class XFEBuilderBase<T> : IParameterService<T>, IParameterServiceBase where T : XFEBuilderBase<T>
{
    /// <inheritdoc/>
    public Dictionary<string, object> ParameterDictionary { get; set; } = [];

    /// <inheritdoc/>
    public T AddParameter(string name, object value)
    {
        ParameterDictionary.Add(name, value);
        return (T)this;
    }

    /// <inheritdoc/>
    public T AddParameterList(params KeyValuePair<string, object>[] keyValuePairList)
    {
        foreach (var keyValuePair in keyValuePairList)
            AddParameter(keyValuePair.Key, keyValuePair.Value);
        return (T)this;
    }

    /// <inheritdoc/>
    public void ApplyParameter(object service) => IParameterServiceBase.ApplyParameter(ParameterDictionary, service);
}
