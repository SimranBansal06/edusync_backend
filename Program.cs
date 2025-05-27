////using Microsoft.EntityFrameworkCore;
////using webapi.Data;
////using System.Text.Json.Serialization; // Add this line

////namespace webapi
////{
////    public class Program
////    {
////        public static void Main(string[] args)
////        {
////            var builder = WebApplication.CreateBuilder(args);

////            // Add services to the container.

////            // Update this part
////            builder.Services.AddControllers()
////                .AddJsonOptions(options =>
////                {
////                    // This prevents circular reference errors during JSON serialization
////                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////                    // This ignores null values during serialization
////                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
////                });

////            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
////            builder.Services.AddEndpointsApiExplorer();
////            builder.Services.AddSwaggerGen();
////            builder.Services.AddDbContext<AppDbContext>(options =>
////                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////            var app = builder.Build();

////            // Configure the HTTP request pipeline.
////            if (app.Environment.IsDevelopment())
////            {
////                app.UseSwagger();
////                app.UseSwaggerUI();
////            }

////            app.UseHttpsRedirection();

////            app.UseAuthorization();

////            app.MapControllers();

////            app.Run();
////        }
////    }
////}


//using Microsoft.EntityFrameworkCore;
//using webapi.Data;
//using System.Text.Json.Serialization;

//namespace webapi
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add CORS service
//            builder.Services.AddCors(options =>
//            {
//                options.AddPolicy("ReactAppPolicy", policy =>
//                {
//                    policy.WithOrigins("http://localhost:3000")
//                          .AllowAnyHeader()
//                          .AllowAnyMethod()
//                          .AllowCredentials();
//                });
//            });

//            // Add services to the container.
//            builder.Services.AddControllers()
//                .AddJsonOptions(options =>
//                {
//                    // This prevents circular reference errors during JSON serialization
//                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//                    // This ignores null values during serialization
//                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//                });

//            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();
//            builder.Services.AddDbContext<AppDbContext>(options =>
//                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            // Pass the policy name explicitly to UseCors
//            app.UseCors("ReactAppPolicy");

//            // Comment this line temporarily for testing if you still have issues
//            // app.UseHttpsRedirection();

//            app.UseAuthorization();

//            app.MapControllers();

//            app.Run();
//        }
//    }
//}

//24/05/25

//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using System.Text.Json.Serialization;
//using webapi.Data;

//namespace webapi
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add CORS
//            builder.Services.AddCors(options =>
//            {
//                options.AddPolicy("ReactAppPolicy", policy =>
//                {
//                    policy.WithOrigins("http://localhost:3000")
//                          .AllowAnyHeader()
//                          .AllowAnyMethod()
//                          .AllowCredentials();
//                });
//            });

//            // Add controllers
//            builder.Services.AddControllers()
//                .AddJsonOptions(options =>
//                {
//                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//                });

//            // Add database context
//            builder.Services.AddDbContext<AppDbContext>(options =>
//                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//            // Add JWT Authentication
//            builder.Services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//            .AddJwtBearer(options =>
//            {
//                options.RequireHttpsMetadata = false;
//                options.SaveToken = true;
//                options.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuer = true,
//                    ValidateAudience = true,
//                    ValidateLifetime = true,
//                    ValidateIssuerSigningKey = true,
//                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                    ValidAudience = builder.Configuration["Jwt:Audience"],
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "defaultkeyfordevwhichshouldbereplaced"))
//                };
//            });

//            // Add authorization policies
//            builder.Services.AddAuthorization(options =>
//            {
//                options.AddPolicy("RequireInstructorRole", policy =>
//                    policy.RequireRole("Instructor"));
//                options.AddPolicy("RequireStudentRole", policy =>
//                    policy.RequireRole("Student"));
//            });

//            // Add API Explorer and Swagger
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();

//            var app = builder.Build();

//            // Configure the HTTP request pipeline
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseCors("ReactAppPolicy");

//            app.UseAuthentication();
//            app.UseAuthorization();

//            app.MapControllers();

//            app.Run();
//        }
//    }
//}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using webapi.Data;
using webapi.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // Add JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "defaultkeyfordevwhichshouldbereplaced"))
            };
        });

        // Add authorization policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireInstructorRole", policy =>
                policy.RequireRole("Instructor"));
            options.AddPolicy("RequireStudentRole", policy =>
                policy.RequireRole("Student"));
        });

        // Add BlobStorageService
        builder.Services.AddScoped<BlobStorageService>();

        // Add Controllers with JSON options
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        // Add API Explorer and Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add DbContext
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            // Use Swagger in production too for testing
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Use CORS with the new policy
        app.UseCors("AllowAllOrigins");

        app.UseAuthentication();
        app.UseAuthorization();

        // Only set up physical file provider if not in Azure (check for Azure environment)
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
        {
            // We're in Azure, don't use physical file provider
            // Files should be stored in Azure Blob Storage instead
        }
        else
        {
            // Local development environment
            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/uploads"
            });
        }

        app.MapControllers();

        app.Run();
    }
}