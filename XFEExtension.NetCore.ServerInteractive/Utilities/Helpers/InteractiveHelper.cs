using System.Net;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

public static class InteractiveHelper
{
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
}
