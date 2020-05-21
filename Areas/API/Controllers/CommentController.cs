using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Areas.API.ViewModel;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Promotion.Areas.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/contact")]
    public class CommentController : ControllerBase
    {
        private readonly ILogger _logger;

        public CommentController(ILogger<NewsletterController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("")]
        public IActionResult Send([FromBody] ContactCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var message = new MimeMessage();
                    message.To.Add(new MailboxAddress("Promotion", "promotion@email.com"));
                    message.From.Add(new MailboxAddress("Contact Promotion", "contactpromotion@contact.com"));
                    message.Subject = "Promotion - Comentátio";
                    message.Body = new TextPart(TextFormat.Html)
                    {
                        Text = "Perfil do GitHub: " + model.Profile + "<br>Mensagem: " + model.Message
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.mailtrap.io", 587, false);
                        client.Authenticate("", "");
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    return StatusCode(200, new {
                        StatusCode = 200,
                        Message = "Enviado com sucesso!",
                    });
                }

                IDictionary<string, string> errorList = new Dictionary<string, string>();

                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Any())
                    {
                        errorList.Add(error.Key, error.Value.Errors.First().ErrorMessage);
                    }
                }

                return StatusCode(422, new {
                    StatusCode = 422,
                    Message = "Unprocessable entity",
                    Error = errorList
                });
            }
            catch (Exception exception)
            {
                _logger.LogError("Contact send error: " + exception);
                return StatusCode(500);
            }
        }
    }
}