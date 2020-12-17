using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataIdentity.DataContext;
using Domain.Table;
using Domain.APIService;

using Domain.Models;
using DataIdentity.Repository;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebSport.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly WebSportContext _context;
        private readonly IBaseContext _baseContext;
        private readonly UserManager<ApplicationUser> _userManager;
        BaseApi _baseApi = new BaseApi();

        public EventsController(WebSportContext context, IBaseContext baseContext,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _baseContext = baseContext;
            _userManager = userManager;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            List<Event> events = new List<Event>();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync("api/EventsApi");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                events = JsonConvert.DeserializeObject<List<Event>>(results);
            }

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Event events = new Event();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync($"api/EventsApi/{id}");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                events = JsonConvert.DeserializeObject<Event>(result);
            }
            return View(events);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["SportId"] = new SelectList(_context.SportGames, "Id", "SportName");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaxParticipants,Day,StartTime,EndTime,Street,Neighborhood,City,SportId")] Event events)
        {
            var identidade = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                events.OrganizerId = identidade;
                events.waitEvent = true;
                events.numbParticipants += 1;
                _context.Add(events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SportId"] = new SelectList(_context.SportGames, "Id", "SportName", events.SportId);
            return View(events);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Somente edita quem e o dono do evento
            var identidade = _userManager.GetUserId(User);

            if (!_baseContext.EventOrg(identidade, (int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
         
            ViewData["SportId"] = new SelectList(_context.SportGames, "Id", "SportName", @event.SportId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaxParticipants,numbParticipants,Day,StartTime,EndTime,Street,Neighborhood,City,waitEvent,confirmEvent,OrganizerId,SportId")] Event events)
        {
            var identidade = _userManager.GetUserId(User);

            if (id != events.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    events.OrganizerId = identidade;
                    events.waitEvent = true;
                    events.numbParticipants += 1;
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(events.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["SportId"] = new SelectList(_context.SportGames, "Id", "SportName", events.SportId);
            return View(events);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Somente deleta quem e o dono do evento
            var identidade = _userManager.GetUserId(User);
            if (!_baseContext.EventOrg(identidade, (int)id))
            {
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Events
                .Include(p => p.Organizer)
                .Include(p => p.SportGame)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
