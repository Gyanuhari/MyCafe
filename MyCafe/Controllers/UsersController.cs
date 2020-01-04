using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCafe.Data;
using MyCafe.Models;
using MyCafe.Utility;

namespace MyCafe.Controllers
{
    [Authorize(Roles=SD.AdminEndUser)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            return View( _context.Users.Where(u=>u.Id!=claim.Value).ToList());
        }

        //Get: Edit User
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFromDb = await _context.Users.FindAsync(id);
            if(userFromDb==null)
            {
                return NotFound();
            }
            return View(userFromDb);
        }

        //Post: Edit User
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(ApplicationUser model, string id)
        {
            if(id!=model.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userFromDb = _context.Users.Where(u => u.Id == model.Id).FirstOrDefault();
            userFromDb.LockoutEnd = model.LockoutEnd;
            userFromDb.LockoutReason = model.LockoutReason;
            userFromDb.AccessFailedCount = model.AccessFailedCount;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LockUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFromDb = await _context.Users.FindAsync(id);
            if (userFromDb == null)
            {
                return BadRequest();
            }
            //Add 100 years to todays date to lock
            userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLockUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userFromDb = await _context.Users.FindAsync(id);
            if (userFromDb == null)
            {
                return BadRequest();
            }
            //Subtract 1 day from todays date to unlock
            userFromDb.LockoutEnd = DateTime.Now.AddDays(-1);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}