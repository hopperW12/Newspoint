using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Newspoint.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositoriesFromAssembly();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddServicesFromAssembly();
builder.Services.AddWebServicesFromAssembly();
builder.Services.AddApplicationValidation();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddNewspointSwagger();

var app = builder.Build();

await app.MigrateAsync();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Newspoint API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
