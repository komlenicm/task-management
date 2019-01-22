using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Models {
    public class Tasks {

        public int Id { get; set; }
        public float Progress { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int StatusId { get; set; }
        public virtual Status Status { get; set; }

    }
}