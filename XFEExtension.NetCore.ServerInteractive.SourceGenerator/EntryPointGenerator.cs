using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace XFEExtension.NetCore.ServerInteractive.SourceGenerator;

/// <summary>
/// 次级入口点增量生成器
/// 用于自动生成IServerCoreStandardService的入口点字典
/// </summary>
[Generator]
public class EntryPointGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor NonPartialClassRule = new(
        id: "XFESI001",
        title: "包含EntryPoint方法的类必须为partial",
        messageFormat: "类'{0}'必须声明为partial以便增量生成器可以生成入口点代码",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MethodMustBeParameterlessRule = new(
        id: "XFESI002",
        title: "EntryPoint方法不能有参数",
        messageFormat: "方法'{0}'标记了[EntryPoint]但包含参数，入口点方法必须是无参数的",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidReturnTypeRule = new(
        id: "XFESI003",
        title: "EntryPoint方法返回类型无效",
        messageFormat: "方法'{0}'的返回类型'{1}'无效，入口点方法必须返回void或Task",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidPathCharactersRule = new(
        id: "XFESI004",
        title: "EntryPoint路径包含无效字符",
        messageFormat: "入口点路径'{0}'包含无效字符（引号或反斜杠），这些字符不允许在路径中使用",
        category: "XFEServerInteractive",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 找到所有标记了EntryPointAttribute的方法
        var methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateMethod(s),
                transform: static (ctx, _) => GetMethodForGeneration(ctx))
            .Where(static m => m is not null);

        // 按类分组
        var compilationAndMethods = context.CompilationProvider.Combine(methodDeclarations.Collect());

        // 生成源代码
        context.RegisterSourceOutput(compilationAndMethods,
            static (spc, source) => Execute(source.Left, source.Right!, spc));
    }

    private static bool IsCandidateMethod(SyntaxNode node)
    {
        return node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0;
    }

    private static MethodCandidate? GetMethodForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return null;

        // 检查是否有EntryPointAttribute
        var entryPointAttribute = methodSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "EntryPointAttribute");

        if (entryPointAttribute is null)
            return null;

        // 获取Path参数
        var path = entryPointAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
        if (string.IsNullOrEmpty(path))
            return null;

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

        return new MethodCandidate(
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
            typeConstraints);
    }

    private static void Execute(Compilation compilation, ImmutableArray<MethodCandidate?> methods, SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
            return;

        var validMethods = new List<MethodCandidate>();

        // 校验并报告诊断信息
        foreach (var method in methods)
        {
            if (method is null) continue;

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
        public new static List<string> EntryPointList {{ get; }} = new()
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

    /// <summary>
    /// 轻量级位置信息，避免在增量生成器缓存中持有SyntaxTree引用
    /// </summary>
    private readonly struct LocationInfo
    {
        public string FilePath { get; }
        public TextSpan TextSpan { get; }
        public LinePositionSpan LineSpan { get; }

        private LocationInfo(string filePath, TextSpan textSpan, LinePositionSpan lineSpan)
        {
            FilePath = filePath;
            TextSpan = textSpan;
            LineSpan = lineSpan;
        }

        public static LocationInfo From(Location location)
        {
            var mappedSpan = location.GetMappedLineSpan();
            return new LocationInfo(
                mappedSpan.Path ?? "",
                location.SourceSpan,
                mappedSpan.Span);
        }

        public Location ToLocation()
        {
            return Location.Create(FilePath, TextSpan, LineSpan);
        }
    }

    /// <summary>
    /// 方法候选信息，包含验证所需的所有数据
    /// </summary>
    private class MethodCandidate
    {
        public string Namespace { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public string Path { get; }
        public bool IsAsync { get; }
        public bool IsContainingTypePartial { get; }
        public int ParameterCount { get; }
        public bool HasValidReturnType { get; }
        public string ReturnTypeName { get; }
        public LocationInfo MethodLocation { get; }
        public LocationInfo ClassLocation { get; }
        public string TypeParameters { get; }
        public string TypeConstraints { get; }

        public MethodCandidate(string namespaceName, string className, string methodName, string path, bool isAsync,
            bool isContainingTypePartial, int parameterCount, bool hasValidReturnType, string returnTypeName,
            LocationInfo methodLocation, LocationInfo classLocation, string typeParameters, string typeConstraints)
        {
            Namespace = namespaceName;
            ClassName = className;
            MethodName = methodName;
            Path = path;
            IsAsync = isAsync;
            IsContainingTypePartial = isContainingTypePartial;
            ParameterCount = parameterCount;
            HasValidReturnType = hasValidReturnType;
            ReturnTypeName = returnTypeName;
            MethodLocation = methodLocation;
            ClassLocation = classLocation;
            TypeParameters = typeParameters;
            TypeConstraints = typeConstraints;
        }
    }
}
