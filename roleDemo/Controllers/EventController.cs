using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roleDemo.Data;
using roleDemo.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace roleDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private ApplicationDbContext _context;


        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> OnGetAsync()
        {
            EventRepo e = new EventRepo(_context);
            var query = e.GetAll();
            var res = new
            {
                EventArray = query
            };
            return new ObjectResult(res);
        }


        [HttpGet]
        [Route("getOne")]
        public IActionResult GetOne(int EventID)
        {
            EventRepo e = new EventRepo(_context);
            var query = e.GetOne(EventID);
            var res = new
            {
                EventArray = query
            };
            return new ObjectResult(res);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Manager")]
        public async Task<IActionResult> OnPostAsync([FromBody] Event Event)
        {
            if (ModelState.IsValid)
            {
                EventRepo e = new EventRepo(_context);
                var result = e.Create(Event.EventName, Event.Description, Event.Date, Event.Time);
                if (result)
                {
                    var res = new
                    {
                        StatusCode = "Ok",
                        EventName = Event.EventName
                    };

                    return new ObjectResult(res);
                }
            }


            var invalidRes = new
            {
                StatusCode = "Invalid",
                EventName = Event.EventName
            };


            return new ObjectResult(invalidRes);

        }


        [HttpGet]
        [Route("Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Manager")]
        public IActionResult Delete(int EventID)
        {
            try
            {
                EventRepo e = new EventRepo(_context);
                e.Delete(EventID);
                var res = new
                {
                    StatusCode = "ok",
                    message = "Deleted"
                };

                return new ObjectResult(res);
            }
            catch
            {
                var errorRes = new
                {
                    StatusCode = "error",
                    message = "You can't delete an event with attendees."
                };

                return new ObjectResult(errorRes);
            }
        }


        [HttpGet]
        [Route("AttendEvent")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]


        public IActionResult AttendEvent(int EventID)
        {
            AttendeeRepo c = new AttendeeRepo(_context);
            EventRepo e = new EventRepo(_context);
            var claim = HttpContext.User.Claims.ElementAt(0);
            string email = claim.Value;
            var user = c.GetOneByEmail(email);

            try
            {
                e.AttendEvent(EventID, user.ID);

                var res = new
                {
                    StatusCode = "You are attending this event.",
                };
                return new ObjectResult(res);


            }
            catch
            {
                var res = new
                {
                    error = "You are already attending this event.",
                };
                return new ObjectResult(res);
            }
        }


        [HttpGet]
        [Route("CancelAttendance")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CancelAttendance(int EventID)
        {
            AttendeeRepo c = new AttendeeRepo(_context);
            EventRepo e = new EventRepo(_context);
            var claim = HttpContext.User.Claims.ElementAt(0);
            string email = claim.Value;
            var user = c.GetOneByEmail(email);
            try
            {
                e.CancelAttendance(EventID, user.ID);
                var res = new
                {
                    StatusCode = "You are no longer attending this event",
                };
                return new ObjectResult(res);
            }
            catch
            {
                var res = new
                {
                    error = "You were not attending this event.",
                };
                return new ObjectResult(res);
            }
        }

        [HttpGet]
        [Route("MyEvents")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult MyEvents()
        {
            EventAttendeeRepo ea = new EventAttendeeRepo(_context);
            AttendeeRepo c = new AttendeeRepo(_context);
            var claim = HttpContext.User.Claims.ElementAt(0);
            string email = claim.Value;
            var user = c.GetOneByEmail(email);
            var query = ea.GetCurrentAttandingEvents(user.ID);
            var res = new
            {
                EventArray = query,
            };
            return new ObjectResult(res);
        }


        [HttpGet]
        [Route("GetAttendees")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAttendees(int EventID)
        {
            EventAttendeeRepo ea = new EventAttendeeRepo(_context);
            var query = ea.ViewAttendees(EventID);
            var res = new
            {
                EventArray = query,
            };
            return new ObjectResult(res);
        }
    }
}
