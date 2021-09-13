using BugTracker.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Bug
    {
        public int Id { get; set; }

        public Severity Severity { get; set; }

        public Priority Priority { get; set; }

        public Status Status { get; set; }

        public BugType BugType { get; set; }

        public string Reporter { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public string Summary { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public Bug()
        {
            Created = DateTime.Now;
            Updated = null;
        }
    }
}
