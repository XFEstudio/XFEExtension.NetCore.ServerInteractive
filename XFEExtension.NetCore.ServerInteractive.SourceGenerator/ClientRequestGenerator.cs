using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using XFEExtension.NetCore.ServerInteractive.SourceGenerator.Models;

namespace XFEExtension.NetCore.ServerInteractive.SourceGenerator;

/// <summary>
/// 客户端请求增量生成器
/// 用于自动生成StandardRequestServiceBase的请求/响应字典
/// </summary>
[Generator]
public class ClientRequestGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor NonPartialClassRule = new(
        id: "XFE0007",
        title: "包含Request或Response方法的类必须为partial",
        messageFormat: "类'{0}'必须声明为partial以便增量生成器可以生成请求/响应代码",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0007",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MethodMustBeParameterlessRule = new(
        id: "XFE0008",
        title: "Request/Response方法不能有参数",
        messageFormat: "方法'{0}'标记了[{1}]但包含参数，请求/响应方法必须是无参数的",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0008",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidReturnTypeRule = new(
        id: "XFE0009",
        title: "Request/Response方法返回类型无效",
        messageFormat: "方法'{0}'的返回类型'{1}'无效，请求/响应方法必须返回object",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0009",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidPathCharactersRule = new(
        id: "XFE0010",
        title: "Request/Response路径包含无效字符",
        messageFormat: "路径'{0}'包含无效字符（引号或反斜杠），这些字符不允许在路径中使用",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0010",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor DuplicatePathRule = new(
        id: "XFE0011",
        title: "Request/Response路径重复注册",
        messageFormat: "路径或名称'{0}'在类'{1}'的[{2}]方法中重复注册",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0011",
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 找到所有标记了RequestAttribute或ResponseAttribute的方法
        var methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateMethod(s),
                transform: static (ctx, _) => GetMethodsForGeneration(ctx))
            .Where(static m => !m.IsDefault && m.Length > 0)
            .SelectMany(static (m, _) => m);

        // 按类分组
        var compilationAndMethods = context.CompilationProvider.Combine(methodDeclarations.Collect());

        // 生成源代码
        context.RegisterSourceOutput(compilationAndMethods,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsCandidateMethod(SyntaxNode node) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };

    private static ImmutableArray<ClientRequestMethodCandidate> GetMethodsForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return default;

        var attributes = methodSymbol.GetAttributes();
        var requestAttributes = attributes.Where(a => a.AttributeClass?.Name == "RequestAttribute").ToList();
        var responseAttributes = attributes.Where(a => a.AttributeClass?.Name == "ResponseAttribute").ToList();

        if (requestAttributes.Count == 0 && responseAttributes.Count == 0)
            return default;

        // 共有信息
        var returnType = methodSymbol.ReturnType;
        var isObjectReturn = returnType.SpecialType == SpecialType.System_Object;

        var classDeclaration = methodDeclaration.Parent as ClassDeclarationSyntax;
        var isContainingTypePartial = classDeclaration?.Modifiers.Any(SyntaxKind.PartialKeyword) ?? false;

        var typeParameters = classDeclaration?.TypeParameterList?.ToString().Trim() ?? "";
        var typeConstraints = classDeclaration?.ConstraintClauses.ToString().Trim() ?? "";

        var containingType = methodSymbol.ContainingType;

        var methodLocation = LocationInfo.From(methodDeclaration.GetLocation());
        var classLocation = classDeclaration is not null
            ? LocationInfo.From(classDeclaration.Identifier.GetLocation())
            : methodLocation;

        var compilationUnit = methodDeclaration.SyntaxTree.GetRoot() as CompilationUnitSyntax;
        var usingDirectives = compilationUnit?.Usings
            .Select(u => u.ToString().Trim())
            .ToArray() ?? System.Array.Empty<string>();

        var results = new List<ClientRequestMethodCandidate>();

        // 为每个Request属性创建候选
        foreach (var attr in requestAttributes)
        {
            var path = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
            if (string.IsNullOrEmpty(path))
                continue;

            var name = (string?)null;
            if (!attr.NamedArguments.IsDefaultOrEmpty)
            {
                var nameArg = attr.NamedArguments.FirstOrDefault(kvp => kvp.Key == "Name");
                if (nameArg.Key is not null)
                    name = nameArg.Value.Value?.ToString();
            }

            results.Add(new ClientRequestMethodCandidate(
                containingType.ContainingNamespace.ToDisplayString(),
                containingType.Name,
                methodSymbol.Name,
                path!,
                name,
                isRequest: true,
                isContainingTypePartial,
                methodSymbol.Parameters.Length,
                isObjectReturn,
                returnType.ToDisplayString(),
                methodLocation,
                classLocation,
                typeParameters,
                typeConstraints,
                "Request",
                usingDirectives));
        }

        // 为每个Response属性创建候选
        foreach (var attr in responseAttributes)
        {
            var path = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
            if (string.IsNullOrEmpty(path))
                continue;

            var name = (string?)null;
            if (!attr.NamedArguments.IsDefaultOrEmpty)
            {
                var nameArg = attr.NamedArguments.FirstOrDefault(kvp => kvp.Key == "Name");
                if (nameArg.Key is not null)
                    name = nameArg.Value.Value?.ToString();
            }

            results.Add(new ClientRequestMethodCandidate(
                containingType.ContainingNamespace.ToDisplayString(),
                containingType.Name,
                methodSymbol.Name,
                path!,
                name,
                isRequest: false,
                isContainingTypePartial,
                methodSymbol.Parameters.Length,
                isObjectReturn,
                returnType.ToDisplayString(),
                methodLocation,
                classLocation,
                typeParameters,
                typeConstraints,
                "Response",
                usingDirectives));
        }

        if (results.Count == 0)
            return default;

        return results.ToImmutableArray();
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClientRequestMethodCandidate> methods, SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
            return;

        var validMethods = new List<ClientRequestMethodCandidate>();

        // 校验并报告诊断信息
        foreach (var method in methods)
        {
            var isValid = true;

            // 校验：包含类型必须为partial
            if (!method.IsContainingTypePartial)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    NonPartialClassRule,
                    method.ClassLocation.ToLocation(),
                    method.ClassName));
                isValid = false;
            }

            // 校验：方法不能有参数
            if (method.ParameterCount > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    MethodMustBeParameterlessRule,
                    method.MethodLocation.ToLocation(),
                    method.MethodName,
                    method.AttributeName));
                isValid = false;
            }

            // 校验：返回类型必须为object
            if (!method.HasValidReturnType)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    InvalidReturnTypeRule,
                    method.MethodLocation.ToLocation(),
                    method.MethodName,
                    method.ReturnTypeName));
                isValid = false;
            }

            // 校验：路径不能包含引号或反斜杠
            if (method.Path.Contains("\"") || method.Path.Contains("\\"))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    InvalidPathCharactersRule,
                    method.MethodLocation.ToLocation(),
                    method.Path));
                isValid = false;
            }

            if (isValid)
            {
                validMethods.Add(method);
            }
        }

        if (validMethods.Count == 0)
            return;

        // 按类分组并生成代码
        var methodsByClass = validMethods.GroupBy(m => (m.Namespace, m.ClassName, m.TypeParameters, m.TypeConstraints));

        foreach (var group in methodsByClass)
        {
            var (namespaceName, className, typeParameters, typeConstraints) = group.Key;
            var methodInfos = group.ToList();
            var constraintsSuffix = string.IsNullOrEmpty(typeConstraints) ? "" : $" {typeConstraints}";

            var requestMethods = methodInfos.Where(m => m.IsRequest).ToList();
            var responseMethods = methodInfos.Where(m => !m.IsRequest).ToList();

            // 检查重复路径/名称
            var hasDuplicateError = false;

            hasDuplicateError |= CheckDuplicateKeys(context, requestMethods, className, "Request");
            hasDuplicateError |= CheckDuplicateKeys(context, responseMethods, className, "Response");

            if (hasDuplicateError)
                continue;

            // 收集所有路径（去重）— 仅包含实际路由路径
            var allPaths = methodInfos.Select(m => m.Path).Distinct().ToList();

            // 收集所有using指令（合并去重）
            var allUsings = methodInfos
                .SelectMany(m => m.UsingDirectives)
                .Distinct()
                .OrderBy(u => u)
                .ToList();

            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine(@"// <auto-generated/>
#nullable enable
");

            // 添加using指令
            foreach (var usingDirective in allUsings)
            {
                sourceBuilder.AppendLine(usingDirective);
            }

            // 确保基础using存在
            if (!allUsings.Any(u => u.Trim() == "using System;"))
                sourceBuilder.AppendLine("using System;");
            if (!allUsings.Any(u => u.Contains("System.Collections.Generic")))
                sourceBuilder.AppendLine("using System.Collections.Generic;");

            sourceBuilder.AppendLine($@"
namespace {namespaceName}
{{
    /// <summary>
    /// {className}的自动生成请求/响应字典部分类
    /// </summary>
    public partial class {className}{typeParameters}{constraintsSuffix}
    {{
        /// <summary>
        /// 本类型的请求路由路径列表（覆盖基类的空列表）
        /// </summary>
        public new static List<string> RequestRouteList {{ get; }} = new()
        {{");

            // 添加所有路径到静态列表
            foreach (var path in allPaths)
            {
                sourceBuilder.AppendLine($"            \"{EscapeStringLiteral(path)}\",");
            }

            sourceBuilder.AppendLine($@"        }};

        private Dictionary<string, Func<object>>? _generatedRequestPoints;
        /// <inheritdoc/>
        public override Dictionary<string, Func<object>> RequestPoints
        {{
            get => _generatedRequestPoints ??= new Dictionary<string, Func<object>>()
            {{");

            // 添加请求方法（路径 → 方法 + 名称 → 方法）
            foreach (var method in requestMethods)
            {
                sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(method.Path)}\", {method.MethodName} }},");
                if (!string.IsNullOrEmpty(method.Name))
                {
                    sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(method.Name!)}\", {method.MethodName} }},");
                }
            }

            sourceBuilder.AppendLine($@"            }};
        }}

        private Dictionary<string, Func<object>>? _generatedResponsePoints;
        /// <inheritdoc/>
        public override Dictionary<string, Func<object>> ResponsePoints
        {{
            get => _generatedResponsePoints ??= new Dictionary<string, Func<object>>()
            {{");

            // 添加响应方法（路径 → 方法 + 名称 → 方法）
            foreach (var method in responseMethods)
            {
                sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(method.Path)}\", {method.MethodName} }},");
                if (!string.IsNullOrEmpty(method.Name))
                {
                    sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(method.Name!)}\", {method.MethodName} }},");
                }
            }

            sourceBuilder.AppendLine($@"            }};
        }}

        private Dictionary<string, string>? _generatedRequestRouteMap;
        /// <inheritdoc/>
        public override Dictionary<string, string> RequestRouteMap
        {{
            get => _generatedRequestRouteMap ??= new Dictionary<string, string>()
            {{");

            // 构建路由映射（所有键 → 实际路径）
            var allMappings = new HashSet<string>();
            foreach (var method in methodInfos)
            {
                var pathKey = method.Path;
                if (allMappings.Add(pathKey))
                {
                    sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(pathKey)}\", \"{EscapeStringLiteral(pathKey)}\" }},");
                }
                if (!string.IsNullOrEmpty(method.Name) && allMappings.Add(method.Name!))
                {
                    sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(method.Name!)}\", \"{EscapeStringLiteral(pathKey)}\" }},");
                }
            }

            sourceBuilder.AppendLine(@"            };
        }
    }
}");

            context.AddSource($"{className}.ClientRequests.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }

    /// <summary>
    /// 检查同类型（Request或Response）候选中是否存在重复的路径或名称键
    /// </summary>
    private static bool CheckDuplicateKeys(SourceProductionContext context, List<ClientRequestMethodCandidate> candidates, string className, string attributeName)
    {
        var seenKeys = new HashSet<string>();
        var hasDuplicate = false;

        foreach (var candidate in candidates)
        {
            if (!seenKeys.Add(candidate.Path))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DuplicatePathRule,
                    candidate.MethodLocation.ToLocation(),
                    candidate.Path,
                    className,
                    attributeName));
                hasDuplicate = true;
            }

            if (!string.IsNullOrEmpty(candidate.Name) && !seenKeys.Add(candidate.Name!))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DuplicatePathRule,
                    candidate.MethodLocation.ToLocation(),
                    candidate.Name!,
                    className,
                    attributeName));
                hasDuplicate = true;
            }
        }

        return hasDuplicate;
    }

    /// <summary>
    /// 转义字符串字面量中的特殊字符
    /// </summary>
    private static string EscapeStringLiteral(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
