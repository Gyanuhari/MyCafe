using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCafe.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCafe.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserAPIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get(string type, string query=null)
        {
            //For Name Search
            if (type.Equals("id") && query != null)
            {
                //var userQuery = await _context.Users
                //    .Where(u => u.FirstName.ToLower().Contains(query.ToLower())|| 
                //    u.LastName.ToLower().Contains(query.ToLower())).ToListAsync();
                var nameQuery = await _context.OrderHeaders
                   .Where(o => o.Id==Convert.ToInt32(query)).Select(o => o.Id).ToListAsync();
                    return Ok(nameQuery);
            }

            //For Phone Search
            if (type.Equals("date") && query != null)
            {
                var userQuery = await _context.OrderHeaders.Where(o=>o.OrderDate.ToShortDateString().Contains(query.ToString())).ToListAsync();
                return Ok(userQuery);
            }

            //For Email Search
            if (type.Equals("email") && query!=null)
            {
                var userQuery =await _context.Users.Where(u => u.Email.ToLower().Contains(query.ToLower())).ToListAsync();
                return Ok(userQuery);
            }

            return Ok();
        }
    }
}
