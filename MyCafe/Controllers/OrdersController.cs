using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;
using MyCafe.Models;
using MyCafe.Models.OrderViewModels;
using MyCafe.Services;
using MyCafe.Utility;

namespace MyCafe.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private int _pageSize = 2;

        public OrdersController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        //Get All Order History For Admin
        [Authorize(Roles = SD.AdminEndUser)]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string searchId, string searchDate, string searchEmail, int productPage = 1)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderDetailsList = await _context.OrderDetails.ToListAsync()
            };
            if (searchId != null || searchEmail != null || searchDate != null)
            {
                if (searchId != null)
                {
                    orderListVM.OrderHeadersList = await _context.OrderHeaders.Where(o => o.Id == Convert.ToInt32(searchId)).ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        orderListVM.OrderHeadersList = await _context.OrderHeaders.Where(o => o.Email.ToLower().Equals(searchEmail.ToLower())).ToListAsync();
                    }
                    else
                    {
                        if (searchDate != null)
                        {
                            orderListVM.OrderHeadersList = await _context.OrderHeaders.Where(o => o.OrderDate.ToShortDateString().ToString() == searchDate.ToString()).ToListAsync();
                        }
                    }
                }
            }
            else
            {
                orderListVM.OrderHeadersList = await _context.OrderHeaders.ToListAsync();
            }

            _pageSize = 10;
            var count = orderListVM.OrderHeadersList.Count;
            orderListVM.OrderHeadersList = orderListVM.OrderHeadersList.Skip((productPage - 1) * _pageSize).Take(_pageSize).ToList();
            orderListVM.PagingInfo = new Models.PagingInfo
            {
                ItemsPerPage = _pageSize,
                CurrentPage = productPage,
                TotalItem = count
            };
            return View(orderListVM);
        }

        //Get Order History For User(Himself/Herself)
        public async Task<IActionResult> CustomerOrderHistory(int productPage = 1)
        {
            _pageSize = 2;
            var userId = ((ClaimsIdentity)this.User.Identity)
                            .FindFirst(ClaimTypes.NameIdentifier).Value;
            var count = (await _context.OrderHeaders.Include(o => o.ApplicationUser)
                            .Where(o => o.ApplicationUser.Id == userId).ToListAsync()).Count;

            OrderListVM orderListHistory = new OrderListVM()
            {
                OrderDetailsList = await _context.OrderDetails.ToListAsync(), //Not using eagerloading becuse we do not need MenuItem here.
                OrderHeadersList = await _context.OrderHeaders.Include(o => o.ApplicationUser)
                                        .Where(o => o.ApplicationUser.Id == userId)
                                        .OrderBy(o => o.Id)
                                        .OrderByDescending(o => o.Id)
                                        .Skip((productPage - 1) * _pageSize)
                                        .Take(_pageSize)
                                        .ToListAsync(),
                PagingInfo = new Models.PagingInfo
                {
                    TotalItem = count,
                    CurrentPage = productPage,
                    ItemsPerPage = _pageSize
                }
            };
            return View(orderListHistory);
        }

        //Get Orders that are ready for pickup for front-desk
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> OrderPickup(int productPage = 1)
        {
            _pageSize = 2;
            var count = (await _context.OrderHeaders.Where(o => o.Status == SD.StatusReady).ToListAsync()).Count;
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderDetailsList = await _context.OrderDetails.ToListAsync(), //Not using eagerloading becuse we do not need MenuItem here.
                OrderHeadersList = await _context.OrderHeaders
                                    .Include(o => o.ApplicationUser)
                                    .Where(o => o.Status == SD.StatusReady)
                                    .Skip((productPage - 1) * _pageSize)
                                    .Take(_pageSize)
                                    .ToListAsync(),
                PagingInfo = new Models.PagingInfo()
                {
                    TotalItem = count,
                    CurrentPage = productPage,
                    ItemsPerPage = _pageSize
                }
            };
            return View(orderListVM);
        }

        //Get All Order For Managing
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> ManageOrder(int productPage = 1)
        {
            var ordersList = await _context.OrderHeaders
                                        .Include(o => o.ApplicationUser)
                                        .Where(o => o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProgress)
                                        .OrderBy(o => o.Id)
                                        .ToListAsync();

            OrderListVM manageOrderList = new OrderListVM()
            {
                OrderDetailsList = await _context.OrderDetails.ToListAsync(), //Not using eagerloading becuse we do not need MenuItem here.
                OrderHeadersList = ordersList.Skip((productPage - 1) * _pageSize).Take(_pageSize).ToList(),
                PagingInfo = new Models.PagingInfo()
                {
                    TotalItem = ordersList.Count,
                    ItemsPerPage = _pageSize,
                    CurrentPage = productPage
                }
            };
            return View(manageOrderList);
        }

        //Get Details/id
        [Authorize(Roles = "Admin, Staff")]
        public async Task<JsonResult> OrderDetailById(int orderId)
        {
            OrderDetailsVM orderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync(),
                OrderDetails = await _context.OrderDetails.Where(o => o.OrderHeaderId == orderId).Include(o => o.MenuItem).ThenInclude(m => m.SubCategory).ToListAsync()
            };
            return Json(orderDetailsVM);
        }

        //Change The Status Of Order To In Progress
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> OrderInProgress(int orderId)
        {
            var OrderHeaderFromDb = await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync();
            OrderHeaderFromDb.Status = SD.StatusInProgress;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageOrder));
        }

        //Change Status To Read For Pickup 
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            var OrderHeaderFromDb = await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync();
            OrderHeaderFromDb.Status = SD.StatusReady;
            await _context.SaveChangesAsync();

            var userEmail = (await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync()).Email;
            await _emailSender.SendStatusEmailAsync(userEmail, SD.StatusReady, orderId.ToString());

            return RedirectToAction(nameof(ManageOrder));
        }

        //Change Status To Cancelled
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> OrderCancelled(int orderId)
        {
            var OrderHeaderFromDb = await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync();
            OrderHeaderFromDb.Status = SD.StatusCancelled;
            await _context.SaveChangesAsync();

            var userEmail = (await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync()).Email;
            await _emailSender.SendStatusEmailAsync(userEmail, SD.StatusCancelled, orderId.ToString());

            return RedirectToAction(nameof(ManageOrder));
        }

        //Change Status to Pickup
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> OrderComplete(int orderId)
        {
            var OrderHeaderFromDb = await _context.OrderHeaders.Where(o => o.Id == orderId).FirstOrDefaultAsync();
            OrderHeaderFromDb.Status = SD.StatusCompleted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(OrderPickup));
        }

        //Get list of OrderDetails for download
        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult OrderHistoryList()
        {
            return View();
        }

        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost]
        public async Task<IActionResult> DownLoadHistory(OrderDownloadVM orderDownloadVM)
        {
            if (!(orderDownloadVM.FromDate.Equals(DateTime.MinValue) || orderDownloadVM.ToDate.Equals(DateTime.MinValue)))
            { 

                var orderIdList = await _context.OrderHeaders.Where(o => o.OrderDate >= orderDownloadVM.FromDate && o.OrderDate <= orderDownloadVM.ToDate).OrderBy(o => o.Id).Select(o => o.Id).ToListAsync();
                List<OrderDetails> orderDetailsList = new List<OrderDetails>();
                foreach (var orderId in orderIdList)
                {
                    var orderDetails = await _context.OrderDetails.Where(o => o.OrderHeaderId == orderId).ToListAsync();
                    orderDetailsList.AddRange(orderDetails);

                }
                byte[] bytes = Encoding.ASCII.GetBytes(ConvertToString(orderDetailsList));
                return File(bytes, "application/text", "OrderDetail.csv");
            }
            else
            {
                return RedirectToAction(nameof(OrderHistoryList));
            }
        }

        public String ConvertToString<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            //Removing the columns that we do not want in excel-sheet
            table.Columns.Remove("Id");
            table.Columns.Remove("OrderHeader");
            table.Columns.Remove("MenuItem");

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in table.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            return sb.ToString();
        }

    }
}