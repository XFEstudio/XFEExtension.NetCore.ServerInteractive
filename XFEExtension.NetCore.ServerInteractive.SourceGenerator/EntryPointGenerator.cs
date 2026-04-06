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
/// 次级入口点增量生成器
/// 用于自动生成IServerCoreStandardService的入口点字典
/// </summary>
[Generator]
public class EntryPointGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor NonPartialClassRule = new(
        id: "XFE0003",
        title: "包含EntryPoint方法的类必须为partial",
        messageFormat: "类'{0}'必须声明为partial以便增量生成器可以生成入口点代码",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0003",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MethodMustBeParameterlessRule = new(
        id: "XFE0004",
        title: "EntryPoint方法不能有参数",
        messageFormat: "方法'{0}'标记了[EntryPoint]但包含参数，入口点方法必须是无参数的",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0004",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidReturnTypeRule = new(
        id: "XFE0005",
        title: "EntryPoint方法返回类型无效",
        messageFormat: "方法'{0}'的返回类型'{1}'无效，入口点方法必须返回void或Task",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0005",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidPathCharactersRule = new(
        id: "XFE0006",
        title: "EntryPoint路径包含无效字符",
        messageFormat: "入口点路径'{0}'包含无效字符（引号或反斜杠），这些字符不允许在路径中使用",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0006",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor DuplicatePathRule = new(
        id: "XFE0012",
        title: "EntryPoint路径重复注册",
        messageFormat: "入口点路径'{0}'在类'{1}'中重复注册，每个路径只能对应一个处理方法",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0012",
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidWildcardUsageRule = new(
        id: "XFE0013",
        title: "EntryPoint通配符使用无效",
        messageFormat: "入口点路径'{0}'中的通配符'*'必须作为完整的路径段使用（例如：v1/*/test），不能与其他字符混合（例如：v1/a*b）",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        helpLinkUri: "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFE0013",
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 找到所有标记了EntryPointAttribute的方法
        var methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateMethod(s),
                transform: static (ctx, _) => GetMethodsForGeneration(ctx))
            .Where(static m => m is { IsDefault: false, Length: > 0 })
            .SelectMany(static (m, _) => m);

        // 按类分组
        var compilationAndMethods = context.CompilationProvider.Combine(methodDeclarations.Collect());

        // 生成源代码
        context.RegisterSourceOutput(compilationAndMethods,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsCandidateMethod(SyntaxNode node) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };

    private static ImmutableArray<MethodCandidate> GetMethodsForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return default;

        // 获取所有EntryPointAttribute
        var entryPointAttributes = methodSymbol.GetAttributes()
            .Where(a => a.AttributeClass?.Name == "EntryPointAttribute")
            .ToList();

        if (entryPointAttributes.Count == 0)
            return default;

        // 检查返回类型：根据返回类型（而非async关键字）判断同步/异步
        // 注意：Task和Task<T>都是有效的异步返回类型（Task<T>可通过协变赋值给Func<Task>）
        var returnType = methodSymbol.ReturnType;
        var isVoid = returnType.SpecialType == SpecialType.System_Void;
        var isTaskLike = returnType.Name == "Task" &&
                         returnType.ContainingNamespace?.ToDisplayString() == "System.Threading.Tasks";
        var hasValidReturnType = isVoid || isTaskLike;
        var isAsync = isTaskLike;

        // 检查包含类型是否为partial
        var classDeclaration = methodDeclaration.Parent as ClassDeclarationSyntax;
        var isContainingTypePartial = classDeclaration?.Modifiers.Any(SyntaxKind.PartialKeyword) ?? false;

        // 获取泛型类型参数和约束（从语法节点获取以保留原始文本，并去除多余空白）
        var typeParameters = classDeclaration?.TypeParameterList?.ToString().Trim() ?? "";
        var typeConstraints = classDeclaration?.ConstraintClauses.ToString().Trim() ?? "";

        var containingType = methodSymbol.ContainingType;

        // 获取位置信息（使用轻量结构避免在增量缓存中持有SyntaxTree引用）
        var methodLocation = LocationInfo.From(methodDeclaration.GetLocation());
        var classLocation = classDeclaration is not null
            ? LocationInfo.From(classDeclaration.Identifier.GetLocation())
            : methodLocation;

        var results = new List<MethodCandidate>();

        foreach (var attr in entryPointAttributes)
        {
            var path = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
            // 空路径或 "*" 均视为全匹配通配符
            if (string.IsNullOrEmpty(path))
                path = "*";

            results.Add(new MethodCandidate(
                containingType.ContainingNamespace.ToDisplayString(),
                containingType.Name,
                methodSymbol.Name,
                path!,
                isAsync,
                isContainingTypePartial,
                methodSymbol.Parameters.Length,
                hasValidReturnType,
                returnType.ToDisplayString(),
                methodLocation,
                classLocation,
                typeParameters,
                typeConstraints));
        }

        return results.Count == 0 ? default : ImmutableArray.CreateRange(results);
    }

    private static void Execute(Compilation compilation, ImmutableArray<MethodCandidate> methods, SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
            return;

        var validMethods = new List<MethodCandidate>();

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
                    method.MethodName));
                isValid = false;
            }

            // 校验：返回类型必须为void或Task
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

            // 校验：通配符 '*' 必须作为完整路径段使用
            if (method.Path.Contains("*") && method.Path != "*")
            {
                var segments = method.Path.Split('/');
                foreach (var segment in segments)
                {
                    if (segment.Contains("*") && segment != "*")
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidWildcardUsageRule,
                            method.MethodLocation.ToLocation(),
                            method.Path));
                        isValid = false;
                        break;
                    }
                }
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

            // 检查重复路径（路径相同的任意两个方法都报错，无论同步/异步）
            var hasDuplicateError = false;
            var pathToMethods = new Dictionary<string, List<MethodCandidate>>();
            foreach (var method in methodInfos)
            {
                if (!pathToMethods.TryGetValue(method.Path, out var list))
                {
                    list = new List<MethodCandidate>();
                    pathToMethods[method.Path] = list;
                }
                list.Add(method);
            }

            foreach (var kvp in pathToMethods)
            {
                if (kvp.Value.Count <= 1)
                    continue;

                hasDuplicateError = true;
                foreach (var dup in kvp.Value)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DuplicatePathRule,
                        dup.MethodLocation.ToLocation(),
                        kvp.Key,
                        className));
                }
            }

            if (hasDuplicateError)
                continue;

            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine($@"// <auto-generated/>
#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace {namespaceName}
{{
    /// <summary>
    /// {className}的自动生成入口点字典部分类
    /// </summary>
    public partial class {className}{typeParameters}{constraintsSuffix}
    {{
        /// <summary>
        /// 本类型的入口点路径列表（覆盖基类的空列表）
        /// </summary>
        public override List<string> EntryPointList {{ get; }} = new()
        {{");

            // 添加所有入口点到静态列表
            foreach (var method in methodInfos)
            {
                sourceBuilder.AppendLine($"            \"{EscapeStringLiteral(method.Path)}\",");
            }

            sourceBuilder.AppendLine($@"        }};

        private Dictionary<string, Action>? _generatedSyncEntryPoints;
        /// <inheritdoc/>
        public override Dictionary<string, Action> SyncEntryPoints
        {{
            get => _generatedSyncEntryPoints ??= new Dictionary<string, Action>()
            {{");

            // 添加同步入口点
            foreach (var method in methodInfos.Where(m => !m.IsAsync))
            {
                sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(method.Path)}\", {method.MethodName} }},");
            }

            sourceBuilder.AppendLine($@"            }};
        }}

        private Dictionary<string, Func<Task>>? _generatedAsyncEntryPoints;
        /// <inheritdoc/>
        public override Dictionary<string, Func<Task>> AsyncEntryPoints
        {{
            get => _generatedAsyncEntryPoints ??= new Dictionary<string, Func<Task>>()
            {{");

            // 添加异步入口点
            foreach (var method in methodInfos.Where(m => m.IsAsync))
            {
                sourceBuilder.AppendLine($"                {{ \"{EscapeStringLiteral(method.Path)}\", {method.MethodName} }},");
            }

            sourceBuilder.AppendLine(@"            };
        }
    }
}");

            context.AddSource($"{className}.EntryPoints.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }

    /// <summary>
    /// 转义字符串字面量中的特殊字符
    /// </summary>
    private static string EscapeStringLiteral(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
