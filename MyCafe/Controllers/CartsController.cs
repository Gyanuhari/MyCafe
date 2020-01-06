using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;
using MyCafe.Extensions;
using MyCafe.Models;
using MyCafe.Models.CartViewModels;
using MyCafe.Services;
using MyCafe.Utility;

namespace MyCafe.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public CartViewModel CartVM { get; set; }

        public CartsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
            CartVM = new CartViewModel()
            {
                CartMenuList = new List<CartMenuItem>(),
                OrderHeader = new OrderHeader(),
                CouponsList = _context.Coupons.Where(c => c.IsActive == true).ToList()
            };

            //Whenever user logins he/she can she previous cart
            //var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;

            //var userId = _userManager.GetUserId(User);
            //var sessionList = _context.CartMenuItems.Where(c => c.ApplicationUserId == userId).Select(c => c.MenuItemId).ToListAsync();
            //if (sessionList != null)
            //    HttpContext.Session.SetObject(SD.SessionCart, sessionList);
        }

        public async Task<IActionResult> Index()
        {
            if ((HttpContext.Session.GetObject<List<int>>(SD.SessionCart)) != null)
            {
                List<int> sessionList = new List<int>();
                var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
                sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);

                foreach (var sessionId in sessionList)
                {
                    var cartMenuItemFromDb = await _context.CartMenuItems.Where(c => c.MenuItemId == sessionId && c.ApplicationUserId == userId).FirstOrDefaultAsync();
                    if (cartMenuItemFromDb != null)
                    {
                        CartMenuItem cartMenuItem = new CartMenuItem()
                        {
                            MenuItem = _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == cartMenuItemFromDb.MenuItemId).FirstOrDefault(),
                            MenuItemId = cartMenuItemFromDb.MenuItemId,
                            MenuCount = cartMenuItemFromDb.MenuCount
                        };
                        CartVM.CartMenuList.Add(cartMenuItem);
                    }
                }
            }
            return View(CartVM);
        }

        //Increase MenuItem by 1
        public async Task<IActionResult> IncreaseItem(int menuId)
        {
            if ((HttpContext.Session.GetObject<List<int>>(SD.SessionCart)) != null)
            {
                var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
                CartMenuItem cartMenuItemFromDb = await _context.CartMenuItems.Where(c => c.MenuItemId == menuId && c.ApplicationUserId == userId).FirstOrDefaultAsync();
                cartMenuItemFromDb.MenuCount = cartMenuItemFromDb.MenuCount + 1;
                _context.Update(cartMenuItemFromDb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //Decrease MenuItem by 1
        public async Task<IActionResult> DecreaseItem(int menuId)
        {
            if ((HttpContext.Session.GetObject<List<int>>(SD.SessionCart)) != null)
            {
                var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
                CartMenuItem cartMenuItemFromDb = await _context.CartMenuItems.Where(c => c.MenuItemId == menuId && c.ApplicationUserId == userId).FirstOrDefaultAsync();
                //Decrease only if the menu item is greater than 1
                if (cartMenuItemFromDb.MenuCount > 1)
                {
                    cartMenuItemFromDb.MenuCount = cartMenuItemFromDb.MenuCount - 1;
                    _context.Update(cartMenuItemFromDb);
                    await _context.SaveChangesAsync();
                }              
            }
            return RedirectToAction(nameof(Index));
        }

        //Remove the Cart Item
        public async Task<IActionResult> RemoveItem(int menuId)
        {
            var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
            CartMenuItem cartMenuItemFromDb = await _context.CartMenuItems.Where(c => c.MenuItemId == menuId && c.ApplicationUserId == userId).FirstOrDefaultAsync();
            _context.CartMenuItems.Remove(cartMenuItemFromDb);
            await _context.SaveChangesAsync();

            List<int> sessionList = new List<int>();
            sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);
            sessionList.Remove(menuId);
            HttpContext.Session.SetObject(SD.SessionCart, sessionList);

            return RedirectToAction(nameof(Index));
        }

        //Get Summary of the CartItems for placing the order
        public async Task<IActionResult> Summary()
        {
            if ((HttpContext.Session.GetObject<List<int>>(SD.SessionCart)) != null)
            {
                var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
                List<int> sessionList = new List<int>();
                sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);

                foreach (var sessionId in sessionList)
                {
                    var cartMenuItemFromDb = await _context.CartMenuItems.Where(c => c.MenuItemId == sessionId && c.ApplicationUserId == userId).FirstOrDefaultAsync();
                    CartMenuItem cartMenuItem = new CartMenuItem()
                    {
                        MenuItem = _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == cartMenuItemFromDb.MenuItemId).FirstOrDefault(),
                        MenuItemId = cartMenuItemFromDb.MenuItemId,
                        MenuCount = cartMenuItemFromDb.MenuCount
                    };
                    CartVM.CartMenuList.Add(cartMenuItem);
                }
            }
            return View(CartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            if (ModelState.IsValid)
            {
                var userId = ((ClaimsIdentity)this.User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value;
                CartVM.OrderHeader.UserId = userId;
                CartVM.OrderHeader.OrderDate = DateTime.Now;
                CartVM.OrderHeader.Status = SD.StatusSubmitted;
                if (CartVM.OrderHeader.CouponCode == null)
                    CartVM.OrderHeader.CouponCode = "No Coupon";

                OrderHeader orderHeader = CartVM.OrderHeader;
                _context.OrderHeaders.Add(orderHeader);
                await _context.SaveChangesAsync();  //We need Id below so we are saving here if not we can directly save it below

                CartVM.CartMenuList =await _context.CartMenuItems.Where(c => c.ApplicationUserId == userId).ToListAsync();
                foreach (var item in CartVM.CartMenuList)
                {
                    item.MenuItem = await _context.MenuItems.Where(m => m.Id == item.MenuItemId).FirstOrDefaultAsync();
                    OrderDetails orderDetails = new OrderDetails()
                    {
                        OrderHeaderId = orderHeader.Id,
                        MenuItemId = item.MenuItemId,
                        MenuName = item.MenuItem.Name,
                        MenuCount = item.MenuCount,
                        Price = item.MenuItem.Price

                    };
                    _context.OrderDetails.Add(orderDetails);
                }

                _context.CartMenuItems.RemoveRange(CartVM.CartMenuList);
                await _context.SaveChangesAsync();
                HttpContext.Session.Clear();

                return RedirectToAction("OrderConfirmation", "Carts", new { orderId=orderHeader.Id});
            }
            else
            {
                return RedirectToAction(nameof(Summary));
            }
        }

        //For Display Message With OrderId After Order Placement
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var userEmail = (await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync()).Email;
            await _emailSender.SendStatusEmailAsync(userEmail, SD.StatusSubmitted, orderId.ToString());

            return View(orderId);
        }
    }
}