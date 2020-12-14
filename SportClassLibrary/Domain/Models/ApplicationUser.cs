using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Domain.Table;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
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

        public ICollection<FriendsList> friends { get; set; }

        // Estabelecer a relação de Many-to-Many com `Event`
        // 1 Evento pode ter N Participantes
        // 1 Usuário pode participar de N Eventos
        public ICollection<UserEvent> Calendar { get; set; }
        // Estabelecendo a relação de One-to-Many com `Event`
        // 1 Evento é criado por um Usuário
        // 1 Usuário pode criar N Eventos
        public ICollection<Event> MyEvents { get; set; }
    }
}
