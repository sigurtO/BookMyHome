using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration; // <-- required

var builder = WebApplication.CreateBuilder(args);

// In-memory YARP config → targets auth1 + auth2
var routes = new List<RouteConfig>
{
    new()
    {
        RouteId = "auth",
        ClusterId = "authCluster",
        Match = new RouteMatch { Path = "/auth/{**catch-all}" }
    }
};

var clusters = new List<ClusterConfig>
{
    new()
    {
        ClusterId = "authCluster",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            ["a1"] = new() { Address = "http://auth1:5000" },
            ["a2"] = new() { Address = "http://auth2:5000" }
        }
    }
};

builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);

var app = builder.Build();

app.MapGet("/", () => "Gateway (code-config) OK — /auth/* goes to auth1+auth2");
app.MapReverseProxy();

app.Run();
