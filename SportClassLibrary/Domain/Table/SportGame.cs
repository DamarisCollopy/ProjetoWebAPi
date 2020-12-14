using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Table
{
    public class SportGame
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SportName { get; set; }

        // One-To-Many
        public ICollection<Event> Events { get; set; }
    }
}
