using HealthTech.Application;
using HealthTech.Infrastructure;
using HealthTech.API.Middleware;
using HealthTech.API.Endpoints;
using HealthTech.API.Swagger;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add API services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add authentication for development/testing
builder.Services.AddAuthentication("Development")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, HealthTech.API.Authentication.DevelopmentAuthenticationHandler>("Development", options => { });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "FHIR-AI Backend API", 
        Version = "v1",
        Description = @"
## HealthTech FHIR-AI Backend

A comprehensive FHIR-compliant healthcare data platform with AI integration capabilities, built on .NET 8 and PostgreSQL.

### Key Features
- **FHIR R4B Compliance**: Full support for FHIR R4B resources and operations
- **Multi-tenant Architecture**: Secure data isolation with Row-Level Security
- **SMART on FHIR**: OAuth2/OpenID Connect authentication with scope enforcement
- **AI Integration**: Ready for machine learning and AI model integration
- **Audit Trail**: Comprehensive logging for compliance and security
- **Performance Optimized**: JSONB storage with GIN indexes for efficient querying

### Authentication
This API supports SMART on FHIR authentication with the following scopes:
- `system/*` - System-level access
- `user/*` - User-level access  
- `patient/*` - Patient-level access
- `user/Patient.read` - Specific resource access

### Multi-tenancy
All requests must include tenant context via:
- JWT claim: `tenant_id` or `org_id`
- Header: `X-Tenant-ID`

### Rate Limiting
- 100 requests per minute per tenant
- Additional limits may apply based on subscription tier

### Support
For technical support, contact: support@healthtech.com
",
        Contact = new OpenApiContact
        {
            Name = "HealthTech Support",
            Email = "support@healthtech.com",
            Url = new Uri("https://healthtech.com/support")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Add JWT authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add API Key for development/testing
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key for development and testing purposes",
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new[] { "system/*", "user/*", "patient/*" }
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add operation filters for better documentation
    c.OperationFilter<SwaggerDefaultValues>();
    
    // Add schema filters for FHIR resources
    c.SchemaFilter<FhirResourceSchemaFilter>();

    // Customize operation IDs
    c.CustomOperationIds(apiDesc =>
    {
        return apiDesc.ActionDescriptor?.DisplayName ?? apiDesc.RelativePath;
    });

    // Add tags for better organization
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }

        var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        // Fallback for Minimal API endpoints
        if (api.RelativePath?.StartsWith("fhir") == true)
        {
            return new[] { "FHIR" };
        }

        if (api.RelativePath?.StartsWith("health") == true)
        {
            return new[] { "System" };
        }

        // Default fallback
        return new[] { "API" };
    });

    c.DocInclusionPredicate((name, api) => true);
    
    // Add support for file uploads
    c.OperationFilter<FileUploadOperationFilter>();
});

// Configure JSON serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Configure form options for file uploads
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB limit for large FHIR bundles
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("HealthTechCors", policy =>
    {
        policy
            //.WithOrigins(
            //    "http://localhost:3000", // React dev server
            //    "http://localhost:4200", // Angular dev server
            //    "https://localhost:3000",
            //    "https://localhost:4200"
            //)
            //.AllowAnyMethod()
            //.AllowAnyHeader()
            //.AllowCredentials();
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FHIR-AI Backend API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
        
        // Customize Swagger UI
        c.DocumentTitle = "FHIR-AI Backend API Documentation";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelExpandDepth(2);
        c.DisplayRequestDuration();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        
        // Add custom CSS for better styling
        c.InjectStylesheet("/swagger-ui/custom.css");
        
        // Add custom JavaScript for enhanced functionality
        c.InjectJavascript("/swagger-ui/custom.js");
        
        // Configure OAuth2 settings for testing
        c.OAuthClientId("swagger-ui");
        c.OAuthClientSecret("swagger-secret");
        c.OAuthRealm("healthtech-fhir");
        c.OAuthAppName("FHIR-AI Backend Swagger UI");
        c.OAuthScopeSeparator(" ");
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();
app.UseCors("HealthTechCors");

// Add custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<AuditLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Map FHIR endpoints
app.MapFhirEndpoints();

// Map health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();
