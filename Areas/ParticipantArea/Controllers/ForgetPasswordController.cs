using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Models;
using Promotion.Interfaces;
using Promotion.Extensions;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Text;
using Promotion.Filters;
using Promotion.Areas.ParticipantArea.ViewModel;

namespace Promotion.Areas.ParticipantArea.Controllers
{
    [Area("ParticipantArea")]
    [Route("participant/forgot-password")]
    [AnonymousOnlyFilter("Index", "Participant")]
    public class ForgetPasswordController : Controller
    {
        private readonly ILogger<ForgetPasswordController> _logger;
        private readonly IParticipantRepository _participantRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;

        public ForgetPasswordController(
            ILogger<ForgetPasswordController> logger,
            IParticipantRepository participantRepository,
            IPasswordResetRepository passwordResetRepository)
        {
            _logger = logger;
            _participantRepository = participantRepository;
            _passwordResetRepository = passwordResetRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("send")]
        public IActionResult Send(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Participant participant = _participantRepository.FindUniqueByEmail(model.Email);

                    if (participant == null)
                    {
                        ModelState.AddModelError("Email", "E-mail não encontrado na base de dados.");

                        return View("Index", model);
                    }
                    
                    StringBuilder builder = new StringBuilder();  
                    builder.Append(RandomString(4, true));
                    string hash = HashExtension.Create(builder.ToString(), Environment.GetEnvironmentVariable("AUTH_SALT"));                    
                    hash = hash.Replace(" ", String.Empty);

                    PasswordReset passwordReset = new PasswordReset
                    {
                        Email = model.Email,
                        Token = hash
                    };

                    PasswordReset old = _passwordResetRepository.FindUniqueByEmail(model.Email);

                    if (old != null)
                    {
                        _passwordResetRepository.Remove(old.Id);
                    }

                    _passwordResetRepository.Add(passwordReset);
                    _passwordResetRepository.SaveChanges();

                    var message = new MimeMessage();
                    message.To.Add(new MailboxAddress(participant.Name, model.Email));
                    message.From.Add(new MailboxAddress("Contact Promotion", "contactpromotion@contact.com"));
                    message.Subject = "Promotion - Reset Password";
                    message.Body = new TextPart(TextFormat.Html)
                    {
                        Text = "<strong>Olá!</strong>" + "<br>Clique no link para recuperar sua senha: " +
                            "<a href='https://localhost:5001/participant/reset-password?email=" + model.Email + "&token=" + hash +"' target='_blank'>Recuperar senha</a>"
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.mailtrap.io", 587, false);
                        client.Authenticate("", "");
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    TempData["Success"] = "Cheque sua caixa de e-mail!";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Contact send error: " + exception);
                return StatusCode(500);
            }

            return View("Index", model);
        }

        private string RandomString(int size, bool lowerCase)
        {  
            StringBuilder builder = new StringBuilder();  
            Random random = new Random();  
            char ch;  
            for (int i = 0; i < size; i++)  
            {  
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));  
                builder.Append(ch);  
            }  
            if (lowerCase)  
                return builder.ToString().ToLower();  
            return builder.ToString();  
        }  
    }
}
