using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        ApplicationDbContext context;

        public RoleController() {
            context = new ApplicationDbContext();
        }

        public ActionResult Index() {

            if (User.Identity.IsAuthenticated) {


                if (!isAdministratorUser()) {
                    return RedirectToAction("Index", "Home");
                }
            } else {
                return RedirectToAction("Index", "Home");
            }

            var pmRole = context.Roles.ToList().Where(r => r.Name == "Project Manager");
            var allRoles = context.Roles.ToList();
            return View(allRoles);

        }
        public bool isAdministratorUser() {
            if (User.Identity.IsAuthenticated) {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Administrator") {
                    return true;
                } else {
                    return false;
                }
            }
            return false;
        }

        public ActionResult Create() {
            if (User.Identity.IsAuthenticated) {


                if (!isAdministratorUser()) {
                    return RedirectToAction("Index", "Home");
                }
            } else {
                return RedirectToAction("Index", "Home");
            }

            var Role = new IdentityRole();
            return View(Role);
        }


        [HttpPost]
        public ActionResult Create(IdentityRole Role) {
            if (User.Identity.IsAuthenticated) {
                if (!isAdministratorUser()) {
                    return RedirectToAction("Index", "Home");
                }
            } else {
                return RedirectToAction("Index", "Home");
            }

            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}