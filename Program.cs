using CustomMiddleWare.MiddleWare;
using CustomMiddleWare.Middlewares;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<MyCustomMiddleware>();

builder.Services.AddScoped<MyCustomAuthorizationMiddleware>();

// builder.Services.AddSingleton<MyTelemeterLog>();

builder.Services.AddOpenTelemetry().ConfigureResource(resource =>
resource.AddService(serviceName: builder.Environment.ApplicationName).AddEnvironmentVariableDetector()).
WithTracing(tracking => tracking.AddAspNetCoreInstrumentation().AddConsoleExporter()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseMiddleware<MyCustomMiddleware>();

app.UseMiddleware<MyCustomAuthorizationMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/api/health");

app.MapControllers();


app.Run();
