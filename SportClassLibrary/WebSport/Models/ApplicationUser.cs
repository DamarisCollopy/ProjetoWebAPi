using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebSport.Areas.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Username is required", AllowEmptyStrings = false)]
        [Display(Name = "Name")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
            "Numbers and special characters are not allowed in the name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "LastName is required", AllowEmptyStrings = false)]
        [Display(Name = "LastName")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
            "Numbers and special characters are not allowed in the surname.")]
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

        [Required]
        [StringLength(11, ErrorMessage = "Identifier too long (11 character limit).")]
        public string Identifier { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(10, MinimumLength = 6)]
        [Display(Name = "PasswordHash")]
        public override string PasswordHash { get; set; }

    }
}
