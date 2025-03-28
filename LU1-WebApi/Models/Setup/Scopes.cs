﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
public static class Scopes
{
    public static void AddScopes(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IPatientRepository>(sp =>
            new PatientRepository(connectionString));

        services.AddScoped<IAccountRepository>(sp =>
        {
            var userManager = sp.GetRequiredService<UserManager<AppUser>>();
            var signInManager = sp.GetRequiredService<SignInManager<AppUser>>();
            return new AccountRepository(userManager, signInManager, connectionString);
        });

        services.AddScoped<IAuthenticationService, AspNetIdentityAuthenticationService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidIssuer = "LU1-WebApi",
               ValidAudience = "LU1-WebApi",
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(connectionString))
           };
       });
    }
}