namespace XFEExtension.NetCore.ServerInteractive.Attributes;

/// <summary>
/// 次级入口点特性，用于标记IServerCoreStandardService中的处理方法
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class EntryPointAttribute : Attribute
{
    /// <summary>
    /// 次级入口点路径
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 创建次级入口点特性
    /// </summary>
    /// <param name="path">入口点路径（例如：user/login）</param>
    public EntryPointAttribute(string path)
    {
        Path = path;
    }
}
