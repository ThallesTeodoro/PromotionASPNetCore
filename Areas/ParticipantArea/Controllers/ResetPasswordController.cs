using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Models;
using Promotion.Interfaces;
using Promotion.Extensions;
using Promotion.ViewModel;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Text;
using Promotion.Filters;
using Promotion.Areas.ParticipantArea.ViewModel;

namespace Promotion.Areas.ParticipantArea.Controllers
{
    [Area("ParticipantArea")]
    [Route("participant/reset-password")]
    [AnonymousOnlyFilter("Index", "Participant")]
    public class ResetPasswordController : Controller
    {
        private readonly ILogger<ResetPasswordController> _logger;
        private readonly IParticipantRepository _participantRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;

        public ResetPasswordController(
            ILogger<ResetPasswordController> logger,
            IParticipantRepository participantRepository,
            IPasswordResetRepository passwordResetRepository)
        {
            _logger = logger;
            _participantRepository = participantRepository;
            _passwordResetRepository = passwordResetRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index(string email, string token)
        {
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel
            {
                Token = token,
                Email = email,
            };

            return View("Index", viewModel);
        }

        [HttpPost]
        [Route("reset")]
        public IActionResult Reset(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)                
                {
                    PasswordReset passwordReset = _passwordResetRepository.FindUniqueByEmail(model.Email);

                    if (passwordReset != null && passwordReset.Token.Equals(model.Token))
                    {
                        Participant participant = _participantRepository.FindUniqueByEmail(model.Email);

                        participant.Password = HashExtension.Create(model.Password, Environment.GetEnvironmentVariable("AUTH_SALT"));

                        _participantRepository.Update(participant);
                        _participantRepository.SaveChanges();
                        
                        _passwordResetRepository.Remove(passwordReset.Id);
                        _passwordResetRepository.SaveChanges();

                        TempData["Success"] = "Senha atualizada com sucesso!";

                        return RedirectToAction("Index", "Login");
                    }

                    ModelState.AddModelError("Email", "E-mail não encontrado ou token expirado!");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Reset password error: " + exception);
                TempData["Error"] = "Internal server error";
            }

            return View("Index", model);
        }
    }
}
