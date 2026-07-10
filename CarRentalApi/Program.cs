using Application;
using Infrastructure;
using CarRentalApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Presentation
builder.Services.AddPresentation();
builder.Services.AddSwaggerDocumentation();

// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UsePresentation();

await app.SeedDataAsync();

app.Run();






//builder.Services.AddScoped<Domain.Services.Interfaces.IFileStorageService, Infrastructure.Services.FileStorageService>();

//Configure static files
/* builder.Services.Configure<StaticFileOptions>(options =>
{
    options.FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads"));
    options.RequestPath = "/uploads";
}); */



