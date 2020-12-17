using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataIdentity.DataContext;
using Domain.Table;
using DataIdentity.Repository;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Domain.APIService;
using System.Net.Http;
using Newtonsoft.Json;

namespace WebSport.Controllers
{
    public class UserEventsController : Controller
    {
        private readonly WebSportContext _context;
        private readonly IBaseContext _baseContext;
        private readonly UserManager<ApplicationUser> _userManager;
        BaseApi _baseApi = new BaseApi();

        public UserEventsController(WebSportContext context, IBaseContext baseContext, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _baseContext = baseContext;
            _userManager = userManager;
        }

        // GET: UserEvents
        public async Task<IActionResult> Index()
        {
            List<UserEvent> userFriends = new List<UserEvent>();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync("api/UserEventsApi");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                userFriends = JsonConvert.DeserializeObject<List<UserEvent>>(results);
            }

            return View(userFriends);
        }

        // GET: UserEvents/Details/5
        public async Task<IActionResult> Details(int EventId)
        {
            UserEvent eventUser = new UserEvent();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync($"api/UserEventsApi/{EventId}");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                eventUser = JsonConvert.DeserializeObject<UserEvent>(result);
            }
            return View(eventUser);
        }

        // GET: UserEvents/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id");
            return View();
        }

        // POST: UserEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId")] UserEvent userEvent)
        {
            var identidade = _userManager.GetUserId(User);
            int id = userEvent.EventId;
            if (_baseContext.EvenEventExists(id, identidade))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                userEvent.UserId = identidade;
                
                var post = _baseApi.Consumer.PostAsJsonAsync<UserEvent>("api/UserEventsApi", userEvent);
                post.Wait();

                var result = post.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                if (_baseContext.EventCount(id))
                {
                    return View(ViewBag.Text = "Limit number reached");
                }

                if (_baseContext.EventCountMore(id))
                {
                    return RedirectToAction(nameof(Index));
                }

                if (!_baseContext.EventCountMore(id))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));

            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id", userEvent.EventId);
            return View(userEvent);
        }

        // GET: UserEvents/Delete/5
        public async Task<IActionResult> Delete(string UserId)
        {
            UserEvent userEvent = new UserEvent();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync($"api/UserEventsApi/{UserId}");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                userEvent = JsonConvert.DeserializeObject<UserEvent>(result);
            }
            return View(userEvent);
        }

        // POST: UserEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(UserEvent userEvents)
        {
            var delete = await _baseApi.Consumer.DeleteAsync($"api/UserEventsApi/{userEvents.UserId}");
            return RedirectToAction(nameof(Index));
        }

        private bool UserEventExists(string id)
        {
            return _context.UserEvents.Any(e => e.UserId == id);
        }
    }
}
