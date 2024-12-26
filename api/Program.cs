using Application.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();
// Add session services
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie accessible only via HTTP
    options.Cookie.IsEssential = true; // Mark the cookie as essential
});

builder.AddKeycloakSettings();
builder.AddKeycloakAuthorization();
//builder.AddSwagerBearerAuthentication();





var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Use session middleware
app.UseSession();

// Enable forwarded headers middleware for correct scheme detection
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



/* 
 
For production environments with multiple servers, you can use SQL Server for distributed cache.

Install the required NuGet package:

dotnet add package Microsoft.Extensions.Caching.SqlServer
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("SqlServerCacheConnection");
    options.SchemaName = "dbo";
    options.TableName = "SessionCache";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


under appsettings:
"ConnectionStrings": {
  "SqlServerCacheConnection": "Server=your-server;Database=your-database;Trusted_Connection=True;"
}


CREATE TABLE SessionCache
(
    Id NVARCHAR(449) NOT NULL PRIMARY KEY,
    Value VARBINARY(MAX) NOT NULL,
    ExpiresAtTime DATETIMEOFFSET NOT NULL,
    SlidingExpirationInSeconds BIGINT NULL,
    AbsoluteExpiration DATETIMEOFFSET NULL
);

 
app.UseSession(); // Add this before UseAuthorization
 
 
 */