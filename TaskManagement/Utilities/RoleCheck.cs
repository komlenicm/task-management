using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Utilities {

    public class RoleCheck : Controller {

        //Intention of this class was to hold function for role checking of each user
        //Idea was to have functions that will be reusable in one class
        //and call it in each Controller separately
        //Solution was not implemented fully
        //therefore, I had to write "redundant" code and have each of these functions
        //in each controller because I had trouble with fixing bug.


        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsAdministratorUser(ApplicationDbContext db) {
            var user = User.Identity;
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var s = UserManager.GetRoles(user.GetUserId());
            if (s[0].ToString() == "Administrator") {
                    return true;
                } else {
                    return false;
                }
            
        }

        public bool IsProjectManagerUser(ApplicationDbContext db) {
            
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Project Manager") {
                    return true;
                } else {
                    return false;
                }
            
            
        }

        public bool IsDeveloperUser(ApplicationDbContext db) {
            
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Developer") {
                    return true;
                } else {
                    return false;
                }
       
        }
    }
}