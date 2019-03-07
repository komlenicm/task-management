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
using System.Diagnostics;
using TaskManagement.Models;
using TaskManagement.Utilities;

namespace TaskManagement.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        //private RoleCheck RoleCheck = new RoleCheck();

        // GET: Projects
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Index() {
            
            //Returning list of Projects and passing it to View
            var projects = db.Projects.ToList();

            return View(projects);
        }

        //[Authorize(Roles = "Project Manager")]
        //public ActionResult ListSuccesses() {

        //    var test = db.Tasks.Include(t => t.User).Average(t => t.Progress);
        //    ViewBag.AllTasks = test.ToString();
        //    var testQuery = db.Database.SqlQuery<List<Tuple<string, float>>>("SELECT p.ProjectCode, AVG(t.Progress) as AverageProgress FROM Projects p, Tasks t WHERE p.Id = t.ProjectId GROUP BY p.ProjectCode").ToList();
        //    var sampleList = new List<Tuple<string, float>>() {
        //    new Tuple<string, float> (testQuery[0].ToString(), testQuery[1].ToString()),
        //      new Tuple<string, float>("Ivan", )
        //    };

        //    var query1 = db.Database.SqlQuery<string>("SELECT p.ProjectCode FROM Projects p").ToList();
        //    var query2 = db.Database.SqlQuery<ProjectProgressQueries>("SELECT p.ProjectCode, AVG(CAST(t.Progress as float)) as AverageProgress FROM Projects p INNER JOIN Tasks t ON t.ProjectId = p.Id GROUP BY p.ProjectCode");
        //    var dict = new Dictionary<string, float>();
        //    for(int i = 0; i < query1.Count(); i++) {
        //        for(int j = 0; j < query2.Count(); j++)
        //        dict.Add(query1[i], query2[j]);
        //    }

        //    var query3 = db.Database.SqlQuery<IEnumerable<Tuple<string, float>>>("SELECT p.ProjectCode, AVG(t.Progress) as AverageProgress FROM Projects p, Tasks t WHERE p.Id = t.ProjectId GROUP BY p.ProjectCode").ToList();
        //    var query4 = db.Database.SqlQuery<ProjectProgressQueries>("SELECT AVG(t.Progress) as AverageProgress FROM Projects p, Tasks t WHERE p.Id = t.ProjectId GROUP BY p.ProjectCode").ToList();
        //    var query5 = db.Database.SqlQuery<List<ProjectProgressQueries>>("SELECT p.ProjectCode, AVG(CAST(t.Progress as float)) as AverageProgress FROM Projects p, Tasks t WHERE p.Id = t.ProjectId GROUP BY p.ProjectCode");
        //    var query6 = db.Database.SqlQuery<ProjectProgressQueries>("SELECT p.ProjectCode, AVG(CAST(t.Progress as float)) as AverageProgress FROM Projects p INNER JOIN Tasks t ON t.ProjectId = p.Id GROUP BY p.ProjectCode").First();
        //    var query7 = db.Database.SqlQuery<Project>("SELECT p.ProjectCode FROM Projects p").ToList();
        //    var query8 = db.Database.SqlQuery<Tasks>("SELECT AVG(t.Progress) as AverageProgress FROM Projects p, Tasks t WHERE p.Id = t.ProjectId GROUP BY p.ProjectCode").ToList();


        //   foreach (ProjectProgressQueries ppq in query6) {
        //     Debug.WriteLine(ppq.ProjectCode);
        //     Debug.WriteLine(ppq.Progress);
        //   }

        //    var avg = (from ppq in query6
        //    join p in db.Projects on ppq.ProjectCode equals p.ProjectCode
        //    select ppq.ProjectCode).ToList();

        //    List<Project> ListA = new List<Project>();
        //    List<Tasks> ListB = new List<Tasks>();

        //    var index = db.Tas;

        //    ListA.AddRange(db.Projects);
        //    ListB.AddRange(db.Tasks);

        //    ProjectProgressQueries ppq = new ProjectProgressQueries();
        //    ppq.ListOfProjects = ListA;
        //    ppq.ListOfTasks = ListB;

        //    return View();

        //}

        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult ListOfSuccessesWithId(int? id) {

            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific object with requested Id 
            Project project = db.Projects.Find(id);

            //Creating helping Model class in order to pass those values for Average Progress of Project
            ProjectProgressQueries ppq = new ProjectProgressQueries();

            //Query for returning AVG Progress based on Project Id
            var singleProjectAvg = db.Database.SqlQuery<double>("SELECT ISNULL(AVG(t.Progress), 0) as AverageProgress FROM Projects p, Tasks t WHERE p.Id = t.ProjectId AND p.Id = {0}", id).SingleOrDefault();
           
            //Assigning values to helping Model and passing model to View
            ppq.ProjectCode = project.ProjectCode;
            ppq.Progress = (float) singleProjectAvg;
            
            return View(ppq);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Details(int? Id) {
            if (Id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting project with specific Id
            Project project = db.Projects.Find(Id);

            if (project == null) {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create() {

            //Checking if logged User is Administrator,
            // if TRUE: pass list of Managers to Create View
            if (IsAdministratorUser()) {
                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "d71c3679-ada7-4a18-a188-17387c6e2688")), "Id", "UserName");
                
            } else if (IsProjectManagerUser()) {

                //Project Managers are able to assign projects to other managers as well
                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "d71c3679-ada7-4a18-a188-17387c6e2688")), "Id", "UserName");
                
            }

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Create([Bind(Include = "Id,ProjectCode,ProjectName,UserId")] Project project) {

            if (ModelState.IsValid) {

                Project currentProject = db.Projects.Where(x => x.ProjectCode == project.ProjectCode).SingleOrDefault();


                //Debug.WriteLine(currentProject.ProjectCode);

                if (currentProject == null) {
                    
                    //Getting values from text fields and adding them into DB
                    db.Projects.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                    
                } else {

                    Debug.WriteLine("Project Exists");


                }
                
            }

            //Checking if logged User is Administrator,
            // if TRUE: pass list of Managers to Create View
            // else check if User is Project Manager
            if (IsAdministratorUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "d71c3679-ada7-4a18-a188-17387c6e2688")), "Id", "UserName");
                return View(project);

            } else if (IsProjectManagerUser()) {

                //Project Managers are able to assign projects to other managers as well
                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "d71c3679-ada7-4a18-a188-17387c6e2688")), "Id", "UserName");
                return View(project);
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);



            if (project == null) {
                return HttpNotFound();
            }

            //Checking if logged User is Administrator
            // if TRUE: Pass list of Managers to Edit View (Administrators must select Project Managers to lead the project)
            if (IsAdministratorUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "d71c3679-ada7-4a18-a188-17387c6e2688")), "Id", "UserName", project.UserId);
                return View(project);
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "Id,ProjectCode,ProjectName,UserId")] Project project) {

            if (ModelState.IsValid) {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Checking if logged User is Administrator
            // if TRUE: Pass list of Managers to Edit View (Administrators must select Project Managers to lead the project)
            if (IsAdministratorUser()) {

                ViewBag.UserId = new SelectList(db.Users.Where(o => o.Roles.Any(r => r.RoleId == "d71c3679-ada7-4a18-a188-17387c6e2688")), "Id", "UserName");
                return View(project);
            }


            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Selecting specific project from DB
            Project project = db.Projects.Find(id);

            if (project == null) {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id) {
            
            //Selecting specific project from DB and deleting it from DB
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
               
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Function to check if logged User is Administrator
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

        //Function to check if logged User is Project Manager
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
    }
}