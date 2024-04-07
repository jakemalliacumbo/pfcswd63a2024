using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;

namespace MyHttpFunction;

public class DemoFunction1 : IHttpFunction
{
    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(HttpContext context)
    {
        string blogId = context.Request.Query["blogId"];

        var storage = StorageClient.Create();
        storage.DeleteObject("pfc-jmc-2024-fg", blogId + ".pdf");

        await context.Response.WriteAsync("File was deleted successfully.");
    }
}
