using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataIdentity.DataContext;
using Domain.Table;

namespace APISport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventsApiController : ControllerBase
    {
        private readonly WebSportContext _context;

        public UserEventsApiController(WebSportContext context)
        {
            _context = context;
        }

        // GET: api/UserEventsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEvent>>> GetUserEvents()
        {
            var webSportContext = _context.UserEvents
                .Include(u => u.Event)
                .Include(u => u.User);

            return await webSportContext.ToListAsync();
        }

        // GET: api/UserEventsApi/5
        [HttpGet("{EventId}")]
        public async Task<ActionResult<UserEvent>> GetUserEvent(int EventId)
        {
            if (EventId == null)
            {
                return NotFound();
            }

            var userEvent = await _context.UserEvents
                .Include(u => u.Event)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.EventId == EventId);
            if (userEvent == null)
            {
                return NotFound();
            }

            return userEvent;
        }


        // POST: api/UserEventsApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserEvent>> PostUserEvent(UserEvent userEvent)
        {
            _context.UserEvents.Add(userEvent);
           
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUserEvent", new { id = userEvent.UserId }, userEvent);
        }

        // DELETE: api/UserEventsApi/5
        [HttpDelete("{UserId}")]
        public async Task<ActionResult<UserEvent>> DeleteUserEvent(string UserId)
        {

            var userEvent = await _context.UserEvents
                .Include(u => u.Event)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserId.Contains(UserId));

            _context.UserEvents.Remove(userEvent);
            await _context.SaveChangesAsync();

            int idEvent = userEvent.EventId;
            Event eventos = _context.Events.Find(idEvent);
            eventos.numbParticipants -= 1;
            eventos.confirmEvent = false;
            eventos.waitEvent = true;
            _context.SaveChanges();

            return userEvent;
        }
    }
}
