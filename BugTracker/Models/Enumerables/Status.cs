using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Enumerables
{
    public enum Status
    {
        New = 1,
        Assigned = 2,
        Accepted = 3,
        Resolved = 4,
        Verified = 5,
        Incomplete = 6,
        Invalid = 7,
        Obsolete = 8,
        Infeasible = 9
    }
}
