using System.Text.Json;
using Box.V2;
using Box.V2.Config;
using Box.V2.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async (context) =>
{
    var log = "";
    try
    {
        Console.WriteLine($"QueryString = {context.Request.QueryString}");
        log += $"QueryString = {context.Request.QueryString}<br>\n";
        
        var clientId = context.Request.Query["clientId"];
        var clientSecret = context.Request.Query["clientSecret"];
        var redirect = context.Request.Query["redirect"];
        var code = context.Request.Query["code"];
        var fileId = context.Request.Query["fileId"];
        
        Console.WriteLine($"clientId = {clientId}");
        Console.WriteLine($"clientSecret = {clientSecret}");
        Console.WriteLine($"redirect = {redirect}");
        Console.WriteLine($"code = {code}");
        Console.WriteLine($"fileId = {fileId}");
        
        log += $"clientId = {clientId}<br>\n";
        log += $"clientSecret = {clientSecret}<br>\n";
        log += $"redirect = {redirect}<br>\n";
        log += $"code = {code}<br>\n";
        log += $"fileId = {fileId}<br>\n";
        
        var redirectUri = new Uri(redirect);
        var config = new BoxConfigBuilder(clientId, clientSecret, redirectUri).Build();
        var sdk = new BoxClient(config);

        var session = await sdk.Auth.AuthenticateAsync(code);
        var client = new BoxClient(config, session);

        // ユーザー情報取得
        var me = await client.UsersManager.GetCurrentUserInformationAsync();
        var meJson = JsonSerializer.Serialize(me);
        Console.WriteLine(meJson);
        
        log += $"user = <br><pre>{meJson}</pre><br>\n";
        
        // ファイル更新 1
        var now = DateTime.Now.ToLongTimeString();
        var param = new BoxFileRequest()
        {
            Id = fileId,
            // Tags = new string []{ "TrustSeal" },
            Name = now
        };
        var file = await client.FilesManager.UpdateInformationAsync(param);
        var fileJson = JsonSerializer.Serialize(file);
        Console.WriteLine(fileJson);
        
        log += $"file = <br><pre>{fileJson}</pre><br>\n";
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        log += "<b>Exception</b><br>\n";
        log += e;
    }

    await context.Response.WriteAsync(log);
});

app.Run();