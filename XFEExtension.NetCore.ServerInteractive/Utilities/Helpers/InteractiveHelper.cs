using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

/// <summary>
/// 服务器交互帮助类
/// </summary>
public static class InteractiveHelper
{
    /// <summary>
    /// 获取响应
    /// </summary>
    /// <param name="requestAddress">请求地址</param>
    /// <param name="postBody">请求体</param>
    /// <returns></returns>
    public static async Task<(string, HttpStatusCode)> GetServerResponse(string requestAddress, string postBody)
    {
        using var client = new HttpClient();
        var result = await client.PostAsync(requestAddress, new StringContent(postBody));
        try
        {
            return (await result.Content.ReadAsStringAsync(), result.StatusCode);
        }
        catch (Exception ex)
        {
            return (ex.Message, result.StatusCode);
        }
    }

    /// <summary>
    /// 获取响应
    /// </summary>
    /// <param name="requestAddress">请求地址</param>
    /// <param name="postBody">请求体对象</param>
    /// <param name="jsonSerializerOptions">序列化参数</param>
    /// <returns></returns>
    public static async Task<(string, HttpStatusCode)> GetServerResponse(string requestAddress, object postBody, JsonSerializerOptions jsonSerializerOptions) => await GetServerResponse(requestAddress, JsonSerializer.Serialize(postBody, jsonSerializerOptions));

    /// <summary>
    /// 获取Stopwatch的时间字符串
    /// </summary>
    /// <param name="stopwatch"></param>
    /// <returns></returns>
    public static string GetStopWatchTime(Stopwatch stopwatch)
    {
        if (stopwatch.Elapsed.TotalSeconds > 1)
        {
            return $"{stopwatch.Elapsed.TotalSeconds:F1} s";
        }
        else if (stopwatch.Elapsed.TotalMilliseconds > 1)
        {
            return $"{stopwatch.Elapsed.TotalMilliseconds:F1} ms";
        }
        else if (stopwatch.Elapsed.TotalMicroseconds > 1)
        {
            return $"{stopwatch.Elapsed.TotalMicroseconds:F1} μs";
        }
        else
        {
            return $"{stopwatch.Elapsed.TotalNanoseconds:F1} ns";
        }
    }
}
