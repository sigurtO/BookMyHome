var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/", () => "Gateway up. Use POST /auth/register and /auth/login");

// ✅ Rewrite inside the proxy pipeline (after route match, before forwarding)
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use((ctx, next) =>
    {
        if (ctx.Request.Path.StartsWithSegments("/auth", out var rest))
            ctx.Request.Path = rest; // /auth/register -> /register
        return next();
    });
});

app.Run();

