using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotion.Areas.Admin.ViewModel;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Extensions;

namespace Promotion.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/users")]
    [Authorize(AuthenticationSchemes = "PromotionScheme")]
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index(string search, int pageNumber)
        {
            int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<User> users = new List<User>();

            if (!String.IsNullOrEmpty(search))
            {
                users = _userRepository.Search(search, userId).ToList();
            }
            else
            {
                users = _userRepository.GetAllWithoutId(userId).ToList();
            }

            int pageSize = 10;
            pageNumber = (pageNumber > 0) ? pageNumber :  1;

            return View(PaginatorExtension<User>.CreateAsync(users, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("store")]
        public IActionResult Store(UserCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User userEmail = _userRepository.FindUniqueByEmail(model.Email);

                    if (userEmail != null)
                    {
                        ModelState.AddModelError("Email", "E-mail já cadastrado");

                        return View("Create", model);
                    }

                    User user = new User
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Password = HashExtension.Create(model.Password, Environment.GetEnvironmentVariable("AUTH_SALT")),
                        CreatedAt = DateTime.Now
                    };

                    _userRepository.Add(user);
                    _userRepository.SaveChanges();
                    
                    TempData["Success"] = "Usuário registrado com sucesso!";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Erro ao realizar cadastro";
                _logger.LogError("User create error: " + exception);
            }

            return View("Create", model);
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            User user = _userRepository.GetById(id);
            
            int authUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (user != null && user.Id != authUserId)
            {
                UserEditViewModel model = new UserEditViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                };

                return View("Edit", model);
            }
            
            TempData["Error"] = "Usuário não encontrado.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("update/{id}")]
        public IActionResult Update(UserEditViewModel model, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = _userRepository.GetById(model.Id);

                    if (user != null)
                    {
                        user.Name = model.Name;
                        user.Email = model.Email;

                        if (model.Password != null)
                        {
                            user.Password = HashExtension.Create(model.Password, Environment.GetEnvironmentVariable("AUTH_SALT"));
                        }

                        _userRepository.Update(user);
                        _userRepository.SaveChanges();

                        TempData["Success"] = "Usuário atualizado com sucesso!";

                        return RedirectToAction("Edit", new { id = model.Id });
                    }
                    else
                    {
                        TempData["Error"] = "Usuário não encontrado.";
                    }
                }
            }
            catch (Exception exception)
            {
                TempData["Error"] = "Erro ao atualizar usuário.";
                _logger.LogError("User update error: " + exception);
            }

            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            User user = _userRepository.GetById(id);

            int authUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (user != null && user.Id != authUserId)
            {
                ViewBag.User = user;

                return View("Delete");
            }
            
            TempData["Error"] = "Usuário não encontrado.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            User user = _userRepository.GetById(id);

            int authUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if (user != null && user.Id != authUserId)
            {
                _userRepository.Remove(id);
                _userRepository.SaveChanges();

                TempData["Success"] = "Usuário removido com sucesso!";
            }
            else
            {
                TempData["Error"] = "Usuário não encontrado.";
            }

            return RedirectToAction("Index");
        }
    }
}