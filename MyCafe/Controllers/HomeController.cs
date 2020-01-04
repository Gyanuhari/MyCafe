using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;
using MyCafe.Extensions;
using MyCafe.Models;
using MyCafe.Models.HomeViewModels;
using MyCafe.Utility;
using System.Security.Claims;

namespace MyCafe.Controllers
{
    public class HomeController : Controller
    {
        public readonly ApplicationDbContext _context;
        public HomeVM homeVm;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
            //Assigning the menuItems that the user had selected before but not had placed order
            var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
                var carMenuList = await _context.CartMenuItems.Where(c => c.ApplicationUserId == userId).ToListAsync();
                if (carMenuList.Count > 0)
                {
                    List<int> sessionList = new List<int>();
                    foreach (var item in carMenuList)
                    {
                        sessionList.Add(item.MenuItemId);
                    }
                    //Assigning the menuItems that the user had selected before but not had placed order
                    HttpContext.Session.SetObject(SD.SessionCart, sessionList);
                }
                else
                {
                    //Clears the session created by previous user
                    HttpContext.Session.Clear();
                }
            }

            homeVm = new HomeVM()
            {
                MenuItems = await _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Categories = await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync(),
                Coupons = await _context.Coupons.Where(c => c.IsActive == true).ToListAsync()
            };
            return View(homeVm);
        }

        //Get: Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var menuFromDb = await _context.MenuItems
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            if (menuFromDb == null)
            {
                return NotFound();
            }

            CartMenuItem cartMenuItem = new CartMenuItem()
            {
                MenuItem = menuFromDb,
                MenuItemId = menuFromDb.Id
            };

            return View(cartMenuItem);
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int menuId)
        {
            List<int> sessionList = new List<int>();
            var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SD.SessionCart)))
            {
                CartMenuItem cartMenuItem = new CartMenuItem()
                {
                    MenuItemId = _context.MenuItems.Where(m => m.Id == menuId).FirstOrDefault().Id,
                    ApplicationUserId = userId
                };
                await _context.CartMenuItems.AddAsync(cartMenuItem);
                _context.SaveChanges();

                sessionList.Add(menuId);
                HttpContext.Session.SetObject(SD.SessionCart, sessionList);
            }
            else
            {
                sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);
                if (!sessionList.Contains(menuId))
                {
                    CartMenuItem cartMenuItem = new CartMenuItem()
                    {
                        //MenuItemId = _context.MenuItems.Where(m => m.Id == menuId).FirstOrDefault().Id,
                        MenuItemId = menuId,
                        ApplicationUserId = userId,
                        MenuCount = 1
                    };

                    await _context.CartMenuItems.AddAsync(cartMenuItem);
                    _context.SaveChanges();
                    sessionList.Add(menuId);
                    HttpContext.Session.SetObject(SD.SessionCart, sessionList);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
