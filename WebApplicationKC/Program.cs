using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace WebApplicationKC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = "http://localhost:8084/realms/realm_01";
                options.ClientId = "AspNetApp";
                options.ClientSecret = "yRqrp7SphnjRzd1ENhoKZobCdlj1Yaz9";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.RequireHttpsMetadata = false; // Usa true in produzione
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    NameClaimType = "name",
                };
            });

            var app = builder.Build();

            // Other service configurations...

            // Enable PII logging only in development
            if (app.Environment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                IdentityModelEventSource.ShowPII = false;
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
