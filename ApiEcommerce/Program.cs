using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Migrations;
using ApiEcommerce.Model;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"))); 

builder.Services.AddResponseCaching(options  =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = false;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepositoy>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var secreKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

if (string.IsNullOrEmpty(secreKey))
{
    throw new InvalidOperationException("SecretKey no está configurada");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreKey)),
        ValidateIssuer = false,
        ValidateAudience = false, 
    };
});


builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
    options.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
});


builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;

}).AddApiExplorer(option =>
{
    option.GroupNameFormat ="'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors(options =>
{
   options.AddPolicy(PolicyNames.AllowSpecificOrigin,
   builder=>
   {
       builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
   });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. Escribe tu token a continuación.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo()
    {
         Version = "v1",
         Title = "API Ecommerce",
         Description = "API para gestionar productos y usuarios",
         TermsOfService = new Uri("http://example.com/terms"),
         Contact = new OpenApiContact()
         {
            Name = "4w4kt",
            Url = new Uri("http://4w4kt.com")
         },
         License = new OpenApiLicense()
         {
             Name = "License de uso",
             Url = new Uri("http://example.com/license")
         }
    });
    options.SwaggerDoc("v2", new OpenApiInfo()
    {
         Version = "v2",
         Title = "API Ecommerce",
         Description = "API para gestionar productos y usuarios",
         TermsOfService = new Uri("http://example.com/terms"),
         Contact = new OpenApiContact()
         {
            Name = "4w4kt",
            Url = new Uri("http://4w4kt.com")
         },
         License = new OpenApiLicense()
         {
             Name = "License de uso",
             Url = new Uri("http://example.com/license")
         }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
 