using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Promotion.Data;
using Promotion.Interfaces;
using Promotion.Repositories;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Promotion
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<PromotionContext>(
                options => options.UseMySql(Configuration.GetConnectionString("Default"))
            );

            services.AddAuthentication("PromotionScheme")
                .AddCookie("PromotionScheme", options => 
                {
                    options.AccessDeniedPath = "/admin/login";
                    options.LoginPath = "/admin/login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = "Promotion.Admin";
                })
                .AddCookie("PromotionParticipantScheme", options => 
                {
                    options.AccessDeniedPath = "/participant/login";
                    options.LoginPath = "/participant/login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = "Promotion.Participant";
                });

            services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, ConfigureCookie>();

            services.AddScoped<IUserRepository, UserReposiotry>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IEpisodeRepository, EpisodeRepository>();
            services.AddScoped<INewsletterRepository, NewsletterRepository>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            services.AddScoped<IParticipationRepository, ParticipationRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) => 
            {
                if (!context.Request.Path.Value.StartsWith("/admin"))
                {
                    var participantScheme = await context.AuthenticateAsync("PromotionParticipantScheme");
                    var principal = new ClaimsPrincipal();

                    if (participantScheme?.Principal != null)
                    {
                        principal.AddIdentities(participantScheme.Principal.Identities);
                    }
                    else
                    {
                        principal.AddIdentity(new ClaimsIdentity());
                    }

                    context.User = principal;
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Admin",
                    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "API",
                    pattern: "{area=API}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "ParticipantArea",
                    pattern: "{area=ParticipantArea}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
