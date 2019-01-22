using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement;
using TaskManagement.Models;
using TaskManagement.Controllers;
using System.Web.Mvc;

namespace TaskManagement.Tests.Controllers {

    [TestClass]
    public class TasksControllerTest {

        public ApplicationDbContext db = new ApplicationDbContext();
        
        
        [TestMethod]
        public void Index() {

            //Creating instance of TasksController
            var tasksController = new TasksController();

            //Calling Index() function from TasksController and casted it to ViewResult (returns View())
            var result = tasksController.Index() as ViewResult;

            //Creating list of projects and assigning our result model type to list
            List<Tasks> myTasks = (List<Tasks>) result.Model;

            //Checking if our list object is not-null.
            Assert.IsNotNull(myTasks);
        }

        [TestMethod]
        public void Details() {

            //Selecting Tasks from DB with Id = 1
            var task = db.Tasks.Find(1);

            //Creating instance of TasksController
            var tasksController = new TasksController();

            //Calling Details() function from TasksController and casted it to ViewResult
            ViewResult result = tasksController.Details(1) as ViewResult;

            //Casting View model to actual object type in order to access its attributes for comparison
            var pr = (Tasks) result.ViewData.Model;

            //Checking if instance type from View is equal to type Tasks
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(Tasks));

            //Checking if task Id we got from our View is equal to our selected Task from DB
            Assert.AreEqual(task.Id, pr.Id);

        }

        [TestMethod]
        public void Create() {

            //Creating new object of type Tasks
            Tasks task = new Tasks();

            //Assigning values to our object
            DateTime dt = DateTime.Today;
            task.ProjectId = 1;
            task.StatusId = 1;
            task.UserId = "8ed2b3a7-7c4a-4129-a503-bc5c87a1d5fd";
            task.Progress = 0;
            task.Deadline = dt;
            task.Description = "Test description for new task";

            //Creating instance of TasksController
            var tasksController = new TasksController();

            //Creating object of type RedirectToRouteResult because after calling post method CREATE, it returns RedirectToAction
            RedirectToRouteResult result = tasksController.Create(task) as RedirectToRouteResult;

            //Checking if Create method redirects to Index() page after execution
            Assert.AreEqual("Index", result.RouteValues["Action"]);

            //Checking if result value is not null
            Assert.IsNotNull(result.ToString());

        }

    }
}
