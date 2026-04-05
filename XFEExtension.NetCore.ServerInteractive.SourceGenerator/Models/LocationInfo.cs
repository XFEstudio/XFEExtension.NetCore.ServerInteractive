using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace XFEExtension.NetCore.ServerInteractive.SourceGenerator.Models;

/// <summary>
/// 轻量级位置信息，避免在增量生成器缓存中持有SyntaxTree引用
/// </summary>
public readonly struct LocationInfo(string filePath, TextSpan textSpan, LinePositionSpan lineSpan)
{
    public string FilePath { get; } = filePath;
    public TextSpan TextSpan { get; } = textSpan;
    public LinePositionSpan LineSpan { get; } = lineSpan;

    public static LocationInfo From(Location location)
    {
        var mappedSpan = location.GetMappedLineSpan();
        return new LocationInfo(
            mappedSpan.Path,
            location.SourceSpan,
            mappedSpan.Span);
    }

    public Location ToLocation()
    {
        return Location.Create(FilePath, TextSpan, LineSpan);
    }
}