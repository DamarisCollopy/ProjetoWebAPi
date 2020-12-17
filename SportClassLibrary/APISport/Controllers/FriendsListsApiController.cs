using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataIdentity.DataContext;
using Domain.Table;
using WebSport.Models;
using Domain.Models;

namespace APISport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsListsApiController : ControllerBase
    {
        private readonly WebSportContext _context;
        private readonly IUser _user;

        public FriendsListsApiController(WebSportContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        // GET: api/FriendsListsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FriendsList>>> GetfriendsLists()
        {
            var webSportContext = _context.friendsLists.Include(f => f.ApplicationUser);
            return await webSportContext.ToListAsync();
        }

        // GET: api/FriendsListsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FriendsList>> GetFriendsList(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var friendsList = await _context.friendsLists
                .Include(f => f.ApplicationUser)
                .FirstOrDefaultAsync(m => m.FriendsId == id);
            if (friendsList == null)
            {
                return NotFound();
            }

            return friendsList;
        }

        // PUT: api/FriendsListsApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriendsList(int id, FriendsList friendsList)
        {
            if (id != friendsList.FriendsId)
            {
                return BadRequest();
            }

            _context.Entry(friendsList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendsListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FriendsListsApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FriendsList>> PostFriendsList(FriendsList friendsList)
        {
            _context.friendsLists.Add(friendsList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFriendsList", new { id = friendsList.FriendsId }, friendsList);
        }

        // DELETE: api/FriendsListsApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FriendsList>> DeleteFriendsList(int id)
        {
            var friendsList = await _context.friendsLists
                .Include(f => f.ApplicationUser)
                .FirstOrDefaultAsync(m => m.FriendsId == id);

            if (friendsList == null)
            {
                return NotFound();
            }

            _context.friendsLists.Remove(friendsList);
            await _context.SaveChangesAsync();
            

            return friendsList;
        }

        private bool FriendsListExists(int id)
        {
            return _context.friendsLists.Any(e => e.FriendsId == id);
        }
    }
}
