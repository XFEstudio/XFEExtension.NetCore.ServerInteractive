namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

/// <summary>
/// 路由通配符匹配辅助类
/// </summary>
internal static class RouteMatchHelper
{
    /// <summary>
    /// 判断路由是否包含通配符
    /// </summary>
    /// <param name="pattern">路由模式</param>
    /// <returns>是否包含通配符</returns>
    public static bool IsWildcardRoute(string pattern) => pattern.Contains('*');

    /// <summary>
    /// 将路由模式与实际路由进行通配符匹配
    /// <para>支持的通配符：</para>
    /// <para>- <c>*</c>：匹配任意路径</para>
    /// <para>- <c>v1/*</c>：匹配 v1/ 后跟一个路径段</para>
    /// <para>- <c>v1/*/test</c>：匹配 v1/ 后跟任意一个段再跟 /test</para>
    /// </summary>
    /// <param name="pattern">包含通配符的路由模式</param>
    /// <param name="route">实际请求路由</param>
    /// <returns>是否匹配</returns>
    public static bool MatchWildcardRoute(string pattern, string route)
    {
        // 单独的 "*" 匹配所有路由
        if (pattern == "*")
            return true;

        var patternSegments = pattern.Split('/');
        var routeSegments = route.Split('/');

        if (patternSegments.Length != routeSegments.Length)
            return false;

        for (var i = 0; i < patternSegments.Length; i++)
        {
            if (patternSegments[i] == "*")
                continue;
            if (patternSegments[i] != routeSegments[i])
                return false;
        }

        return true;
    }
}
