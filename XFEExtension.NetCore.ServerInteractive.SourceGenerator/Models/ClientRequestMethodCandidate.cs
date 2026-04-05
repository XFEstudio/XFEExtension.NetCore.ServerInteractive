namespace XFEExtension.NetCore.ServerInteractive.SourceGenerator.Models;

/// <summary>
/// 客户端请求方法候选信息，包含验证所需的所有数据
/// </summary>
public class ClientRequestMethodCandidate(string namespaceName, string className, string methodName, string path, string? name, bool isRequest, bool isContainingTypePartial, int parameterCount, bool hasValidReturnType, string returnTypeName, LocationInfo methodLocation, LocationInfo classLocation, string typeParameters, string typeConstraints, string attributeName, string[] usingDirectives)
{
    public string Namespace { get; } = namespaceName;
    public string ClassName { get; } = className;
    public string MethodName { get; } = methodName;
    public string Path { get; } = path;
    public string? Name { get; } = name;
    public bool IsRequest { get; } = isRequest;
    public bool IsContainingTypePartial { get; } = isContainingTypePartial;
    public int ParameterCount { get; } = parameterCount;
    public bool HasValidReturnType { get; } = hasValidReturnType;
    public string ReturnTypeName { get; } = returnTypeName;
    public LocationInfo MethodLocation { get; } = methodLocation;
    public LocationInfo ClassLocation { get; } = classLocation;
    public string TypeParameters { get; } = typeParameters;
    public string TypeConstraints { get; } = typeConstraints;
    public string AttributeName { get; } = attributeName;
    public string[] UsingDirectives { get; } = usingDirectives;
}
