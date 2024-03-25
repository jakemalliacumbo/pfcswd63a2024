using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Repositories;

namespace Presentation.Controllers
{
    public class BlogsController : Controller
    {
        private BlogsRepository blogsRepository;
        private BucketRepository bucketRepository;
        private PubSubRepository pubSubRepository;
        public BlogsController(BlogsRepository _blogsRepository, 
            BucketRepository _bucketRepository,
            PubSubRepository _pubsubRepository) { 
            blogsRepository= _blogsRepository;
            bucketRepository= _bucketRepository;
            pubSubRepository = _pubsubRepository;
        }
        public async Task<IActionResult> Index()
        {
            List<Blog> list = await blogsRepository.GetBlogs();
            return View(list);
        }

        public async Task<IActionResult> Search(string search)
        {
            List<Blog> list = await blogsRepository.GetBlogs(search);
            return View("Index",list);
        }


        [Authorize]
        [HttpGet] //this will load the page to the user to be able to type in blog data
        public IActionResult Create()
        { return View(); }

        [Authorize]
        [HttpPost] //this is triggered after the user submits the blogs data and you process it
        public async Task<IActionResult> Create(Blog blog, IFormFile file)
        {
            string newFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
            MemoryStream fileAsAStream = new MemoryStream();
            file.CopyTo(fileAsAStream);

            //using (var s = file.OpenReadStream())
            //{
            //    s.CopyTo(fileAsAStream);
            //}

            await bucketRepository.UploadFile(fileAsAStream, newFilename);
            bucketRepository.GrantPermissionToFile(newFilename, "jmalliacumbo@gmail.com");

            //finegrained bucket = https://storage.cloud.google.com/pfc-jmc-2024-fg/laptop.png;
            //uniform bucket = https://storage.googleapis.com/pfc-jmc-2024/{newFilename}


            blog.Uri = $"https://storage.cloud.google.com/pfc-jmc-2024-fg/{newFilename}";
            blog.Author = User.Identity.Name;
            blog.DateCreated = Timestamp.FromDateTime(DateTime.UtcNow);
            blog.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);

            blogsRepository.Add(blog);

            return View(blog);
        }

        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
             await  blogsRepository.DeleteBlog(id);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(string id)
        {
            var list = await blogsRepository.GetBlogs();
            return View(list.SingleOrDefault(x => x.Id == id));
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Edit(Blog blog)
        {
            await blogsRepository.UpdateBlog(blog);
            return RedirectToAction("Index");
        
        }

        public async Task<IActionResult> CreateReport(string blogId)
        {
            await pubSubRepository.PublishBlog(blogId);
            return RedirectToAction("Index");
        }
    }
}
