using BlogVilla.Models;
using BlogVilla.ViewModels;
using BlogVilla.Repositories;

using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BlogVilla.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BlogVilla.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        private const string AdminSecretCode = "makeadmin"; // Admin secret code

        public AuthController(IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if(Auth.IsLoggedIn(HttpContext))
            {
                Message.SetMessage(HttpContext, "Can't access that page while logged in. Logout first.", "error");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (Auth.IsLoggedIn(HttpContext))
            {
                Message.SetMessage(HttpContext, "Can't access that page while logged in. Logout first.", "error");
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Check if the username or email is already taken
                if (_userRepository.IsUsernameTaken(model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken");
                    return View(model);
                }

                if (_userRepository.IsEmailTaken(model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already taken");
                    return View(model);
                }

                // Handle profile photo upload
                string photoPath = null;

                if (model.ProfilePhoto != null && model.ProfilePhoto.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePhoto.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfilePhoto.CopyTo(fileStream);
                    }

                    photoPath = "/uploads/" + uniqueFileName; // Store the relative path
                }

                // Determine if the user should be an admin
                bool isAdmin = model.AdminSecretCode == AdminSecretCode;

                if(model.AdminSecretCode != null && !isAdmin)
                {
                    ModelState.AddModelError("AdminSecretCode", "Invalid code.");
                    return View(model);
                }

                // Create a new user object
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password, // In a real-world app, you should hash the password
                    IsAdmin = isAdmin,
                    ProfilePhoto = photoPath
                };

                // Save the user to the database
                _userRepository.AddUser(user);

                Message.SetMessage(HttpContext, "Registration Successful. Please Login.", "success");

                // Redirect to a login page or home page
                return RedirectToAction("Login", "Auth");
            }

            //Console.WriteLine("Reg failed");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (Auth.IsLoggedIn(HttpContext))
            {
                Message.SetMessage(HttpContext, "Can't access that page while logged in. Logout first.", "error");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (Auth.IsLoggedIn(HttpContext))
            {
                Message.SetMessage(HttpContext, "Can't access that page while logged in. Logout first.", "error");
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var user = _userRepository.FindByUsernameAndPassword(model.Username, model.Password);
                if (user != null)
                {
                    // Set session data or any required authentication logic
                    Auth.Login(HttpContext, user);

                    Message.SetMessage(HttpContext, "Login Successful.", "success");

                    // Redirect to a landing page or home page
                    return RedirectToAction("Profile", "Home", new {userId = user.Id});
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            // Clear session data on logout
            Auth.Logout(HttpContext);

            Message.SetMessage(HttpContext, "Logout Successful.", "success");

            return RedirectToAction("Login", "Auth");
        }
    }
}
