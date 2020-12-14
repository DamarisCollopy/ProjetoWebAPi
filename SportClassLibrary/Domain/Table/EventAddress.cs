using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Table
{
    public class EventAddress : Address
    {
        // A classe herda de `Address`

        // Estabelecendo a relação One-to-One com `Evento`
        // Mesmo que dois eventos ocorram no mesmo endereço, o endereço para cada evento é tratado como um entidade independente.
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
