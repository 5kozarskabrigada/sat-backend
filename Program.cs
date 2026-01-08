using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SAT.API.Data;
using SAT.API.Hubs;
using SAT.API.Middleware;
using SAT.API.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

const string CorsPolicyName = "FrontendPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins("https://sat-frontend-two.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
    });
});

// Supabase client
builder.Services.AddSingleton(sp =>
{
    var url = configuration["Supabase:Url"]!;
    var key = configuration["Supabase:Key"]!;
    var options = new SupabaseOptions
    {
        AutoConnectRealtime = false
    };
    return new Client(url, key, options);
});

var jwtSecret = configuration["Supabase:JwtSecret"]
    ?? throw new InvalidOperationException("Supabase:JwtSecret missing");

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token) &&
                    context.Request.Cookies.TryGetValue("sat_jwt", out var cookieToken))
                {
                    context.Token = cookieToken;
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// SignalR
builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SAT ExamHub API", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, Array.Empty<string>() }
    });
});

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IAccessCodeService, AccessCodeService>();
builder.Services.AddScoped<IResponseService, ResponseService>();
builder.Services.AddScoped<IGradingService, GradingService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ITimerService, TimerService>();

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Seed");
    await SeedData.InitializeAsync(db, logger); // Ensure this does NOT call db.Database.MigrateAsync()
}



// Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SAT API V1");
    c.RoutePrefix = "swagger";
});

// CORS must be before auth
app.UseCors(CorsPolicyName);

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseMiddleware<JwtValidationMiddleware>();
app.UseMiddleware<SupabaseJwtMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ExamHub>("/hubs/examhub");

app.Run();