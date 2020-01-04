using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyCafe.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public UserNameViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier)).Value;
            var userFromDb = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            return View(userFromDb);
        }
    }
}
