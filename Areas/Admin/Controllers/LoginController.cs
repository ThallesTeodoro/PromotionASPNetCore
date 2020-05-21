using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Promotion.Areas.Admin.ViewModel;
using Promotion.Interfaces;
using Promotion.Extensions;
using Promotion.Filters;
using Promotion.Models;

namespace Promotion.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/login")]
    [AnonymousOnlyFilter("Index", "Dashboard")]
    public class LoginController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository,ILogger<LoginController> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> Auth(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = _userRepository.FindUniqueByEmail(model.Email);

                    if (user == null || HashExtension.Validate(
                        model.Password,
                        Environment.GetEnvironmentVariable("AUTH_SALT"),
                        user.Password) == false)
                    {
                        ModelState.AddModelError("Email", "Invalid credentials");
                        return View("Index", model);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    await HttpContext.SignInAsync(
                        "PromotionScheme",
                        new ClaimsPrincipal(new ClaimsIdentity(claims, "PromotionScheme")),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        }
                    );

                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Internal server error";
                _logger.LogError("Login error: " + exception);
            }

            return View("Index", model);
        }
    }
}