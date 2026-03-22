using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using Minio;
using Api.Services.Files;
using Api.Services.Auth;
using Api.Options;
using Api.Database.Context;
using Api.Services.ObjectStorage;
using Api.Services.ConverterClient;
using Api.Services.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FileConverterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var minioSettings = builder.Configuration.GetSection("MinioSetup");

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

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    return new MinioClient()
        .WithEndpoint(minioSettings["Endpoint"]!.Replace("http://", ""))
        .WithCredentials(minioSettings["AccessKey"], minioSettings["SecretKey"])
        .WithSSL(false)
        .Build();
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IObjectStorage, ObjectStorage>();
builder.Services.AddScoped<IConverterClient, ConverterClient>();

builder.Services.AddHttpClient<IConverterClient, ConverterClient>(client =>
{
    client.BaseAddress = new Uri("http://hidden-api:8000");
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.Configure<MinioDirectoriesOptions>(
    builder.Configuration.GetSection("MinioDirectoriesOptions"));
builder.Services.Configure<Api.Options.FileOptions>(
    builder.Configuration.GetSection("FileOptions"));

builder.Services.AddControllers();

var jwt = "SuperExtraSecretJwtKey1234567890";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt))
        };
    });

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileConverterContext>();
    dbContext.Database.Migrate();
}

app.Run();