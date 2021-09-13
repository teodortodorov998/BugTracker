using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Manager { get; set; }

        public DateTime Deadline { get; set; }

        public virtual ICollection<Bug> Bugs { get; set; }
        public Project()
        {

        }
    }
}
