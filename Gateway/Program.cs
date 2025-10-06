var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();
app.MapGet("/", () => "Gateway up. Use POST /auth/register and /auth/login");
app.MapReverseProxy();
app.Run();
