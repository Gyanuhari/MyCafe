using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;
using MyCafe.Models;
using MyCafe.Utility;

namespace MyCafe.Controllers
{
    [Authorize(Roles=SD.AdminEndUser)]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.ToListAsync());
        }

        //Get: Create Coupons
        public IActionResult Create()
        {
            return View();
        }

        //Post: Create Coupons
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Coupons coupons)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files[0] != null && files[0].Length > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                    _context.Coupons.Add(coupons);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(coupons);
        }

        //Get: Edit Coupons
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var couponsFromDb = await _context.Coupons.FindAsync(id);
            if (couponsFromDb == null)
            {
                return NotFound();
            }
            return View(couponsFromDb);
        }

        //Post: Edit Coupons
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, Coupons coupons)
        {
            if (id != coupons.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var couponsFromDb = await _context.Coupons.FindAsync(coupons.Id);
                var files = HttpContext.Request.Form.Files;
                if (files[0] != null && files[0].Length > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponsFromDb.Picture = p1;
                }
                couponsFromDb.Name = coupons.Name;
                couponsFromDb.CouponType = coupons.CouponType;
                couponsFromDb.Discount = coupons.Discount;
                couponsFromDb.MinimumAmount = coupons.MinimumAmount;
                couponsFromDb.IsActive = coupons.IsActive;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(coupons);
            }
        }

        //Get: Delete Coupons
        public async Task<IActionResult> Delete(int? id)
        {
            if(id==null)
            {
                return BadRequest();
            }
            var couponFromDb = await _context.Coupons.FindAsync(id);
            if(couponFromDb==null)
            {
                return BadRequest();
            }

            return View(couponFromDb);
        }

        //Post: Delete Coupons
        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if(id==null)
            {
                return BadRequest();
            }
            var couponFromDb = await _context.Coupons.FindAsync(id);
            if(couponFromDb==null)
            {
                return BadRequest();
            }

             _context.Coupons.Remove(couponFromDb);
             await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}