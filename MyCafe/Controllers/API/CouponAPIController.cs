using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCafe.Data;
using MyCafe.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCafe.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class CouponAPIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponAPIController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(double orderTotal, string couponCode=null)
        {
            var rtn = "";
            if(couponCode==null)
            {
                rtn = orderTotal + ":Error";
                return Ok(rtn);
            }
            var couponFromDb = _context.Coupons.Where(c => c.Name == couponCode).FirstOrDefault();
            if(couponFromDb==null)
            {
                rtn = orderTotal + ":Error";
                return Ok(rtn);
            }
            //Total Amount should be equal or greater than minimun amount
            if(couponFromDb.MinimumAmount>orderTotal)
            {
                rtn = orderTotal + ":Error";
                return Ok(rtn);
            }

            //For Discount in dollar
            if(Convert.ToInt32(couponFromDb.CouponType)==(int)Coupons.ECouponType.Dollar)
            {
                orderTotal = orderTotal - couponFromDb.Discount;
                rtn = orderTotal + ":Success";
                return Ok(rtn);
            }

            //For Discount in percentage
            if(Convert.ToInt32(couponFromDb.CouponType)==(int)Coupons.ECouponType.Percent)
            {
                orderTotal = orderTotal - (orderTotal * couponFromDb.Discount / 100);
                rtn = orderTotal + ":Success";
                return Ok(rtn);
            }
            return Ok();
        }
    }
}
