using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Models;
using Promotion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Promotion.ViewModel;
using System.Security.Claims;
using Promotion.Filters;

namespace Promotion.Controllers
{
    [Route("participation")]
    [Authorize(AuthenticationSchemes = "PromotionParticipantScheme")]
    [TypeFilter(typeof(OneParticipationFilter))]
    public class ParticipationController : Controller
    {
        private readonly ILogger<ParticipationController> _logger;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IParticipationRepository _participationRepository;

        public ParticipationController(
            ILogger<ParticipationController> logger,
            IEpisodeRepository episodeRepository,
            IParticipationRepository participationRepository)
        {
            _logger = logger;
            _episodeRepository = episodeRepository;
            _participationRepository = participationRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.Episodes = _episodeRepository.GetAll().ToList();

            return View();
        }

        [HttpPost]
        [Route("vote")]
        public IActionResult Vote(VoteViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Episode episode = _episodeRepository.GetById(model.EpisodeId);

                    if (episode == null)
                    {
                        ModelState.AddModelError("EpisodeId", "Episódio não encontrado.");
                        ViewBag.Episodes = _episodeRepository.GetAll().ToList();

                        return View("Index", model);
                    }

                    Participation participation = new Participation
                    {
                        ParticipantId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value),
                        EpisodeId = episode.Id,
                        CreatedAt = DateTime.Now
                    };

                    _participationRepository.Add(participation);
                    _participationRepository.SaveChanges();

                    TempData["Success"] = "Voto registrado com sucesso!";

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Internal server error";
                _logger.LogError("Register vote error: " + exception);
            }

            ViewBag.Episodes = _episodeRepository.GetAll().ToList();

            return View("Index", model);
        }
    }
}
