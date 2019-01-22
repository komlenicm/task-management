using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        public ActionResult Index() {

            //Declaring two list of type IQueryable<Tasks>
            //that will hold list of different tasks
            //tasksAll holds list of all tasks that is
            //passed to View if Administrator is logged in;
            //taskManAndDev is list that holds tasks for Managers and Developers
            //Tasks can be seen if its assigned to specific user (Project Managers or Developers)
            //or not assigned to anyone
            IQueryable<Tasks> tasksAll;
            IQueryable<Tasks> taskManAndDev;
            
            
            //Checking is logged user is Administrator
            if (IsAdministratorUser()) {
                tasksAll = db.Tasks.Include(t => t.Project).Include(t => t.Status).Include(t => t.User);
                return View(tasksAll.ToList());

            //Check if logged user is Project Manager or Developer
            //They share same list that is filtered by two conditions:
            //1. Task must have no assignee
            //2. Task must be assigned to him
            } else if (IsProjectManagerUser() || IsDeveloperUser()) {
                
                var userId = User.Identity.GetUserId();
                taskManAndDev = db.Tasks.Include(t => t.Project).Include(t => t.Status).Include(t => t.User).Where(t => (t.UserId == userId) || (t.UserId == null));
                return View(taskManAndDev.ToList());

            } 
            
            return View();
        }


        [Authorize(Roles = "Administrator")]
        public ActionResult AssignTask() {

            //List of all tasks and users is sent to View
            ViewBag.Id = new SelectList(db.Tasks, "Id", "Description");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult AssignTask([Bind(Include = "Id, UserId")] Tasks tasks) {

            ////Query to show developers task number: "SELECT u.UserName, COUNT(t.Id) as NumOfTasks FROM Tasks t, AspNetUsers u WHERE u.Id = t.UserId GROUP BY u.UserName"
            //UserTaskQueries utq = new UserTaskQueries();
            
            ////Getting userId from task
            //string uId = tasks.UserId;

            ////Query to calculate number of task that user have
            //var numOfTask = db.Database.SqlQuery<int>("SELECT COUNT(t.Id) as NumOfTasks FROM Tasks t, AspNetUsers u WHERE u.Id = t.UserId AND u.Id = {0}", uId).SingleOrDefault();
            //if (numOfTask > 3) {
            //    Console.WriteLine("Cannot assign more than 3 task to developer!");
            //    return RedirectToAction("Index");

            //} else {

            //    if (ModelState.IsValid) {
            //        if (IsAdministratorUser()) {

            //            db.Tasks.Attach(tasks);
            //            var task = db.Entry(tasks);

            //            db.Entry(tasks).Property("Id").IsModified = true;
            //            db.Entry(tasks).Property("UserId").IsModified = true;

            //            db.SaveChanges();
            //            return RedirectToAction("Index");

            //        }

            //    }
            //}

            //Checking if model is valid and
            //setting specific columns to IsModified state
            //and saving changes to DB
            if (ModelState.IsValid) {
                
                    db.Tasks.Attach(tasks);
                    var task = db.Entry(tasks);

                    db.Entry(tasks).Property("Id").IsModified = true;
                    db.Entry(tasks).Property("UserId").IsModified = true;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                
            }

            return View(tasks);
        }

        // GET: Tasks/Details/5
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific Tasks from DB
            Tasks tasks = db.Tasks.Find(id);

            if (tasks == null) {
                return HttpNotFound();
            }
            return View(tasks);
        }

        // GET: Tasks/Create
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create() {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectCode");
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name");
            
            //If Administrator is logged in, he can assign task to anyone including Project Managers and Developers
            //else if Project Manager is logged in, he can assign task only to Developers
            if (IsAdministratorUser()) {

                ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
                return View();
            } else if (IsProjectManagerUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "b8fd5816-9b2a-4fd1-8d70-16691608e8bf")), "Id", "UserName");
                return View();
            }

            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create([Bind(Include = "Id,Progress,Deadline,Description,ProjectId,UserId,StatusId")] Tasks tasks) {

            if (ModelState.IsValid) {
                db.Tasks.Add(tasks);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Assigning list of Projects and Statuses from DB to a ViewBag
            //that will be used in dropdown menu
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectCode", tasks.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name", tasks.StatusId);

            //If Administrator is logged in, he can assign task to anyone including Project Managers and Developers
            //else if Project Manager is logged in, he can assign task only to Developers
            if (IsAdministratorUser()) {

                ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
                return View(tasks);
            } else if (IsProjectManagerUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "b8fd5816-9b2a-4fd1-8d70-16691608e8bf")), "Id", "UserName");
                return View(tasks);
            }

            return View(tasks);
        }

        // GET: Tasks/Edit/5
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific Task from DB
            Tasks tasks = db.Tasks.Find(id);

            if (tasks == null) {
                return HttpNotFound();
            }

            //Assigning list of Projects and Statuses from DB to a ViewBag
            //that will be used in dropdown menu
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectCode", tasks.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name", tasks.StatusId);

            
            if (IsAdministratorUser()) {

                ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", tasks.UserId);
                return View(tasks);

            } else if (IsProjectManagerUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "b8fd5816-9b2a-4fd1-8d70-16691608e8bf")), "Id", "UserName");
                return View(tasks);

            } else if (IsDeveloperUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "b8fd5816-9b2a-4fd1-8d70-16691608e8bf")), "Id", "UserName");
                return View(tasks);
            }
            

            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        public ActionResult Edit([Bind(Include = "Id,Progress,Deadline,Description,ProjectId,UserId,StatusId")] Tasks tasks) {

            if (ModelState.IsValid) {

                //There are different scenarios for Edit function of each role
                //If administrator uses edit function, he can edit every column
                //else if Project Manager is editing task, he can edit 
                //
                if (IsAdministratorUser()) {

                    db.Entry(tasks).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                } else if (IsProjectManagerUser()) {

                    //Connecting object to specific type
                    db.Tasks.Attach(tasks);
                    
                    //Allowing operation on entity
                    db.Entry(tasks);

                    //Setting column states to modifiable so Manager can edit them
                    db.Entry(tasks).Property("UserId").IsModified = true;
                    db.Entry(tasks).Property("StatusId").IsModified = true;
                    db.Entry(tasks).Property("Progress").IsModified = true;
                    db.Entry(tasks).Property("Deadline").IsModified = true;
                    db.Entry(tasks).Property("Description").IsModified = true;

                    db.SaveChanges();
                    return RedirectToAction("Index");

                } else if (IsDeveloperUser()) {

                    //Connecting object to specific type
                    db.Tasks.Attach(tasks);

                    //Allowing operation on entity
                    db.Entry(tasks);

                    //Setting column states to modifiable so Developer can edit them
                    db.Entry(tasks).Property("StatusId").IsModified = true;
                    db.Entry(tasks).Property("Progress").IsModified = true;
                    db.Entry(tasks).Property("Description").IsModified = true;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


            }

            //Assigning list of Projects and Statuses from DB to a ViewBag
            //that will be used in dropdown menu
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectCode", tasks.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name", tasks.StatusId);
            
            //If Administrator is logged in, he can choose any User from dropdown and assign task to him
            //else if Project Manager is logged in, he can assign task only to Developers afterwards 
            if (IsAdministratorUser()) {

                ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", tasks.UserId);
                return View(tasks);
            } else if (IsProjectManagerUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "b8fd5816-9b2a-4fd1-8d70-16691608e8bf")), "Id", "UserName");
                return View(tasks);
            }

            return View(tasks);
        }

        // GET: Tasks/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific task from DB
            Tasks tasks = db.Tasks.Find(id);

            if (tasks == null) {
                return HttpNotFound();
            }
            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id) {
            //Selecting specific task and removing it from DB
            Tasks tasks = db.Tasks.Find(id);
            db.Tasks.Remove(tasks);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Checking if user is Administrator
        public bool IsAdministratorUser() {
            if (User.Identity.IsAuthenticated) {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Administrator") {

                    return true;

                } else {

                    return false;
                }
            }
            return false;
        }

        //Checking if user is Project Manager
        public bool IsProjectManagerUser() {
            if (User.Identity.IsAuthenticated) {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Project Manager") {
                    return true;
                } else {
                    return false;
                }
            }
            return false;
        }

        //Checking if user is Developer
        public bool IsDeveloperUser() {
            if (User.Identity.IsAuthenticated) {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Developer") {
                    return true;
                } else {
                    return false;
                }
            }
            return false;
        }
    }
}