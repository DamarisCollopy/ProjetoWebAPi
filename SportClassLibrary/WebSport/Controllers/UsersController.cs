using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebSport.Data;
using WebSport.Areas.Models;
using Microsoft.AspNetCore.Identity;
using WebSport.Models;

namespace WebSport.Controllers
{
    public class UsersController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly WebSportContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(WebSportContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
       
        
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);


            user.Name = model.Name;
            user.lastName = model.lastName;
            user.PhoneNumber = model.Telephone;
            user.PasswordHash = model.PasswordHash;

            _context.Update(user);
            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction();
        }
    }

}
