using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Api.ChatHub;
using Application.Common.Security;
using Application.Dtos;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Core.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mini E-commerce API", Version = "v1" });
    // CookieAuth
    c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "AccessToken",
        Description = "Authentication via AccessToken cookie"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "cookieAuth"
                }
            },
            new string[] {}
        }
    });
});

// 🔹 DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
   {
       options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Infrastructure"));
       options.UseLazyLoadingProxies(false);
   });

// 🔹 Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔹 Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RoleClaimType = "role",
        NameClaimType = JwtRegisteredClaimNames.Sub,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AccessToken"))
            {
                context.Token = context.Request.Cookies["AccessToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

// 🔹 DI
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IPackageUnitService, PackageUnitService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IBarcodeService, BarcodeService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IApprovalConfigService, ApprovalConfigService>();
builder.Services.AddScoped<IApprovalFeatureService, ApprovalFeatureService>();
builder.Services.AddScoped<IApprovalStepService, ApprovalStepService>();
builder.Services.AddScoped<IApprovalHistoryService, ApprovalHistoryService>();
builder.Services.AddScoped<IApprovalRequestService, ApprovalRequestService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();

// 🔹 Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RefreshTokenRequestDtoValidator>();

// 🔹 AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new AuthProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new ChatProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new CatalogProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new ApprovalConfigProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new ApprovalFeatureProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new ApprovalStepProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new ApprovalHistoryProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new ApprovalRequestProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new DepartmentProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new PositionProfile()));

// 🔹 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularLocal",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200",
                    "http://demax-hr.vn",
                    "http://demax-wh.vn",
                    "http://app.demax-wh.vn",
                    "http://app.demax-hr.vn",
                    "http://ui.demax-wh.vn",
                    "http://ui.demax-hr.vn",
                    "https://demax-hr.vn",
                    "https://demax-wh.vn",
                    "https://app.demax-wh.vn",
                    "https://app.demax-hr.vn",
                    "https://ui.demax-wh.vn",
                    "https://ui.demax-hr.vn"
            )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Initialize roles and permissions
using (var scope = app.Services.CreateScope())
{
    var rolePermissionService = scope.ServiceProvider.GetRequiredService<IRolePermissionService>();
    await rolePermissionService.InitializeRolesAndPermissionsAsync();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await Infrastructure.Data.DataSeeder.SeedAsync(db);
}
app.UseCors("AllowAngularLocal");

// 🔹 Bật đọc X-Forwarded-For, X-Forwarded-Proto
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chatHub");
app.MapControllers();
app.Run();
