using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        // GET: Users
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Administrator")]
        public ActionResult Index() {

            //Selecting all users from DB and passing it to View
            var users = db.Users.ToList();
            
            return View(users);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Details(string Id) {

            if (Id.Equals("")) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific user from DB
            //Initializing UserManager in order to get Role for specific user
            //Passing role name to ViewBag.Rola 
            var user = db.Users.Find(Id);
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var s = UserManager.GetRoles(user.Id);
            ViewBag.Rola = s[0].ToString();


            if (user.Equals(null)) {
                return HttpNotFound();
            }

            return View(user);
        }

        //Delete function is similar to Details
        //Passing same arguments to View
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string Id) {

            if (Id.Equals("")) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific user from DB
            //Initializing UserManager in order to get Role for specific user
            //Passing role name to ViewBag.Rola
            
            var user = db.Users.Find(Id);
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var s = UserManager.GetRoles(user.Id);
            ViewBag.Rola = s[0].ToString();

            if (user.Equals(null)) {
                return HttpNotFound();
            }
            return View(user);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(string Id) {

            //Selecting specific User from DB
            var user = db.Users.Find(Id);
            
            //Removing user from DB
            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string Id) {
            if (Id.Equals("")) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific user from DB
            var user = db.Users.Find(Id);

            if (user.Equals(null)) {
                return HttpNotFound();
            }


            //Passing roles list to View page
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name");

            return View(user);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "Id, Email, UserName, FirstName, SurName")] ApplicationUser user) {

            if (ModelState.IsValid) {

                //If model is valid, edit user
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            
            //Passing roles list to View page
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name");

            return View(user);
            
        }

    }
}