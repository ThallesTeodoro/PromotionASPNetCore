using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Promotion.Data
{
    internal class ConfigureCookie : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        public ConfigureCookie() {}

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            if (name == "PromotionScheme")
            {
                options.AccessDeniedPath = "/admin/login";
                options.LoginPath = "/admin/login";
                options.Cookie.Name = "Promotion.Admin";
            } 
            else if (name == "PromotionParticipantScheme")
            {
                options.AccessDeniedPath = "/participant/login";
                options.LoginPath = "/participant/login";
                options.Cookie.Name = "Promotion.Participant";
            }
        }

        public void Configure(CookieAuthenticationOptions options)
            => Configure(Options.DefaultName, options);
    }
}