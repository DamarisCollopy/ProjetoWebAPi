using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Table
{
    public class UserEvent
    {
        // Join de User e Evento, o que seria a "Agenda"

        public int EventId { get; set; }
        public Event Event { get; set; }

        // Participantes
        public string UserId { get; set; }
        public ApplicationUser User
        {
            get; set;
        }
    }
}
