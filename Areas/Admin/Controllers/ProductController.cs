using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Areas.Admin.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using Promotion.Extensions;

namespace Promotion.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/products")]
    [Authorize(AuthenticationSchemes = "PromotionScheme")]
    public class ProductController : Controller
    {
        private readonly ILogger _logger;       
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            IProductRepository productRepository, 
            ILogger<ProductController> logger,
            IWebHostEnvironment environment)
        {
            _productRepository = productRepository;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index(string search, int pageNumber)
        {
            List<Product> products = new List<Product>();

            if (!String.IsNullOrEmpty(search))
            {
                products = _productRepository.Search(search).ToList();
            }
            else
            {
                products = _productRepository.GetAll().ToList();
            }

            int pageSize = 10;
            pageNumber = (pageNumber > 0) ? pageNumber :  1;

            return View(PaginatorExtension<Product>.CreateAsync(products, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("store")]
        public async Task<IActionResult> Store(ProductCreateViewModel model)
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

                    var imageMbName = GetUniqueFileName(model.ImageMobile.FileName);
                    uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    filePath = Path.Combine(uploads,imageMbName);
                    using (var steam = System.IO.File.Create(filePath))
                    {
                        await model.ImageMobile.CopyToAsync(steam);
                    }

                    Product product = new Product
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Image = imageName,
                        ImageMobile = imageMbName,
                        CreatedAt = DateTime.Now
                    };

                    _productRepository.Add(product);
                    _productRepository.SaveChanges();

                    TempData["Success"] = "Produto cadastrado com sucesso!";
                    
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Erro ao cadastrar produto";
                _logger.LogError("Product store error: " + exception);
            }

            return View("Create", model);
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            Product product = _productRepository.GetById(id);

            if (product != null)
            {
                ProductEditViewModel item = new ProductEditViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description
                };

                ViewBag.Image = product.Image;
                ViewBag.ImageMobile = product.ImageMobile;

                return View("Edit", item);
            }

            TempData["Error"] = "Produto não encontrado.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(ProductEditViewModel model, int id)
        {
            Product product = _productRepository.GetById(id);

            if (product != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        product.Name = model.Name;
                        product.Description = model.Description;

                        if (model.Image != null)
                        {
                            var imageName = GetUniqueFileName(model.Image.FileName);
                            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                            var filePath = Path.Combine(uploads,imageName);
                            using (var steam = System.IO.File.Create(filePath))
                            {
                                await model.Image.CopyToAsync(steam);
                            }

                            filePath = Path.Combine(uploads,product.Image);
                            System.IO.File.Delete(filePath);

                            product.Image = imageName;
                        }

                        if (model.ImageMobile != null)
                        {
                            var imageMbName = GetUniqueFileName(model.ImageMobile.FileName);
                            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                            var filePath = Path.Combine(uploads,imageMbName);
                            using (var steam = System.IO.File.Create(filePath))
                            {
                                await model.ImageMobile.CopyToAsync(steam);
                            }

                            filePath = Path.Combine(uploads,product.ImageMobile);
                            System.IO.File.Delete(filePath);

                            product.ImageMobile = imageMbName;
                        }

                        _productRepository.Update(product);
                        _productRepository.SaveChanges();

                        TempData["Success"] = "Produto atualizado com sucesso!";
                        
                        return RedirectToAction("Edit", new { id = product.Id });
                    }
                }
                catch (Exception exception)
                {
                    TempData["Error"] = "Erro ao atualizar o produto";
                    _logger.LogError("Product edit error: " + exception);
                }
            }
            else
            {
                TempData["Error"] = "Produto não encontrado.";
            }

            return RedirectToAction("Edit", model);
        }

        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Product product = _productRepository.GetById(id);

            if (product != null)
            {
                ViewBag.Product = product;

                return View("Delete", product);
            }

            TempData["Error"] = "Produto não encontrado.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            Product product = _productRepository.GetById(id);

            if (product != null)
            {
                try
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    var filePath = Path.Combine(uploads, product.Image);

                    System.IO.File.Delete(filePath);

                    uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    filePath = Path.Combine(uploads, product.ImageMobile);

                    System.IO.File.Delete(filePath);

                    _productRepository.Remove(product.Id);
                    _productRepository.SaveChanges();

                    TempData["Success"] = "Produto removido com sucesso!";
                    return RedirectToAction("Index");
                }
                catch (Exception exception)
                {
                    TempData["Error"] = "Erro ao remover produto.";
                    _logger.LogError("Product remove error: " + exception);
                    return RedirectToAction("Index");        
                }
            }

            TempData["Error"] = "Produto não encontrado.";

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