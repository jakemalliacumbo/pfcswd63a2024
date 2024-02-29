﻿using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Repositories;

namespace Presentation.Controllers
{
    public class BlogsController : Controller
    {
        private BlogsRepository blogsRepository;

        public BlogsController(BlogsRepository _blogsRepository) { 
            blogsRepository= _blogsRepository;
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
        public IActionResult Create(Blog blog)
        {
            blog.Author = User.Identity.Name;
            blog.DateCreated = Timestamp.FromDateTime(DateTime.UtcNow);
            blog.DateUpdated = Timestamp.FromDateTime(DateTime.UtcNow);

            blogsRepository.Add(blog);


          


            return View(blog);
        }
    }
}
