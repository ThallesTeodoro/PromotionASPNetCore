using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Areas.Admin.ViewModel;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using Promotion.Extensions;

namespace Promotion.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/episodes")]
    [Authorize(AuthenticationSchemes = "PromotionScheme")]
    public class EpisodeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IWebHostEnvironment _environment;

        public EpisodeController(
            IEpisodeRepository episodeRepository,
            ILogger<EpisodeController> logger,
            IWebHostEnvironment environment)
        {
            _episodeRepository = episodeRepository;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index(string search, int pageNumber)
        {
            List<Episode> episodes = new List<Episode>();

            if (!String.IsNullOrEmpty(search))
            {
                episodes = _episodeRepository.Search(search).ToList();
            }
            else
            {
                episodes = _episodeRepository.GetAll().ToList();
            }

            int pageSize = 10;
            pageNumber = (pageNumber > 0) ? pageNumber :  1;

            return View(PaginatorExtension<Episode>.CreateAsync(episodes, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("store")]
        public async Task<IActionResult> Store(EpisodeCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var imageName = GetUniqueFileName(model.Image.FileName);
                    var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    var filePath = Path.Combine(uploads,imageName);
                    using (var steam = System.IO.File.Create(filePath))
                    {
                        await model.Image.CopyToAsync(steam);
                    }

                    var thumb = GetUniqueFileName(model.Thumb.FileName);
                    uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    filePath = Path.Combine(uploads,thumb);
                    using (var steam = System.IO.File.Create(filePath))
                    {
                        await model.Thumb.CopyToAsync(steam);
                    }

                    Episode episode = new Episode
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Image = imageName,
                        Thumb = thumb,
                        VidelUrl = model.VidelUrl,
                        CreatedAt = DateTime.Now
                    };

                    _episodeRepository.Add(episode);
                    _episodeRepository.SaveChanges();
                    
                    TempData["Success"] = "Episódio cadastrado com sucesso!";
                    
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Erro ao cadastrar episódio";
                _logger.LogError("Episode store error: " + exception);
            }

            return View("Create", model);
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            Episode episode = _episodeRepository.GetById(id);

            if (episode != null)
            {
                EpisodeEditViewModel model = new EpisodeEditViewModel
                {
                    Id = episode.Id,
                    Title = episode.Title,
                    Description = episode.Description,
                    VidelUrl = episode.VidelUrl
                };

                ViewBag.Image = episode.Image;
                ViewBag.Thumb = episode.Thumb;

                return View("Edit", model);
            }

            TempData["Error"] = "Episódio não encontrado.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(EpisodeEditViewModel model, int id)
        {
            Episode episode = _episodeRepository.GetById(id);

            if (episode != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        episode.Title = model.Title;
                        episode.Description = model.Description;
                        episode.VidelUrl = model.VidelUrl;
                        episode.UpdatedAt = DateTime.Now;

                        if (model.Image != null)
                        {
                            var imageName = GetUniqueFileName(model.Image.FileName);
                            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                            var filePath = Path.Combine(uploads,imageName);
                            using (var steam = System.IO.File.Create(filePath))
                            {
                                await model.Image.CopyToAsync(steam);
                            }

                            filePath = Path.Combine(uploads,episode.Image);
                            System.IO.File.Delete(filePath);

                            episode.Image = imageName;
                        }

                        if (model.Thumb != null)
                        {
                            var imageName = GetUniqueFileName(model.Thumb.FileName);
                            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                            var filePath = Path.Combine(uploads,imageName);
                            using (var steam = System.IO.File.Create(filePath))
                            {
                                await model.Thumb.CopyToAsync(steam);
                            }

                            filePath = Path.Combine(uploads,episode.Thumb);
                            System.IO.File.Delete(filePath);

                            episode.Thumb = imageName;
                        }

                        _episodeRepository.Update(episode);
                        _episodeRepository.SaveChanges();

                        TempData["Success"] = "Episódio atualizado com sucesso!";
                        
                        return RedirectToAction("Edit", new { id = episode.Id });
                    }

                    return View("Edit", model);
                }
                catch (Exception exception)
                {
                    TempData["Error"] = "Erro ao atualizar episódio.";
                    _logger.LogError("Episode update error: " + exception);

                    return RedirectToAction("Edit", new { id = episode.Id });
                }
            }

            TempData["Error"] = "Episódio não encontrado.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Episode episode = _episodeRepository.GetById(id);

            if (episode != null)
            {
                ViewBag.Episode = episode;

                return View("Delete");
            }

            TempData["Error"] = "Episódio não encontrado.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            Episode episode = _episodeRepository.GetById(id);

            if (episode != null)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads, episode.Image);

                System.IO.File.Delete(filePath);

                filePath = Path.Combine(uploads, episode.Thumb);

                System.IO.File.Delete(filePath);

                _episodeRepository.Remove(id);
                _episodeRepository.SaveChanges();

                TempData["Success"] = "Episódio removido com sucesso";
            }
            else
            {
                TempData["Error"] = "Episódio não encontrado.";
            }

            return RedirectToAction("Index");
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                    + "_"
                    + Guid.NewGuid().ToString().Substring(0,4)
                    + Path.GetExtension(fileName);
        }
    }
}