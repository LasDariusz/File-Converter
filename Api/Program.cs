using Api.Application;
using Api.Infrastructure.ConverterClient;
using Api.Infrastructure.Database;
using Api.Infrastructure.Jwt;
using Api.Infrastructure.ObjectStorage;
using Api.Middlewares;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:58647",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.ConfigureConverterHttpClient();

builder.ConfigureDatabaseContext();

builder.ConfigureTokenAuthentication();

builder.ConfigureObjectStorage();

builder.ConfigureApplicationServices();


builder.Services.AddControllers();


builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "File Converter API",
        Version = "1.0"
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    o.DocExpansion(DocExpansion.List);
    o.DefaultModelExpandDepth(0);
    o.DisplayRequestDuration();
    o.EnableFilter();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileConverterContext>();
    dbContext.Database.Migrate();
}*/

app.Run();