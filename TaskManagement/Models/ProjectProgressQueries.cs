using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Models {
    public class ProjectProgressQueries {

        //This is help model or "hybrid" model that
        //contains values from two different tables (Project and Tasks)
        //It is used as helping class to show Average Progress
        //for each Project

        public string ProjectCode { get; set; }
        public virtual Project Project { get; set; }

        public float Progress { get; set; }
        public virtual Tasks Tasks { get; set; }

    }
}