using System;
using System.Collections.Generic;
using System.Text;

namespace XFEExtension.NetCore.ServerInteractive.SourceGenerator.Models;

/// <summary>
/// 方法候选信息，包含验证所需的所有数据
/// </summary>
public class MethodCandidate(string namespaceName, string className, string methodName, string path, bool isAsync, bool isContainingTypePartial, int parameterCount, bool hasValidReturnType, string returnTypeName, LocationInfo methodLocation, LocationInfo classLocation, string typeParameters, string typeConstraints)
{
    public string Namespace { get; } = namespaceName;
    public string ClassName { get; } = className;
    public string MethodName { get; } = methodName;
    public string Path { get; } = path;
    public bool IsAsync { get; } = isAsync;
    public bool IsContainingTypePartial { get; } = isContainingTypePartial;
    public int ParameterCount { get; } = parameterCount;
    public bool HasValidReturnType { get; } = hasValidReturnType;
    public string ReturnTypeName { get; } = returnTypeName;
    public LocationInfo MethodLocation { get; } = methodLocation;
    public LocationInfo ClassLocation { get; } = classLocation;
    public string TypeParameters { get; } = typeParameters;
    public string TypeConstraints { get; } = typeConstraints;
}