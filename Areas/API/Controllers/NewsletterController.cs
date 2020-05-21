using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Interfaces;
using Promotion.Areas.API.ViewModel;
using Promotion.Models;
using System.Collections.Generic;

namespace Promotion.Areas.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/newsletter")]
    public class NewsletterController : ControllerBase
    {
        private readonly INewsletterRepository _newsletterRepository;
        private readonly ILogger _logger;

        public NewsletterController(INewsletterRepository newsletterRepository, ILogger<NewsletterController> logger)
        {
            _newsletterRepository = newsletterRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] NewsletterCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Newsletter newsletter = _newsletterRepository.FindUniqueByEmail(model.Email);

                    if (newsletter != null)
                    {
                        ModelState.AddModelError("Email", "Email já cadastrado");

                        IDictionary<string, string> error = new Dictionary<string, string>();
                        error.Add("email", "Email já cadastrado");

                        return StatusCode(422, new {
                            StatusCode = 442,
                            Message = "Unprocessable entity",
                            Errors = error
                        });
                    }

                    Newsletter obj = new Newsletter
                    {
                        Email = model.Email,
                        Name = model.Name,
                        CreatedAt = DateTime.Now,
                    };

                    _newsletterRepository.Add(obj);
                    _newsletterRepository.SaveChanges();

                    HttpContext.Response.StatusCode = 201;

                    return StatusCode(201, new {
                        StatusCode = 201,
                        Message = "Created",
                        Data = obj
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
                _logger.LogError("Newsletter api register error: " + exception);
                return StatusCode(500);
            }
        }
    }
}