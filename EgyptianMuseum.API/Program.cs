using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Application.Services.Auth;
using EgyptianMuseum.Application.Services.Chat;
using EgyptianMuseum.Application.Services.Feedback;
using EgyptianMuseum.Application.Services.NotifictionsService;
using EgyptianMuseum.Application.Services.ScannedArtifacts;
using EgyptianMuseum.Application.Services.TourService;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using EgyptianMuseum.Infrastructure.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EgyptianMuseum.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(15),
                errorNumbersToAdd: null);
        }));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            // Validate Jwt configuration section exists
            var jwtSection = builder.Configuration.GetSection("Jwt");
            if (string.IsNullOrWhiteSpace(jwtSection["SecretKey"]))
            {
                throw new InvalidOperationException("JWT configuration is missing or incomplete. Please ensure 'Jwt:SecretKey' is set in configuration.");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });


            builder.Services.AddScoped<IAuthService, AuthService>();

            // Register Chat services and repositories
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IChatConversationRepository, ChatConversationRepository>();
            builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            builder.Services.AddScoped<IAiChatService, MockAiChatService>();

            // Register ScannedArtifact services and repositories
            builder.Services.AddScoped<IScannedArtifactService, ScannedArtifactService>();
            builder.Services.AddScoped<IScannedArtifactRepository, ScannedArtifactRepository>();
            builder.Services.AddScoped<IPieceRepository, PieceRepository>();

            // Register Feedback services and repositories
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();


            // Register Notification services and repositories
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

            // Register Tour services and repositories

            builder.Services.AddScoped<ITourRepository, TourRepository>();
            builder.Services.AddScoped<ITourService, TourService>();




            #region SwaggerSettings
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}'"
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
                        new string[] { }
                    }
                });

            });
            #endregion 

            var app = builder.Build();

            // Configure the HTTP request pipeline.


            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("MyPolicy");
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
