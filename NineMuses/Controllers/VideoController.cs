﻿using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NineMuses.Models;
using NineMuses.Repositories;
using NineMuses.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace NineMuses.Controllers
{
    public class VideoController : Controller
    {
        private UserRepository _userRepo = new UserRepository();
        private VideoRepository _videoRepo = new VideoRepository();

        public ActionResult Upload()
        {
            if (Session["UserID"] == null)
            {

                return RedirectToAction("SignIn", "User", new { returnUrl = Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped).ToString() });
            }

            else
            {
                return View();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Upload(UploadVideoViewModel model)
        {
            if (model.VideoFile == null)
            {
                ModelState.AddModelError("", "Video file is required");
                return View(model);
            }

            if (model.Video.Title == null)
            {
                ModelState.AddModelError("", "Title is required");
                return View(model);
            }

            var id = _videoRepo.Upload(model);

            if (id != 0)
            {
                return RedirectToAction("View", "Video", new { id = id });
            }

            else
            {
                ModelState.AddModelError("", "Something went wrong with the Upload");

                return View(model);
            }

        }

        public ActionResult View(int id)
        {
            var model = new ViewVideoViewModel();
            if (id != 0)
            {
                model.Video = _videoRepo.GetVideo(id, true);

                SqlCommand command = new SqlCommand()
                {
                    CommandText = "spGetUserVideos",
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@id", id);
                model.VideoList = _videoRepo.GetVideoList(command);
            }

            return View(model);
        }
    }
}