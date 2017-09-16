using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TecoRP_Website.Models;

namespace TecoRP_Website.Areas.AdminPanel.Controllers
{
    public class PickController : Controller
    {
        Models.TecoRPEntities db = new TecoRPEntities();
        // GET: AdminPanel/Pick
        public ActionResult Index()
        {
            return View();
        }
        static List<AspNetUsers> peoples = new List<AspNetUsers>();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string number)
        {
            peoples.Clear();
            Random r = new Random();
            int numberOfPeople = 0;
            if (int.TryParse(number, out numberOfPeople))
            {
                var playerPool = db.Applications.Where(w => w.IsApproved == true).ToList();
                for (int i = 0; i < numberOfPeople; i++)
                {
                    var picked = playerPool[r.Next(0, playerPool.Count)];
                    var user = db.AspNetUsers.FirstOrDefault(x => x.SocialClubName == picked.SocialClubName);
                    peoples.Add(user);
                }
                    return RedirectToAction("Result");

            }
            return RedirectToAction("Index");
        }

        public ActionResult Result()
        {
            return View(peoples);
        }
    }
}