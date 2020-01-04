using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;
using MyCafe.Models;
using MyCafe.Utility;

namespace MyCafe.Controllers.API
{
    [Produces("application/json")]
    [Route("api/OrderHistoryAPI")]
    public class OrderHistoryAPIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderHistoryAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get: api/OrderHistoryAPI
        [Authorize(Roles=SD.AdminEndUser)]
        [HttpGet]
        public IActionResult GetOrderHistory(string fromDate=null , string toDate=null)
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            if (fromDate == null)
            {
                //Assigning the date before the project was lunched
                startDate = new DateTime(2019, 01, 12, 00, 00, 00);
            }
            else
            {
                startDate = Convert.ToDateTime(fromDate);
            }

            if (toDate == null)
            {
                //assign today's date to get the details till today
                endDate = DateTime.Now;
            }
            else
            {
                endDate = Convert.ToDateTime(toDate);
            }

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            var orderIdList = _context.OrderHeaders
                            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                            .Select(o => o.Id)
                            .ToList();

            foreach (var orderId in orderIdList)
            {
                var orderDetails = _context.OrderDetails
                    .Where(o => o.OrderHeaderId == orderId)
                    .ToList();

                //foreach (var orderDetail in orderDetails)
                //{
                //    orderDetailsList.Add(orderDetail);
                //}
                orderDetailsList.AddRange(orderDetails);
            }
            return Ok(new { orderDetailList = orderDetailsList });
        }

    }
}