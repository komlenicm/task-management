using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
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
        public ActionResult AssignRole()
        {
            var listUsers = db.Users.Select(u => new SelectListItem { Value = u.UserName.ToString(), Text = u.UserName }).ToList();
            var listRoles = db.Roles.Select(r => new SelectListItem { Value = r.Name.ToString(), Text = r.Name }).ToList();

            ViewBag.Users = listUsers;
            ViewBag.Roles = listRoles;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult AssignRole(string UserName, string RoleName)
        {

            ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
           
            
            var currentRoleOfUser = userManager.GetRoles(user.Id).Single();

            if (currentRoleOfUser == RoleName) {

                System.Diagnostics.Debug.WriteLine("User already have that role.");

            } else {

                userManager.RemoveFromRole(user.Id, currentRoleOfUser);
                userManager.AddToRole(user.Id, RoleName);

            }
            

            // Passing list of users and roles to DropDown
            var listUsers = db.Users.Select(u => new SelectListItem { Value = u.UserName.ToString(), Text = u.UserName }).ToList();
            var listRoles = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.Users = listUsers;
            ViewBag.Roles = listRoles;
            
            return View();
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

            return View(user);
            
        }


    }
}