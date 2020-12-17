using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataIdentity.DataContext;
using Domain.Table;
using Microsoft.AspNetCore.Authorization;
using DataIdentity.Repository;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using Domain.APIService;
using Newtonsoft.Json;


namespace WebSport.Controllers
{
    [Authorize]
    public class FriendsListsController : Controller
    {
        private readonly WebSportContext _context;
        private readonly IBaseContext _baseContext;
        private readonly UserManager<ApplicationUser> _userManager;
        BaseApi _baseApi = new BaseApi();

        public FriendsListsController(WebSportContext context, IBaseContext baseContext, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _baseContext = baseContext;
            _userManager = userManager;
        }


        // GET: FriendsLists
        public async Task<IActionResult> Index()
        {
            List<FriendsList> friends = new List<FriendsList>();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync("api/FriendsListsApi");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                friends = JsonConvert.DeserializeObject<List<FriendsList>>(results);
            }

            return View(friends);
        }

        // GET: FriendsLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            FriendsList friends = new FriendsList();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync($"api/FriendsListsApi/{id}");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                friends = JsonConvert.DeserializeObject<FriendsList>(result);
            }
            return View(friends);
        }
        // GET: FriendsLists/Create
        public async Task<IActionResult> Create()
        {
            var user = await _context.Users.ToListAsync();
            return View(user);
        }
        public IActionResult CreateSave()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSave(FriendsList friendsList, string? id)
        {
            var identidade = _userManager.GetUserId(User);
            if (id == null)
            {
                return NotFound();
            }

            if (!_baseContext.FriendsExist(id))
            {
                if (identidade.Contains(id))
                {
                    return RedirectToAction(nameof(Index));
                }
                friendsList.ApplicationUserId = id;
                var post = _baseApi.Consumer.PostAsJsonAsync<FriendsList>("api/FriendsListsApi", friendsList);
                post.Wait();

                var result = post.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Text = "Friend has already been added to the list!!";
            return RedirectToAction(nameof(Index));
        }

        // GET: FriendsLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            FriendsList friends = new FriendsList();
            HttpResponseMessage res = await _baseApi.Consumer.GetAsync($"api/FriendsListsApi/{id}");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                friends = JsonConvert.DeserializeObject<FriendsList>(result);
            }
            return View(friends);
        }

        // POST: FriendsLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(FriendsList friends)
        {
            var delete = await _baseApi.Consumer.DeleteAsync($"api/FriendsListsApi/{friends.FriendsId}");
            return RedirectToAction("Index");
        }
    }
}
