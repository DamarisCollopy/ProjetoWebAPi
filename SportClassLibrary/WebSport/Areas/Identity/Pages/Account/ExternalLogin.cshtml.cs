﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WebSport.Areas.Models;

namespace WebSport.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Username is required", AllowEmptyStrings = false)]
            [Display(Name = "Name")]
            [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
          "Numbers and special characters are not allowed in the name.")]
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
            [Required]
            [StringLength(11, ErrorMessage = "Identifier too long (11 character limit).")]
            public string Identifier { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        Name =  info.Principal.FindFirstValue(ClaimTypes.GivenName),
                        lastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                        //birthDate = info.Principal.FindFirstValue(ClaimTypes.DateOfBirth),
                        Street = info.Principal.FindFirstValue(ClaimTypes.Locality),
                        zipCode = info.Principal.FindFirstValue(ClaimTypes.PostalCode),
                        PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                        City = info.Principal.FindFirstValue(ClaimTypes.StateOrProvince),
                        Password = info.Principal.FindFirstValue(ClaimTypes.Hash)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Name = Input.Name,
                    lastName = Input.lastName,
                    birthDate = Input.birthDate,
                    Street = Input.Street,
                    Neighborhood = Input.Neighborhood,
                    City = Input.City,
                    zipCode = Input.zipCode,
                    PhoneNumber = Input.PhoneNumber,
                    Identifier = Input.Identifier,
                    Email = Input.Email
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                        {
                            await _userManager.AddClaimAsync(user,
                                info.Principal.FindFirst(ClaimTypes.GivenName));
                        }

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                      
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
