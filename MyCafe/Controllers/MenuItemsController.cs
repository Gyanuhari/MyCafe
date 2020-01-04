using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;
using MyCafe.Models;
using MyCafe.Models.MenuItemViewModels;
using MyCafe.Utility;

namespace MyCafe.Controllers
{
    [Authorize(Roles=SD.AdminEndUser)]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemVM menuItemVM { get; set; }

        public MenuItemsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            menuItemVM = new MenuItemVM()
            {
                MenuItem = new Models.MenuItem(),
                CategoryList = _context.Categories.ToList(),

                //We did not initialize SubCategoryList because based on CategoryList we will retrive SubCategory in view.
            };
        }

        //Get: List of MenuItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync());
        }

        //Get: Create MenuItems
        public IActionResult Create()
        {
            return View(menuItemVM);
        }

        //Get SubCategories dropdown
        public JsonResult GetSubCategory(int categoryId)
        {
            List<SubCategory> subCategoryList = new List<SubCategory>();
            //subCategoryList = _context.SubCategories
            //    .Where(s => s.CategoryId == categoryId)
            //    .ToList();

            subCategoryList = (from subCategory in _context.SubCategories
                               where subCategory.CategoryId == categoryId
                               select subCategory).ToList();

            return Json(new SelectList(subCategoryList, "Id", "Name"));
        }

        //Post: MenuItems Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            menuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(menuItemVM);
            }
            _context.MenuItems.Add(menuItemVM.MenuItem);
            await _context.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _context.MenuItems.Find(menuItemVM.MenuItem.Id);
            if(files[0]!=null && files[0].Length>0)
            {
                //When User Uploads Images
                var uploads = Path.Combine(webRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, menuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                menuItemFromDb.Image = @"\images\" + menuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //When User does not uploads images
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + menuItemVM.MenuItem.Id + ".jpg");
                menuItemFromDb.Image = @"\images\" + menuItemVM.MenuItem.Id + "jpg";
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Get: MenuItem for Edit
        public async Task<IActionResult> Edit(int?id)
        {
            if(id==null)
            {
                return NotFound();
            }

            menuItemVM.MenuItem =await _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == id);
            menuItemVM.SubCategoryList = await _context.SubCategories.Where(s => s.CategoryId == menuItemVM.MenuItem.CategoryId).ToListAsync();

            return View(menuItemVM);
        }

        //Post: Edit Menu Items
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            menuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (id!=menuItemVM.MenuItem.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var menuItemFromDb = _context.MenuItems.Where(m => m.Id == menuItemVM.MenuItem.Id).FirstOrDefault();
                    if(files[0].Length>0 && files[0]!=null)
                    {
                        //If user upload new image
                        var uploads = Path.Combine(webRootPath, "images");
                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));
                        var extension_Old = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf("."), menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));

                        if (System.IO.File.Exists(Path.Combine(uploads,menuItemVM.MenuItem.Id + extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, menuItemVM.MenuItem.Id + extension_Old));
                        }

                        using (var filestream = new FileStream(Path.Combine(uploads, menuItemVM.MenuItem.Id + extension_New),FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        menuItemVM.MenuItem.Image = @"\images\" + menuItemVM.MenuItem.Id + extension_New;
                    }
                    if(menuItemVM.MenuItem.Image!=null)
                    {
                        menuItemFromDb.Image = menuItemVM.MenuItem.Image;
                    }
                    menuItemFromDb.Name = menuItemVM.MenuItem.Name;
                    menuItemFromDb.Description  = menuItemVM.MenuItem.Description ;
                    menuItemFromDb.Price = menuItemVM.MenuItem.Price ;
                    menuItemFromDb.Spicyness  = menuItemVM.MenuItem.Spicyness ;
                    menuItemFromDb.CategoryId = menuItemVM.MenuItem.CategoryId;
                    menuItemFromDb.SubCategoryId = menuItemVM.MenuItem.SubCategoryId;
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                return RedirectToAction(nameof(Index));
            }
            menuItemVM.SubCategoryList =await _context.SubCategories.Where(s => s.CategoryId == menuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(menuItemVM);
        }

        //Get: MenuItems for Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            menuItemVM.MenuItem =await _context.MenuItems.Include(m=>m.Category).Include(m=>m.SubCategory).FirstOrDefaultAsync(m=>m.Id==id);
            if(menuItemVM.MenuItem==null)
            {
                return NotFound();
            }
            return View(menuItemVM);
        }

        //Get: Delete MenuItems
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            menuItemVM.MenuItem = await _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == id);
            if (menuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(menuItemVM);
        }

        //Post: Delete
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if(id==null)
            {
                return BadRequest();
            }
            var menuItemFromDb = await _context.MenuItems.FindAsync(id);
            if(menuItemFromDb==null)
            {
                return BadRequest();
            }

            var webRootPath = _hostingEnvironment.WebRootPath;
            var uploads = Path.Combine(webRootPath, "images");
            var extension = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf("."), menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));
            var imagePath = Path.Combine(uploads, menuItemFromDb.Id + extension);
            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _context.MenuItems.Remove(menuItemFromDb);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}