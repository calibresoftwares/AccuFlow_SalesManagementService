using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalesManagementService.API.Endpoints;
using SalesManagementService.Application;
using SalesManagementService.Application.Mappings;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.Interfaces;
using SalesManagementService.Domain.TenantSettings;
using SalesManagementService.Infrastructure;
using SalesManagementService.Infrastructure.Config;
using SalesManagementService.Infrastructure.DatabaseManager;
using SalesManagementService.Infrastructure.Kafka;
using SalesManagementService.Infrastructure.Repositories;
using SalesManagementService.Infrastructure.TenantConfig;

using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<ISalesInvoiceRepository, SalesInvoiceRepository>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));

var environment = builder.Environment.EnvironmentName;
IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
   .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
   .Build();

//Serilog
var serilog = new Serilog.LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
var loggerFactory = new LoggerFactory().AddSerilog(serilog);
var logger = loggerFactory.CreateLogger(nameof(Program));

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read configuration from appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Save logs to file
    .CreateLogger();

// Replace the default logging provider with Serilog
builder.Host.UseSerilog();

//Add Method Extension
builder.Services.AddInjectionApplication();
builder.Services.AddInfrastructureInjectionServices(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// JWT Authentication

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                       .AddJwtBearer(options =>
                       {
                           options.TokenValidationParameters = new TokenValidationParameters
                           {
                               // Validate the issuer
                               ValidateIssuer = true,
                               ValidIssuer = issuer,

                               // Validate the audience
                               ValidateAudience = true,
                               ValidAudience = audience,

                               // Validate the expiration date
                               ValidateLifetime = true,

                               // Validate the signing key (this should match the key used to sign the token)
                               ValidateIssuerSigningKey = true,
                               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("JWT SecretKey is not configured"))),

                               RoleClaimType = "Role",
                           };


                           // Optionally, you can add events for more logging or customization
                           options.Events = new JwtBearerEvents
                           {
                               OnTokenValidated = context =>
                               {
                                   var claimsPrincipal = context.Principal;

                                   // Validate specific claims
                                   var role = claimsPrincipal?.FindFirst("Role")?.Value;
                                   var tenantId = claimsPrincipal?.FindFirst("TenantId")?.Value;

                                   if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(tenantId))
                                   {
                                       context.Fail("Required claims are missing.");
                                   }

                                   return Task.CompletedTask;
                               },

                               OnAuthenticationFailed = context =>
                               {
                                   var exceptionMessage = context.Exception.Message;
                                   var stackTrace = context.Exception.StackTrace;

                                   Console.WriteLine($"Authentication failed: {exceptionMessage}");
                                   Console.WriteLine($"Stack Trace: {stackTrace}");

                                   Console.WriteLine("Authentication failed.");
                                   return Task.CompletedTask;
                               }
                               //OnTokenValidated = context =>
                               //{
                               //    Console.WriteLine("Token validated.");
                               //    return Task.CompletedTask;
                               //}
                           };
                       });

// Add CORS policy
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Memory Cache
builder.Services.AddMemoryCache();

builder.Services.AddScoped<ITenantService, TenantService>(); // TenantService registration

builder.Services.AddHttpClient<ITenantService, TenantService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GlobalERPMasterService:BaseUrl"] ?? throw new InvalidOperationException("GlobalERPMasterService:BaseUrl is not configured"));
});

builder.Services.AddDbContext<SalesManagementDbContext>((serviceProvider, options) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var tenantService = serviceProvider.GetRequiredService<ITenantService>();
    //var tenantId = _ca

    // Retrieve TenantId from the request header
    //var tenantIdHeader = httpContextAccessor.HttpContext?.Request.Headers["TenantId"];
    //if (string.IsNullOrEmpty(tenantIdHeader))
    //{
    //    throw new Exception("TenantId header is missing.");
    //}

    //if (!Guid.TryParse(tenantIdHeader, out var tenantId))
    //{
    //    throw new Exception("Invalid TenantId format.");
    //}

    // Fetch the tenant-specific connection string
    var tenantDetails = tenantService.GetConnectionStringAsync().GetAwaiter().GetResult();

    if (tenantDetails == null)
    {
        throw new Exception("Tenant connection string not found.");
    }

    options.UseNpgsql(tenantDetails.ConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

// Enable middleware to catch unhandled exceptions
app.UseSerilogRequestLogging();  // Enable Serilog request logging

//app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapCustomerEndpoints();
app.MapSalesOrderEndpoints();
app.MapSalesInvoiceEndpoints();

app.MapControllers();

app.Run();





//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
