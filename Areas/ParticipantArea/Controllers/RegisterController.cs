using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Promotion.Models;
using Promotion.Interfaces;
using Promotion.Filters;
using Promotion.Areas.ParticipantArea.ViewModel;
using Promotion.Extensions;

namespace Promotion.Areas.ParticipantArea.Controllers
{
    [Area("ParticipantArea")]
    [Route("participant/register")]
    [AnonymousOnlyFilter("Index", "Participant")]
    public class RegisterController : Controller
    {
        private readonly ILogger _logger;
        private readonly IParticipantRepository _participantRepository;

        public RegisterController(IParticipantRepository participantRepository, ILogger<RegisterController> logger)
        {
            _participantRepository = participantRepository;
            _logger = logger;            
        }
        
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("store")]
        public IActionResult Store(ParticipantRegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Participant participant = _participantRepository.FindUniqueByEmail(model.Email);

                    if (participant != null)
                    {
                        ModelState.AddModelError("Email", "E-mail já cadastrado");

                        return View("Index", model);
                    }

                    participant = new Participant
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Birthdate = model.Birthdate,
                        Gender = model.Gender,
                        Password = HashExtension.Create(model.Password, Environment.GetEnvironmentVariable("AUTH_SALT")),
                        CreatedAt = DateTime.Now
                    };

                    _participantRepository.Add(participant);
                    _participantRepository.SaveChanges();

                    TempData["Success"] = "Registro efetuado com sucesso!";

                    return RedirectToAction("Index", "Login");
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