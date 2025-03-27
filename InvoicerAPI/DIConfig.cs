using InvoicerAPI.Application.DTOs.Auth;
using InvoicerAPI.Application.Jobs;
using InvoicerAPI.Application.Services.Auth;
using InvoicerAPI.Core.Entities;
using InvoicerAPI.Core.Interfaces.Auth;
using InvoicerAPI.Core.Interfaces.Providers;
using InvoicerAPI.Infrastructure.Data;
using InvoicerAPI.Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using System.Text;

namespace InvoicerAPI;

public static class DIConfig
{
	public static IServiceCollection AddSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(setup =>
		{
			setup.SwaggerDoc("v1",
				new OpenApiInfo
				{
					Title = "Invoicer API",
					Version = "v: 1.0"
				});

			setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
			{
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer key-key-key-key\""
			});

			setup.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
							}, new string[]{}
						}
				});
		});

		return services;
	}

	public static IServiceCollection AuthenticationAndAuthorization(
					this IServiceCollection services,
					IConfiguration configuration)
	{
		services.AddScoped<IRequestUserProvider, RequestUserProvider>();

		services.AddIdentity<User, IdentityRole<Guid>>()
				.AddEntityFrameworkStores<InvoicerDbContext>()
				.AddDefaultTokenProviders();

		services.AddScoped<IJwtService, JwtService>();

		var jwtConfig = new JwtConfigDto();

		configuration.Bind("JWT", jwtConfig);

		services.AddSingleton(jwtConfig);

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters =
			new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ClockSkew = TimeSpan.Zero,
				ValidIssuer = jwtConfig.Issuer,
				ValidAudience = jwtConfig.Audience,
				IssuerSigningKey
				= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
			};
		});

		return services;
	}

	public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
	{
		services.AddQuartz(q =>
		{
			var jobKey = new JobKey("CleanupJob");

			q.AddJob<DbCleanupJob>(opts => opts.WithIdentity(jobKey));

			// her bazar gunu , saat 23:00 da ishleyecek.
			q.AddTrigger(opts =>
				opts.ForJob(jobKey)
					.WithIdentity("CleanupTrigger")
					.WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 23, 0))
			);
		});
		services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

		return services;
	}
}