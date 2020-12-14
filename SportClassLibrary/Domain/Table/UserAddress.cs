using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Table
{
    public class UserAddress : Address
    {
        //A classe herda de `Address`

        // Estabelecendo a relação One-to-One com `ApplicationUser`
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
