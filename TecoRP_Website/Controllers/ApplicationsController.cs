using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TecoRP_Website.Models;

namespace TecoRP_Website.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private TecoRPEntities db = new TecoRPEntities();

        //// GET: Applications
        //public ActionResult Index()
        //{
        //    var applications = db.Applications.FirstOrDefault(x => x.UserID == User.Identity.GetUserId());
        //    if (applications == null)
        //    {
        //        return RedirectToAction("New");
        //    }
        //    return View(db.Applications.ToList());
        //}

        // GET: Applications/Details/5
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var application = db.Applications.FirstOrDefault(x => x.UserID == userId );
            if (application == null)
            {
                return RedirectToAction("New");
            } 
            return View(application);
        }

        // GET: Applications/Create
        public ActionResult New(string param)
        {
            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            if (String.IsNullOrEmpty(user.SocialClubName))
            {
                return RedirectToAction("Configuration", "Home");
            }
            if (param=="new")
            {
                var userId = User.Identity.GetUserId();
                var app = db.Applications.FirstOrDefault(x => x.UserID == userId);
                app.Answers.Clear();
                db.Applications.Remove(app);
                db.SaveChanges();
            }
            ApplicationModel appModel = new Models.ApplicationModel();
            Random r = new Random();
            var question = db.Questions.ToList().OrderBy(x=>Guid.NewGuid());
            foreach (var item in question)
            {
                appModel.Answers.Add(new AnswerField(item.QuestionID, item.QuestionText, item.IsTextArea, item.Selection_A, item.Selection_B, item.Selection_C));
            }
            if (appModel.Answers.Count <=0)
            {
                return RedirectToAction("Index","Home");
            }
            return View(appModel);
        }

        // POST: Applications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "Answers")] ApplicationModel application)
        {     

            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            if (String.IsNullOrEmpty(user.SocialClubName))
            {
                return RedirectToAction("Configuration", "Home", new { Area = "" });
            }

            var app = new Applications
            {
                UserID = user.Id,
                Name = user.Email,
                SocialClubName = user.SocialClubName,
                RegisterDate = DateTime.Now,
            };

            foreach (var item in application.Answers)
            {
                app.Answers.Add(new Answers
                {
                    Answer = (byte)item.AnswerSelection,
                    AnswerText = item.AnswerText,
                    Questions = db.Questions.Find(item.QuestionId)
                });
            }
            db.Applications.Add(app);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Applications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applications applications = db.Applications.Find(id);
            if (applications == null)
            {
                return HttpNotFound();
            }
            return View(applications);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApplicationId,IsApproved,Name,SocialClubName,Contact,UserID,RegisterDate")] Applications applications)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applications).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applications);
        }

        // GET: Applications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Applications applications = db.Applications.Find(id);
            if (applications == null)
            {
                return HttpNotFound();
            }
            return View(applications);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Applications applications = db.Applications.Find(id);
            db.Applications.Remove(applications);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
