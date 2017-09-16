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

namespace TecoRP_Website.Areas.AdminPanel.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ApplicationController : Controller
    {
        private TecoRPEntities db = new TecoRPEntities();

        // GET: AdminPanel/Applications
        public ActionResult Index(string filter)
        {
            switch (filter)
            {
                case "awaiting":
                    return View(db.Applications.Where(w=>w.IsApproved == null).ToList());
                case "approved":
                    return View(db.Applications.Where(w => w.IsApproved == true).ToList());
                case "rejected":
                    return View(db.Applications.Where(w => w.IsApproved == false).ToList());

                default:
                    return View(db.Applications.ToList());
            }
        }

        // GET: AdminPanel/Applications/Details/5
        public ActionResult Details(int? id)
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

            ApplicationViewModel viewModel = new ApplicationViewModel();
            var user = db.AspNetUsers.Find(applications.UserID);

            viewModel.ApplicationID = applications.ApplicationId;
            viewModel.Email = user.Email;
            viewModel.SocialClubName = user.SocialClubName;
            viewModel.RegisterDate = (DateTime)applications.RegisterDate;
            viewModel.IsApproved = applications.IsApproved;
            foreach (var item in applications.Answers)
            {
                viewModel.Answers.Add(new AnswerField {
                    AnswerSelection=(int)item.Answer,
                    AnswerText = item.AnswerText,
                    QuestionId = item.QuestionId,
                    QuestionText = item.Questions.QuestionText,
                    Selection_A = item.Questions.Selection_A,
                    Selection_B = item.Questions.Selection_B,
                    Selection_C = item.Questions.Selection_C,
                    IsTextArea = item.Questions.IsTextArea,
                });
            }
            return View(viewModel);
        }

        public ActionResult ApproveStatus(int id, bool? status)
        {
            var application = db.Applications.Find(id);
            if (application!=null)
            {
                string param = "";
                switch (application.IsApproved)
                {
                    case null:
                        param = "awaiting";
                        break;
                    case true:
                        param = "approved";
                        break;
                    case false:
                        param = "rejected";
                        break;
                }
                //if (status==true)
                //{
                //    var user = db.AspNetUsers.Find(User.Identity.GetUserId());
                //    TecoRP.Database.db_WhiteList.AddPlayer(user.SocialClubName,true);
                //}
                application.IsApproved = status;
                db.SaveChanges();
                return RedirectToAction("Index", new { filter = param });
            }
            return RedirectToAction("Index");
        }
        // GET: AdminPanel/Applications/Edit/5
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

        // POST: AdminPanel/Applications/Edit/5
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

        // GET: AdminPanel/Applications/Delete/5
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

        // POST: AdminPanel/Applications/Delete/5
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
