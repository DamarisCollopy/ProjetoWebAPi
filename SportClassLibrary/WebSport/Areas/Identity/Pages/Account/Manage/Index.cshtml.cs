using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace WebSport.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }
            [Required(ErrorMessage = "LastName is required", AllowEmptyStrings = false)]
            [Display(Name = "LastName")]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
                "Numbers and special characters are not allowed in the LastName.")]
            public string lastName { get; set; }

            [Required]
            [Display(Name = "Birth Date")]
            [DataType(DataType.Date)]
            public DateTime birthDate { get; set; }
            public string Street { get; set; }
            public string Neighborhood { get; set; }
            public string City { get; set; }

            [Required]
            [RegularExpression(@"^\d{8}(-\d{5})?$", ErrorMessage = "Invalid Zip Code.")]
            public string zipCode { get; set; }

            [StringLength(12, MinimumLength = 9, ErrorMessage = "Invalid Phone Number")]
            [Required(ErrorMessage = "Phone Number is required")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Name = user.Name,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user,
                    Input.PhoneNumber);

                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException(
                        $"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }

            user.lastName = Input.lastName;
            user.birthDate = Input.birthDate;
            user.Street = Input.Street;
            user.Neighborhood = Input.Neighborhood;
            user.City = Input.City;
            user.zipCode = Input.zipCode;
            user.PhoneNumber = Input.PhoneNumber;

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return Page();
        }
    }
}
