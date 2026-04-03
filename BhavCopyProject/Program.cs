using Bhav.Application.Command;
using Bhav.Application.IRepositories;
using Bhav.Application.Services;
using Bhav.Infrastructure.Persistence;
using Bhav.Infrastructure.Persistence.Entities;
using Bhav.Infrastructure.Repositories;
using Bhav.Infrastructure.Services;
using Margin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<BhavDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<MarginDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("Bhav.Application"));
    cfg.RegisterServicesFromAssembly(typeof(UploadBhavCopyCommand).Assembly);
});

builder.Services.AddScoped<IBhavCopyRepository, BhavCopyRepository>();
builder.Services.AddScoped<IBhavCopyParserService, BhavCopyParserService>();
builder.Services.AddScoped<IBhavUploadService, BhavUploadService>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
