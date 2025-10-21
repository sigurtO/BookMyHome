using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string? connString =
    builder.Configuration.GetConnectionString("Default")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
    ?? throw new InvalidOperationException("Missing ConnectionStrings__Default");

static string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}

async Task<IResult> Register(UserLogin input)
{
    await using var conn = new MySqlConnection(connString);
    await conn.OpenAsync();

    var hash = HashPassword(input.Password);

    await using var cmd = new MySqlCommand(
        "INSERT INTO Users (Username, PasswordHash) VALUES (@u, @p)", conn);
    cmd.Parameters.AddWithValue("@u", input.Username);
    cmd.Parameters.AddWithValue("@p", hash);

    await cmd.ExecuteNonQueryAsync();
    return Results.Ok("User registered!");
}

async Task<IResult> Login(UserLogin input)
{
    await using var conn = new MySqlConnection(connString);
    await conn.OpenAsync();

    await using var cmd = new MySqlCommand(
        "SELECT PasswordHash FROM Users WHERE Username=@u", conn);
    cmd.Parameters.AddWithValue("@u", input.Username);

    var result = await cmd.ExecuteScalarAsync();
    if (result is null) return Results.Unauthorized();

    var storedHash = Convert.ToString(result);
    var loginHash = HashPassword(input.Password);

    return storedHash == loginHash ? Results.Ok("Login successful!") : Results.Unauthorized();
}

app.MapGet("/auth/whoami", () => Results.Text(Environment.GetEnvironmentVariable("INSTANCE") ?? Environment.MachineName, "text/plain"));
app.MapGet("/auth/ping", () => Results.Ok(new { ok = true, instance = Environment.MachineName, time = DateTime.UtcNow }));

app.MapPost("/register", Register);
app.MapPost("/auth/register", Register);
app.MapPost("/login", Login);
app.MapPost("/auth/login", Login);

app.Run();

record UserLogin(string Username, string Password);
