using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Models;
using Promotion.Interfaces;

namespace Promotion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IEpisodeRepository _episodeRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IProductRepository productRepository,
            IEpisodeRepository episodeRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _episodeRepository = episodeRepository;
        }

        public IActionResult Index()
        {
            List<Product> products = _productRepository.GetAll().ToList();
            List<Episode> episodes = _episodeRepository.GetAll().ToList();

            ViewData["Products"] = products;
            ViewData["Episodes"] = episodes;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
