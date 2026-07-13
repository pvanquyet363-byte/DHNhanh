using AspNetCoreRateLimit;
using Auto_Api.Service.Impliment;
using Auto_Api.Service.Interface;
using Deobely_sync;
using Deobely_sync.DAL;
using Deobely_sync.HostedServices;
using Deobely_sync.Infrastructure;
using Deobely_sync.Model;
using Deobely_sync.Service.Impliment;
using Deobely_sync.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var provider = builder.Services.BuildServiceProvider();
var Configuration = provider.GetRequiredService<IConfiguration>();
string name = "DbCnn_Sys";
string name2 = "DbCnn";
string name_thue = "DbCnn_thue";
string name_thue_sys = "DbCnn_Thue_Sys";
DAL_SQL.SetConnectionString(Configuration.GetConnectionString(name2));
DAL_SQL_SYS.SetConnectionString(Configuration.GetConnectionString(name));
DAL_SQL_Thue.SetConnectionString(Configuration.GetConnectionString(name_thue));
DAL_SQL_Thue_SYS.SetConnectionString(Configuration.GetConnectionString(name_thue_sys));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// IHttpClientFactory cho Nhanh - dùng lại connection pool, tránh socket exhaustion
builder.Services.AddHttpClient("nhanh", c =>
{
    c.BaseAddress = new Uri("https://pos.open.nhanh.vn/");
    c.Timeout = TimeSpan.FromSeconds(30);
    c.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(
    retryCount: 3,
    sleepDurationProvider: (attempt, resp, _) =>
    {
        var delta = resp?.Result?.Headers?.RetryAfter?.Delta;
        if (delta != null)
            return delta.Value;
        return TimeSpan.FromSeconds(Math.Pow(2, attempt));
    },
    onRetryAsync: (outcome, timespan, attempt, _) =>
    {
        APILog.ApiLog.logval("nhanh_retry",
            $"attempt={attempt}, delay={timespan.TotalSeconds}s, status={(int?)outcome.Result?.StatusCode}");
        return Task.CompletedTask;
    }));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.GeneralRules = new List<RateLimitRule>

        {

            new RateLimitRule
            {
                Endpoint = "*",
                Period = "100s",
                Limit = 50
            }

        };
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidIssuer = Configuration["Jwt:Issuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {

        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddScoped<IAuthentication, AuthenticationService>();
builder.Services.AddScoped<IProduct, Product>();
builder.Services.AddScoped<ICreateOrder, CreateOrderService>();
builder.Services.AddScoped<ICustomer, CustomerRepository>();
builder.Services.AddScoped<IContract, ContractRepository>();
builder.Services.AddScoped<ITbc, TbcRepository>();
builder.Services.AddScoped<ITranslateProvider, TranslateProvider>();
builder.Services.AddScoped<IWebHook, Webhook>();
builder.Services.AddScoped<IMatBao, MatBao>();

// Background xử lý webhook hàng loạt để tránh rate limit 50/100s
builder.Services.AddSingleton<NhanhWebhookQueue>();
builder.Services.AddHostedService<NhanhWebhookProcessor>();

var app = builder.Build();
string logFilePath = "log.txt";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options => {
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
    options.AllowAnyMethod();
});
app.UseCors();
app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestBodyLoggingMiddleware>();
app.MapControllers();


app.Run();
