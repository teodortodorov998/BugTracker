using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enumerables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace BugTracker.Controllers
{
    public class BugsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public BugsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Bugs
        [Authorize(Roles = "Admin, Manager, QA Engineer")]
        public async Task<IActionResult> Index()
        {
            var users = userManager.GetUsersInRoleAsync("Developer").Result.ToList();
            ViewData["ApplicationUserId"] = new SelectList(users, "Id", "Email");
            var applicationDbContext = _context.Bug.Include(b => b.Project).Include(b => b.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bugs/Details/5
        [Authorize(Roles = "Admin, Manager, QA Engineer, Developer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bug
                .Include(b => b.Project)
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bug == null)
            {
                return NotFound();
            }

            return View(bug);
        }

        // GET: Bugs/Create
        [Authorize(Roles = "Admin, QA Engineer")]
        public IActionResult Create()
        {
            var users = userManager.GetUsersInRoleAsync("Developer").Result.ToList();
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name");
            ViewData["ApplicationUserId"] = new SelectList(users, "Id", "Email");
            return View();
        }

        // POST: Bugs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, QA Engineer")]
        public async Task<IActionResult> Create([Bind("Id,Severity,Priority,Status,BugType,Reporter,Created,Updated,Summary,ProjectId,ApplicationUserId")] Bug bug)
        {
            if (ModelState.IsValid)
            {
                bug.Reporter = User.Identity.Name;
                bug.Created = DateTime.Now;
                _context.Add(bug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var users = userManager.GetUsersInRoleAsync("Developer").Result.ToList();
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", bug.ProjectId);
            ViewData["ApplicationUserId"] = new SelectList(users, "Id", "Id", bug.ApplicationUserId);
            return View(bug);
        }

        // GET: Bugs/Edit/5
        [Authorize(Roles = "Admin, QA Engineer, Developer, Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bug.FindAsync(id);
            if (bug == null)
            {
                return NotFound();
            }
            var users = userManager.GetUsersInRoleAsync("Developer").Result.ToList();
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", bug.ProjectId);
            ViewData["ApplicationUserId"] = new SelectList(users, "Id", "Email", bug.ApplicationUserId);
            return View(bug);
        }

        // POST: Bugs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, QA Engineer, Developer, Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Severity,Priority,Status,BugType,Reporter,Created,Updated,Summary,ProjectId,ApplicationUserId")] Bug bug)
        {
            if (id != bug.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bug.Updated = DateTime.Now;
                    _context.Update(bug);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BugExists(bug.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = bug.Id });
                
            }
            var users = userManager.GetUsersInRoleAsync("Developer").Result.ToList();
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", bug.ProjectId);
            ViewData["ApplicationUserId"] = new SelectList(users, "Id", "Id", bug.ApplicationUserId);
            return View(bug);
        }

        // GET: Bugs/Delete/5
        [Authorize(Roles = "Admin, QA Engineer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bug
                .Include(b => b.Project)
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bug == null)
            {
                return NotFound();
            }

            return View(bug);
        }

        // POST: Bugs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, QA Engineer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bug = await _context.Bug.FindAsync(id);
            _context.Bug.Remove(bug);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BugExists(int id)
        {
            return _context.Bug.Any(e => e.Id == id);
        }

        public async Task<IActionResult> BugsOfProject(int id)
        {
            var applicationDbContext = _context.Bug
                .Where(b => b.ProjectId == id)
                .Include(b => b.Project)
                .Include(b => b.ApplicationUser);

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Developer, Admin")]
        public async Task<IActionResult> ShowAssignedBugs(string id)
        {
            var applicationDbContext = _context.Bug
                .Where(b => b.ApplicationUserId == id)
                .Include(b => b.Project)
                .Include(b => b.ApplicationUser);

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin, Manager, QA Engineer")]
        public async Task<IActionResult> ShowResolved()
        {
            var applicationDbContext = _context.Bug
                .Where(b => b.Status == Models.Enumerables.Status.Resolved)
                .Include(b => b.Project).Include(b => b.ApplicationUser);

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin, Manager, QA Engineer, Developer")]
        public async Task<IActionResult> ShowSearchResultsIndex(string SearchPhrase, string SearchOption)
        {

            var applicationDbContext = _context.Bug.Include(b => b.Project).Include(u => u.ApplicationUser);
            if (SearchOption == "Severity")
            {
                Severity option = (Severity)Enum.Parse(typeof(Severity), SearchPhrase, true);
                return View("Index", await applicationDbContext.Where(b => b.Severity == option).ToListAsync());
            }
            if (SearchOption == "Priority")
            {
                Priority option = (Priority)Enum.Parse(typeof(Priority), SearchPhrase, true);
                return View("Index", await applicationDbContext.Where(b => b.Priority == option).ToListAsync());
            }
            if (SearchOption == "Status")
            {
                Status option = (Status)Enum.Parse(typeof(Status), SearchPhrase, true);
                return View("Index", await applicationDbContext.Where(b => b.Status == option).ToListAsync());
            }
            if (SearchOption == "BugType")
            {
                BugType option = (BugType)Enum.Parse(typeof(BugType), SearchPhrase, true);
                return View("Index", await applicationDbContext.Where(b => b.BugType == option).ToListAsync());
            }
            return View();
        }

        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ShowBugsOfDev(string email)
        {
            string id = "";
            foreach (var item in _context.Users)
            {
                if (email == item.Email)
                {
                    id = item.Id;
                }
            }
            var assignedBugs = _context.Bug.Include(b => b.Project).Include(u => u.ApplicationUser);
            return View("Index", await assignedBugs.Where(b => b.ApplicationUserId == id).ToListAsync());
        }

    }
}
