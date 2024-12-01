using SE.Sustainability.Platform.OpenTelemetryInstrumentation;
using SE.Sustainability.Shared.ContextualLogging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("APP_ENV") ?? "local";
Console.WriteLine($"Environment: {environment}");
builder.Configuration.AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{environment}.json");
var appSettings = builder.Configuration.GetSection("AppSettings");

// TODO: Uncomment the following to access Key Vault using managed identity. Then update your appsettings.env.json file AppSettings:AzureKeyVault:KeyVaultUrl value with your key vault url
// Fetch secrets from Key Vault
// var keyVaultSection = builder.Configuration.GetSection("AppSettings:AzureKeyVault");
// var keyVaultUrl = keyVaultSection.GetValue("KeyVaultUrl", string.Empty);
// Console.WriteLine(
//     $"Adding Azure KeyVault: {keyVaultSection.GetValue("KeyVaultUrl", string.Empty)}"
// );
// builder.Configuration.AddAzureKeyVault(
//     new Uri(keyVaultUrl!),
//     new DefaultAzureCredential()
// );

// Configure Contextual Logging
builder.Services.AddSEContextualLogging(configure =>
{
    // TODO: Change the application name
    configure.ApplicationName = "CHANGE ME";
    configure.LogToConsole = true;
    configure.MinimumLogLevel = environment == "local" ? LogLevel.Debug : LogLevel.Information;
    configure.LoggingFilters.Add("Microsoft.AspNetCore", LogLevel.Warning);
    configure.LoggingFilters.Add("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    configure.LoggingFilters.Add("Microsoft.IdentityModel", LogLevel.Warning);
    configure.LoggingFilters.Add("System.Net.Http.HttpClient", LogLevel.Warning);
    configure.LoggingFilters.Add("Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService",
        LogLevel.Warning);
});
// Configure Open Telemetry
builder.Services.AddOpenTelemetryInstrumentation(builder.Configuration);

builder.Services.AddHealthChecks();
builder.Services.AddControllers();

// Configure CORS. TODO: Review and update as necessary
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        config =>
        {
            config.AllowAnyOrigin() // We could limit this to specific origins once we know them
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

if (environment != "prod")
{
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SchemaGeneratorOptions.SupportNonNullableReferenceTypes = true;
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    });
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseHealthChecks("/healthz");

// Apply CORS policy
app.UseCors("AllowAll");

app.UsePathBase("/api");
app.MapControllers();

// Configure the HTTP request pipeline.
if (environment != "prod")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();