using BlogVilla.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BlogVilla.ViewModels;
using BlogVilla.Util;
using BlogVilla.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace BlogVilla.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        { 
           return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthorizeActionFilter))]
        public IActionResult Profile(int userId)
        {
            
            var user = _userRepository.GetUserById(userId); // Implement this method in your repository

            if (user == null)
            {
                return NotFound();
            }

            bool isCurrentUser =( userId == HttpContext.Session.GetInt32("userId")); // Get the current user's ID from the session

            var viewModel = new ProfileViewModel
            {
                User = user,
                IsCurrentUser = isCurrentUser
            };

            return View(viewModel);
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthorizeActionFilter))]
        public IActionResult EditProfile()
        {
            
            int userId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault();
            // Fetch user by userId from the database
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Prepare the view model with the existing user data
            var viewModel = new EditProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ExistingProfilePhoto = user.ProfilePhoto,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthorizeActionFilter))]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            var user = _userRepository.GetUserById(model.Id);

            if (ModelState.IsValid)
            {
                // Fetch user from the 
                if (user == null)
                {
                    return NotFound();
                }

                // Update user details
                user.Username = model.Username;
                user.Email = model.Email;

                // If a new profile photo is uploaded, save it
                if (model.ProfilePhoto != null)
                { 
                
                    string oldPhotoPath = Path.Combine(_environment.WebRootPath, user.ProfilePhoto);
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                        
                    // Save the new profile photo
                    string newPhotoFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePhoto.FileName;
                    string newPath = Path.Combine(_environment.WebRootPath, "uploads", newPhotoFileName);

                    using (var fileStream = new FileStream(newPath, FileMode.Create))
                    {
                        model.ProfilePhoto.CopyTo(fileStream);
                    }

                    // Update the user entity with the new photo path
                    user.ProfilePhoto = "/uploads/" + newPhotoFileName;
                }

                // Save changes to the database
                _userRepository.UpdateUser(user);

                Message.SetMessage(HttpContext, "Profile updated successfully.", "success");

                return RedirectToAction("Profile", new { userId = user.Id });// Redirect to profile page
            }

            model.ExistingProfilePhoto = user.ProfilePhoto;

            return View(model); // If validation fails, reload the form with validation messages
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthorizeActionFilter))]
        public IActionResult DeleteUser(int userId)
        {
            // Find the user in the database
            var user = _userRepository.GetUserById(userId);

            // Check if the user exists
            if (user == null)
            {
                return NotFound();
            }

            // Optionally: Delete the user's profile photo from the server if it exists
            if (!string.IsNullOrEmpty(user.ProfilePhoto))
            { 
            
                if (System.IO.File.Exists(user.ProfilePhoto))
                {
                    System.IO.File.Delete(user.ProfilePhoto); // Delete the old profile photo
                }
            }

            // Remove the user from the database
            _userRepository.DeleteUser(user.Id);

            if (user.Id == HttpContext.Session.GetInt32("userId")) Auth.Logout(HttpContext);

            Message.SetMessage(HttpContext, "Your account has been deleted successfully.", "success");

            // Redirect to a suitable page, e.g., a list of users or home page
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthorizeActionFilter))]
        public IActionResult AdminActions()
        {
            var userId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault();

            var user = _userRepository.GetUserById(userId);

            ViewBag.user = user;
            
            var actions = _userRepository.GetAdminActionsByUserId(userId);

            return View(actions);
        }

    }
}
