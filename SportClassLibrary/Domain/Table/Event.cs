using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Table
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        public int numbParticipants { get; set; }

        [Required]
        [Display(Name = "Day of the event")]
        [DataType(DataType.Date)]
        public DateTime Day { get; set; }

        [Display(Name = "StartTime")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH}")]
        public DateTime StartTime { get; set; }

        [Display(Name = "EndTime")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH}")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Street")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
            "Numbers and special characters are not allowed in the street.")]
        public string Street { get; set; }
        [Display(Name = "Neighborhood")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
           "Numbers and special characters are not allowed in the neighborhood.")]
        public string Neighborhood { get; set; }

        [Display(Name = "City")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
           "Numbers and special characters are not allowed in the city.")]
        public string City { get; set; }
        public bool waitEvent { get; set; }
        public bool confirmEvent { get; set; }

        // Estabelecendo relação "Many-to-One" com Usuário
        // 1 Evento é criado por um Usuário
        // 1 Usuário pode criar N Eventos
        [ForeignKey("Organizer")]
        public string OrganizerId { get; set; }
        public ApplicationUser Organizer { get; set; }
     
        // Estabelecendo a relação "Many-to-One" com `Sport`
        // 1 esporte pode compor N eventos
        // 1 evento tem apenas 1 esporte
        [ForeignKey("SportGame")]
        public int SportId { get; set; }
        [Required]
        public SportGame SportGame { get; set; }

    }
}
