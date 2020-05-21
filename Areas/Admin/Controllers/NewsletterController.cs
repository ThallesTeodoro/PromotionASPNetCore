using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Extensions;
using System.Threading.Tasks;

namespace Promotion.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/newsletter")]
    [Authorize(AuthenticationSchemes = "PromotionScheme")]
    public class NewsletterController : Controller
    {
        private readonly INewsletterRepository _newsletterRepository;
        private readonly ILogger _logger;

        public NewsletterController(INewsletterRepository newsletterRepository, ILogger<NewsletterController> logger)
        {
            _newsletterRepository = newsletterRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index(string search, int pageNumber)
        {
            List<Newsletter> newsletters = new List<Newsletter>();

            if (!String.IsNullOrEmpty(search))
            {
                newsletters = _newsletterRepository.Search(search).ToList();
                pageNumber = 1;
            }
            else
            {
                newsletters = _newsletterRepository.GetAll().ToList();
            }

            int pageSize = 10;
            pageNumber = (pageNumber > 0) ? pageNumber :  1;

            return View(PaginatorExtension<Newsletter>.CreateAsync(newsletters, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("export")]
        public IActionResult Export()
        {
            try
            {
                List<Newsletter> newsletters = _newsletterRepository.GetAll().ToList();

                var builder = new StringBuilder();
                builder.AppendLine("ID,NOME,EMAIL,CRIADO EM");

                foreach (var item in newsletters)
                {
                    builder.AppendLine($"{item.Id},{item.Name},{item.Email},{item.CreatedAt}");
                }

                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "newsletter.csv");
            }
            catch (Exception exception)
            {
                _logger.LogError("Newsletter export error: " + exception);

                TempData["Error"] = "Erro ao exportar dados de newsletter";

                return RedirectToAction("Index");
            }
        }
    }
}