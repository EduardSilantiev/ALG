using ALG.Application;
using ALG.Application.Helpers.Exceptions;
using ALG.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ALG.Web.Host.JWT
{
    public static class AuthenticationServiceExtentions
    {
        public static IServiceCollection AddJWTSecurity(this IServiceCollection services,
                            JWTSettings settings)
        {
            var key = Encoding.ASCII.GetBytes(settings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents()
                {
                    //implement unified error messaging approach
                    OnChallenge = context =>
                    {
                        // Skip the default logic.
                        context.HandleResponse();

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)AppConsts.HttpStatusCodes.Status401Unauthorized;

                        return context.Response.WriteAsync(new ExceptionMessage("User is not authenticated.").ToString());
                    }
                };
            });

            return services;
        }
    }
}
