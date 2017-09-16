using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TecoRP.Database;
using Microsoft.AspNet.Identity;
using TecoRP_Website.Models;

namespace TecoRP_Website.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Models.TecoRPEntities db = new Models.TecoRPEntities();
        public ActionResult Index()
        {
            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            HomeIndexViewModel viewModel = new Models.HomeIndexViewModel();
            viewModel.OwnerSocialClubName = user.SocialClubName;
            viewModel.ConfigurationEnabled = String.IsNullOrEmpty(user.SocialClubName);
            return View(viewModel);
        }
        public ActionResult Configuration()
        {
            string userID = User.Identity.GetUserId();
            return View(db.AspNetUsers.Find(userID));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Configuration([Bind(Include = "Id,SocialClubName")] AspNetUsers _model)
        {
            if (ModelState.IsValid)
            {
                var edited = db.AspNetUsers.Find(_model.Id);
                edited.SocialClubName = _model.SocialClubName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(_model);
        }
        public ActionResult Skills()
        {
            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            if (String.IsNullOrEmpty(user.SocialClubName))
            {
                return RedirectToAction("Configuration");
            }

            var player = db_Accounts.GetOfflineUserDatas(user.SocialClubName,true);
            return View(player.JobAbilities);
        }
        public ActionResult Vehicles()
        {
            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            if (String.IsNullOrEmpty(user.SocialClubName))
            {
                return RedirectToAction("Configuration");
            }

            var vehicles = db_Vehicles.GetOfflinePlayerVehicles(user.SocialClubName);
            return View(vehicles);
        }
        public ActionResult MyCharacter()
        {
            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            if (String.IsNullOrEmpty(user.SocialClubName))
            {
                return RedirectToAction("Configuration");
            }

            return View(db_Accounts.GetOfflineUserDatas(user.SocialClubName, true));
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}