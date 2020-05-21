using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Promotion.Areas.ParticipantArea.ViewModel;
using Promotion.Interfaces;
using Promotion.Extensions;
using Promotion.Filters;
using Promotion.Models;

namespace Promotion.Areas.ParticipantArea.Controllers
{
    [Area("ParticipantArea")]
    [Route("participant/login")]
    [AnonymousOnlyFilter("Index", "Participant")]
    public class LoginController : Controller
    {
        private readonly ILogger _logger;
        private readonly IParticipantRepository _participantRepository;

        public LoginController(IParticipantRepository participantRepository, ILogger<LoginController> logger)
        {
            _logger = logger;
            _participantRepository = participantRepository;
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
                    Participant participant = _participantRepository.FindUniqueByEmail(model.Email);
                    
                    if (participant == null || HashExtension.Validate(
                        model.Password,
                        Environment.GetEnvironmentVariable("AUTH_SALT"),
                        participant.Password) == false)
                    {
                        ModelState.AddModelError("Email", "Invalid credentials");
                        TempData["Error"] = "Aqui";
                        return View("Index", model);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, participant.Id.ToString()),
                        new Claim(ClaimTypes.Name, participant.Name),
                        new Claim(ClaimTypes.Email, participant.Email)
                    };

                    await HttpContext.SignInAsync(
                        "PromotionParticipantScheme",
                        new ClaimsPrincipal(new ClaimsIdentity(claims, "PromotionParticipantScheme")),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        }
                    );

                    return RedirectToAction("Index", "Participant");
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