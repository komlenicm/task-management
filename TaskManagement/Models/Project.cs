using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Models {
    public class Project {

        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual List<Tasks> Tasks { get; set; }

    }
}