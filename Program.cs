using System.Text.Json;
using Box.V2;
using Box.V2.Config;
using Box.V2.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async (context) =>
{
    var clientId = context.Request.Query["clientId"];
    var clientSecret = context.Request.Query["clientSecret"];
    var redirect = context.Request.Query["redirect"];
    var code = context.Request.Query["code"];
    Console.WriteLine($"clientId = {clientId}");
    Console.WriteLine($"clientSecret = {clientSecret}");
    Console.WriteLine($"redirect = {redirect}");
    Console.WriteLine($"code = {code}");
    
    var redirectUri = new Uri(redirect);
    var config = new BoxConfigBuilder(clientId, clientSecret, redirectUri).Build();
    var sdk = new BoxClient(config);
    
    var session = await sdk.Auth.AuthenticateAsync(code);
    var client = new BoxClient(config, session);
    
    var me = await client.UsersManager.GetCurrentUserInformationAsync();
    var meJson = JsonSerializer.Serialize(me);
    Console.WriteLine(meJson);
    
    await context.Response.WriteAsync(meJson);
});

app.Run();
