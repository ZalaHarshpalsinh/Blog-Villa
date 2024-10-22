        using BlogVilla.Models;
using BlogVilla.Repositories;
using BlogVilla.Util;
using BlogVilla.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogVilla.Controllers
{
    [ServiceFilter(typeof(AuthorizeActionFilter))]
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;

        public BlogController(IBlogRepository blogRepository, IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _blogRepository = blogRepository;
            _userRepository = userRepository;
            _environment = environment;
        }

        // GET: Blog/Create
        [HttpGet]
        public IActionResult CreateBlog()
        {
            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        public IActionResult CreateBlog(BlogUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                int userId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault();

                string photoPath = null;

                Console.WriteLine(model.CoverPhoto);

                if (model.CoverPhoto != null && model.CoverPhoto.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverPhoto.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.CoverPhoto.CopyTo(fileStream);
                    }

                    photoPath = "/uploads/" + uniqueFileName; // Store the relative path
                }

                var blog = new Blog
                {
                    Title = model.Title,
                    Content = model.Content,
                    CoverPhoto = photoPath,
                    IsDraft = model.IsDraft, // Publish or save as draft
                    IsCanceled = false, // New blogs are not canceled by default
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    AuthorId = userId
                };

                //Console.WriteLine(blog.IsDraft);
                _blogRepository.AddBlog(blog);

                Message.SetMessage(HttpContext, "Your blog has been successfulyl created.", "success");

                if (blog.IsDraft)
                {
                    return RedirectToAction("UserBlogs", "Blog", new { userId = userId, tab = "drafts" }); // Redirect to blog list page
                }
                else
                {
                    return RedirectToAction("UserBlogs", "Blog", new { userId = userId}); // Redirect to blog list page
                }
            }

            return View(model); // If validation fails, redisplay the form
        }

        [HttpGet]
        public IActionResult UserBlogs(int userId, string tab = "published")
        {
            var user = _userRepository.GetUserById(userId);

            // Get the current logged-in user's ID from the session
            var currentUserId = HttpContext.Session.GetInt32("userId");

            // Check if the logged-in user is viewing their own blogs
            bool isCurrentUser = currentUserId != null && currentUserId == userId;

            // Fetch the filtered blogs based on the tab (Published, Drafts, Canceled)
            var blogs = _blogRepository.GetBlogsByUser(userId, tab);

            // Create the view model
            var model = new UserBlogsViewModel
            {
                Owner = user,
                Blogs = blogs,
                IsCurrentUser = isCurrentUser,
                SelectedTab = tab
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult BlogDetails(int id)
        {
            var blog = _blogRepository.GetBlogById(id); // Get blog from repository

            var userId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault();
            var user = _userRepository.GetUserById(userId);

            ViewBag.IsLikedByUser = blog.Likes.Any(l => l.UserId == userId);

            return View(blog);
        }

        [HttpGet]
        public IActionResult EditBlog(int id)
        {
            var blog = _blogRepository.GetBlogById(id);
            if (blog == null)
            {
                return NotFound();
            }

            var viewModel = new EditBlogViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                IsDraft = blog.IsDraft,
                ExistingCoverPhoto = blog.CoverPhoto
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditBlog(EditBlogViewModel model)
        {
            var blog = _blogRepository.GetBlogById(model.Id);

            model.ExistingCoverPhoto = blog.CoverPhoto;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (blog == null)
            {
                return NotFound();
            }

            blog.Title = model.Title;
            blog.Content = model.Content;
            blog.IsDraft = model.IsDraft;
            blog.UpdatedAt = DateTime.Now; // Update timestamp

            if (model.CoverPhoto != null)
            {

                string oldPhotoPath = Path.Combine(_environment.WebRootPath, blog.CoverPhoto);
                if (System.IO.File.Exists(oldPhotoPath))
                {
                    System.IO.File.Delete(oldPhotoPath);
                }

                // Save the new profile photo
                string newPhotoFileName = Guid.NewGuid().ToString() + "_" + model.CoverPhoto.FileName;
                string newPath = Path.Combine(_environment.WebRootPath, "uploads", newPhotoFileName);

                using (var fileStream = new FileStream(newPath, FileMode.Create))
                {
                    model.CoverPhoto.CopyTo(fileStream);
                }

                // Update the user entity with the new photo path
                blog.CoverPhoto = "/uploads/" + newPhotoFileName;
            }

            _blogRepository.UpdateBlog(blog);

            Message.SetMessage(HttpContext, "Your blog has been successfulyl Updated.", "success");

            return RedirectToAction("BlogDetails", new { id = blog.Id });
        }

        [HttpPost]
        public IActionResult DeleteBlog(int id)
        {
            var blog = _blogRepository.GetBlogById(id);
            if (blog == null)
            {
                return NotFound();
            }

            // Ensure that only the author or admin can delete the blog
            var currentUserId = HttpContext.Session.GetInt32("userId"); // Assuming session-based 

            if (blog.AuthorId != currentUserId)
            {
                return Forbid(); // Return 403 if not the author or admin
            }

            _blogRepository.DeleteBlog(blog.Id);

            Message.SetMessage(HttpContext, "Your blog has been successfulyl Deleted.", "success");

            if (blog.IsCanceled)
            {
                return RedirectToAction("UserBlogs", "Blog", new { userId = currentUserId, tab = "canceled" }); // Redirect to blog list page
            }
            else if (blog.IsDraft)
            {
                return RedirectToAction("UserBlogs", "Blog", new { userId = currentUserId, tab = "drafts" }); // Redirect to blog list page
            }
            else
            {
                return RedirectToAction("UserBlogs", "Blog", new { userId = currentUserId }); // Redirect to blog list page
            }
        }

        [HttpPost]
        public IActionResult RemoveBlog(int id)
        {
            var blog = _blogRepository.GetBlogById(id);
            if (blog == null)
            {
                return NotFound();
            }

            // Ensure that only the author or admin can delete the blog
            var currentUserId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault(); // Assuming session-based 
            var currentUser = _userRepository.GetUserById(currentUserId);
            if (!currentUser.IsAdmin)
            {
                return Forbid(); // Return 403 if not the author or admin
            }

            _userRepository.AddBlogCancelLog(blog.Id, currentUserId);
            _blogRepository.RemoveBlog(blog.Id);

            Message.SetMessage(HttpContext, "Target blog has been successfulyl removed from the Public view.", "success");

            return RedirectToAction("UserBlogs", "Blog", new { userId = blog.AuthorId }); // Redirect to blog 
        }

        public IActionResult BlogList(string searchQuery, int page = 1)
        {
            const int PageSize = 10; // Number of blogs per page
            int totalBlogs;

            // Fetch blogs with search and pagination
            var blogs = _blogRepository.GetBlogs(searchQuery, page, PageSize, out totalBlogs);

            // Create the ViewModel for rendering
            var viewModel = new BlogListViewModel
            {
                SearchQuery = searchQuery,
                Blogs = blogs,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalBlogs / (double)PageSize)
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ToggleLike(int blogId)
        {
            //Console.WriteLine(blogId);

            int userId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault(); // Get current user ID from session or viewbag
            
            _blogRepository.ToggleLike(blogId, userId); // Toggle the like

            return RedirectToAction("BlogDetails", new { id = blogId });
        }

        [HttpPost]
        public IActionResult AddComment(int blogId, string commentText)
        {
            int userId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault(); // Get current user ID
            _blogRepository.AddComment(blogId, userId, commentText);

            Message.SetMessage(HttpContext, "Comment added successfully.", "success");

            return RedirectToAction("BlogDetails", new { id = blogId });
        }

        [HttpPost]
        public IActionResult DeleteComment(int commentId)
        {
            var comment = _blogRepository.GetCommentById(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            // Ensure that only the author or admin can delete the blog
            var currentUserId = (HttpContext.Session.GetInt32("userId")).GetValueOrDefault(); // Assuming session-based 
            var currentUser = _userRepository.GetUserById(currentUserId);
            if (!currentUser.IsAdmin && currentUserId != comment.UserId)
            {
                return Forbid(); // Return 403 if not the author or admin
            }

            if (currentUserId != comment.UserId)
            {
                _userRepository.AddCommentDeleteLog(comment, currentUserId);
            }

            _blogRepository.DeleteComment(comment.Id);

            Message.SetMessage(HttpContext, "Comment deleted successfully.", "success");

            return RedirectToAction("BlogDetails", new { id = comment.Blog.Id });
        }
    }
}
