using Api.Application.Features.Auth.Login;
using Api.Application.Features.Auth.RefreshToken;
using Api.Application.Features.Auth.Register;
using Api.Application.Features.Files.ConvertFile;
using Api.Application.Features.Files.DeleteFile;
using Api.Application.Features.Files.FetchFile;
using Api.Application.Features.Files.FetchFilesMetadata;
using Api.Application.Validation;
using Api.Options;

namespace Api.Application;

public static class ApplicationExtensions
{
    public static void ConfigureApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ILoginService, LoginService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<IRegisterService, RegisterService>();

        builder.Services.AddScoped<IConvertFileService, ConvertFileService>();
        builder.Services.AddScoped<IDeleteFileService, DeleteFileService>();
        builder.Services.AddScoped<IFetchFileService, FetchFileService>();
        builder.Services.AddScoped<IFetchFilesMetadataService, FetchFilesMetadataService>();

        builder.Services.AddTransient<IFileValidator, FileValidator>();

        builder.Services.Configure<UploadedFilesOptions>(
            builder.Configuration.GetSection("UploadedFilesOptions")
        );
    }
}