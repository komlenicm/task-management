using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Models {
    public class UserTaskQueries {

        //This is help model or "hybrid" model that
        //contains values from two different tables (ApplicationUser and Tasks)
        //Solution is not developed in project but
        //Idea was to have model with two columns
        //that will hold information about user and number of tasks assigned to him

        public string UserId { get; set; }
        public virtual ApplicationUser Users { get; set; }

        public int NumberOfTasks { get; set; }
        public virtual Tasks Tasks { get; set; }

    }
}