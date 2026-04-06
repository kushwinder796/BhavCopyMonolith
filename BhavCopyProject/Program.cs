

using Bhav.Application.Command.CommandHandler;
using Bhav.Application.IRepositories;
using Bhav.Application.Services;
using Bhav.Infrastructure.Persistence;
using Bhav.Infrastructure.Persistence.Entities;
using Bhav.Infrastructure.Repositories;
using BhavCopyProject;
using Margin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BhavCopyProject.Common;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseWrapperFilter>();
});

builder.Services.AddOpenApi();

builder.Services.AddDbContext<BhavDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<MarginDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("Bhav.Application"));
    cfg.RegisterServicesFromAssembly(typeof(FetchBhavCopyCommandHandler).Assembly);
});

builder.Services.AddHttpClient("BhavCopy", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

//builder.Services.AddHttpClient<BhavCopyFetchService>();
builder.Services.AddScoped<BhavCopyParserService>();

builder.Services.AddScoped<IBhavCopyRepository, BhavCopyRepository>();




builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;

    // If something already wrote content, don't overwrite it.
    if (response.HasStarted || response.ContentLength is > 0)
        return;

    var status = response.StatusCode;
    var success = status is >= 200 and <= 299;
    if (success) return;

    response.ContentType = "application/json; charset=utf-8";

    var payload = ApiResponse<object?>.Fail(
        errors: null,
        statusCode: status,
        message: status switch
        {
            StatusCodes.Status404NotFound => "Not found",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            _ => "Request failed"
        });

    await response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }));
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
