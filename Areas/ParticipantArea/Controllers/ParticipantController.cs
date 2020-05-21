using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;
using Promotion.Models;
using Promotion.Areas.ParticipantArea.ViewModel;
using Promotion.Extensions;

namespace Promotion.Areas.ParticipantArea.Controllers
{
    [Area("ParticipantArea")]
    [Route("participant")]
    [Authorize(AuthenticationSchemes = "PromotionParticipantScheme")]
    public class ParticipantController : Controller
    {
        private readonly ILogger _logger;
        private readonly IParticipantRepository _participantRepository;
        private readonly IParticipationRepository _participationRepository;

        public ParticipantController(
            IParticipantRepository participantRepository,
            ILogger<ParticipantController> logger,
            IParticipationRepository participationRepository)
        {
            _participantRepository = participantRepository;
            _logger = logger;
            _participationRepository = participationRepository;
        }

        public IActionResult Index()
        {
            int participantId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Participant participant = _participantRepository.GetById(participantId);
            ViewBag.Participant = participant;
            
            return View();
        }

        [HttpGet]
        [Route("edit")]
        public IActionResult Edit()
        {
            int participantId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Participant participant = _participantRepository.GetById(participantId);
            
            ParticipantEditViewModel viewModel = new ParticipantEditViewModel
            {
                Id = participant.Id,
                Name = participant.Name,
                Email = participant.Email,
                Birthdate = participant.Birthdate,
                Gender = participant.Gender
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [Route("update/{id}")]
        public IActionResult Update(ParticipantEditViewModel model, int id)
        {
            try
            {
                if (ModelState.IsValid)                
                {
                    int participantId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    if (participantId != id)
                    {
                        ModelState.AddModelError("Email", "Participante não encontrado.");

                        return View("Edit", model);
                    }

                    Participant participant = _participantRepository.GetById(id);

                    participant.Name = model.Name;
                    participant.Birthdate = model.Birthdate;
                    participant.Gender = model.Gender;

                    if (model.Password != null)
                    {
                        participant.Password = HashExtension.Create(model.Password, Environment.GetEnvironmentVariable("AUTH_SALT"));
                    }

                    _participantRepository.Update(participant);
                    _participantRepository.SaveChanges();

                    TempData["Success"] = "Dados do participante atualizado com sucesso!";

                    return RedirectToAction("Edit");
                }
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Internal server error";
                _logger.LogError("Participant update error: " + exception);
            }

            return View("Edit", model);
        }

        [HttpGet]
        [Route("configuration")]
        public IActionResult Configuration()
        {
            int participantId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Participant participant = _participantRepository.GetById(participantId);

            ViewBag.Participant = participant;
            ViewBag.ParticipationsCount = _participationRepository.CountParticipations(participant.Id);
            
            return View();
        }

        [HttpGet]
        [Route("delete")]
        public IActionResult Delete()
        {
            int participantId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Participant participant = _participantRepository.GetById(participantId);
            ViewBag.Participant = participant;
            
            return View();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<IActionResult> Remove()
        {
            try
            {
                int participantId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                
                await this.Logout();

                _participantRepository.Remove(participantId);
                _participantRepository.SaveChanges();

                TempData["Success"] = "Cadastro removido com sucesso!";

                return RedirectToAction("Index", "Login");
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Erro ao remover cadastro!";
                _logger.LogError("Participant remove error: " + exception);
            }
            
            return RedirectToAction("Delete");
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("PromotionParticipantScheme");

            return RedirectToAction("Index", "Login");
        }
    }
}