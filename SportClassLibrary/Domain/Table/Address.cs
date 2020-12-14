using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Table
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public int Number { get; set; }

        // Not required
        public string Complement { get; set; }

        [Required]
        public string Neighbourhood { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [RegularExpression(@"^\d{5}[\.- ]?\d{3}$", ErrorMessage = "Invalid CEP")]
        public string ZipCode { get; set; }
    }
}
