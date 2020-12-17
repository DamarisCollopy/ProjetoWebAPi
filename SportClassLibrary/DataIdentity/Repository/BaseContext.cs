using DataIdentity.DataContext;
using Domain.Models;
using Domain.Table;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIdentity.Repository
{
    public class BaseContext : IBaseContext
    {
        private readonly WebSportContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public BaseContext(WebSportContext context, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public bool FriendsExist(string id)
        {
            return _context.friendsLists.Any(e => e.ApplicationUserId.Contains(id));
        }
        public bool FriendsExistApp(string id)
        {
            return _context.Users.Any(e => e.Id.Contains(id));
        }
        public bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
        public bool EventOrg(string UserId, int id)
        {
            return _context.Events.Any(e => e.OrganizerId.Contains(UserId) && e.Id == id);
        }
        public bool EventCount(int id)
        {
            Event eventos = _context.Events.Find(id);
            if (eventos.MaxParticipants == eventos.numbParticipants)
            {
                eventos.waitEvent = false;
                eventos.confirmEvent = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool EventCountMore(int id)
        {
            Event eventos = _context.Events.Find(id);
            eventos.numbParticipants += 1;
            if (eventos.MaxParticipants == eventos.numbParticipants)
            {
                eventos.confirmEvent = true;
                eventos.waitEvent = false;
                _context.SaveChanges();
                return true;
            }

            eventos.confirmEvent = false;
            eventos.waitEvent = true;
            _context.SaveChanges();
            return false;
        }
        public bool UserEventExists(string id)
        {
            return _context.UserEvents.Any(e => e.UserId == id);
        }
        public bool EvenEventExists(int id, string UserId)
        {
            return _context.UserEvents.Any(e => e.EventId == id && e.UserId.Contains(UserId));
        }

    }
}
