using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Models.CategoryAndSubCategoryViewModels;
using MyCafe.Data;
using MyCafe.Utility;
using Microsoft.AspNetCore.Authorization;
using MyCafe.Models;

namespace MyCafe.Controllers
{
    [Authorize(Roles=SD.AdminEndUser)]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        [TempData]
        public string StatusMessages { get; set; }

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.SubCategories.Include(s => s.Category).ToListAsync());
        }

        //Get Action for Create
        public async Task<IActionResult> Create()
        {
            CategoryAndSubCategoryVM categoryAndSubCategoryVM = new CategoryAndSubCategoryVM()
            {
                SubCategory = new Models.SubCategory(),
                CategoryList = await _context.Categories.ToListAsync(),
                SubCategoryList = await _context.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync()
            };
            return View(categoryAndSubCategoryVM);
        }

        //Post Action for Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost(CategoryAndSubCategoryVM categorySubCategoryVM)
        {
            if(ModelState.IsValid)
            {
                int doesSubCategoryExists = _context.SubCategories.Where(s => s.Id == categorySubCategoryVM.SubCategory.Id).Count();
                int doesCombinCategoryAndSubCategoryExists = _context.SubCategories
                    .Where(s => s.Name == categorySubCategoryVM.SubCategory.Name && s.CategoryId == categorySubCategoryVM.SubCategory.CategoryId)
                    .Count();

                if (doesSubCategoryExists>0 && categorySubCategoryVM.IsNew)
                {
                    //Error SubCategory already exists
                    StatusMessages = "Error: This Sub Category Already Exists!";
                }
                else
                {
                    if(doesSubCategoryExists==0 && !categorySubCategoryVM.IsNew)
                    {
                        //Error SubCategory does not exists
                        StatusMessages = "Error: This Sub Category Does Not Exists. It Should Be New Sub Category!";
                    }
                    else
                    {
                        if(doesCombinCategoryAndSubCategoryExists>0)
                        {
                            //Error Combination of Category and SubCategory Exists
                            StatusMessages = "Error: This Combination of Category And Sub Category Already Exists!";
                        }
                        else
                        {
                            _context.Add(categorySubCategoryVM.SubCategory);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
                CategoryAndSubCategoryVM newCategoryAndSubCategoryVM = new CategoryAndSubCategoryVM()
                {
                    SubCategory = categorySubCategoryVM.SubCategory,
                    CategoryList = await _context.Categories.ToListAsync(),
                    SubCategoryList = await _context.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync(),
                    StatusMessage= this.StatusMessages
                };

            return View(newCategoryAndSubCategoryVM);
        }

        [HttpGet]
        //Get SubCategory/id
        public IActionResult GetSubCategoryById(int? subCategoryId)
        {
            if(subCategoryId!=null)
            {
                CategoryAndSubCategoryVM categoryListAndSubCategory = new CategoryAndSubCategoryVM()
                {
                    CategoryList = _context.Categories.ToList(),
                    SubCategory = _context.SubCategories.Where(s => s.Id == subCategoryId).FirstOrDefault(),
                    SubCategoryList = new List<string>(),
                };
                return Json(categoryListAndSubCategory);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult SaveEditSubCategory([FromBody] CategoryAndSubCategoryVM subCategoryJson)
        {
            if (ModelState.IsValid)
            {
                var subCategoryFromDb = _context.SubCategories.Include(s => s.Category)
                    .Where(s => s.Id == subCategoryJson.SubCategory.Id).FirstOrDefault();
                if (subCategoryFromDb != null)
                {
                    subCategoryFromDb.Name = subCategoryJson.SubCategory.Name;
                    subCategoryFromDb.CategoryId = subCategoryJson.SubCategory.CategoryId;
                    _context.SaveChanges();

                    return Json(subCategoryJson);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }
        
        [HttpGet]
        //Get Details/id
        public IActionResult GetDetailSubCategory(int? subCategoryId)
        {
            if (subCategoryId != null)
            {
                EditSubCategoryVM editSubCategoryVM = new EditSubCategoryVM()
                {
                    SubCategory = _context.SubCategories.Include(s => s.Category).Where(s => s.Id == subCategoryId).FirstOrDefault(),
                };
                return Json(editSubCategoryVM);
            }
            else
            {
                return BadRequest();
            }
        }

        //Delete SubCateory/id
        public async Task<IActionResult> DeleteSubCategory(int? subCategoryId)
        {
            if(subCategoryId!=null)
            {
                var subCategoryFromDb =await _context.SubCategories.Where(s => s.Id == subCategoryId).FirstOrDefaultAsync();
                _context.SubCategories.Remove(subCategoryFromDb);
                await _context.SaveChangesAsync();
                return Json(new EditSubCategoryVM());
            }
            else
            {
                return BadRequest();
            }
        }
    }
}