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

        // Número máximo de participantes, não obrigatório
        public int MaxParticipants { get; set; }

        // Hora de início e fim
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Estabelecendo relação "Many-to-One" com Usuário
        // 1 Evento é criado por um Usuário
        // 1 Usuário pode criar N Eventos
        [ForeignKey("Organizer")]
        public string OrganizerId { get; set; }
        public ApplicationUser Organizer { get; set; }

        // Estabelecendo relação de Many-to-Many com Usuários
        // 1 Evento pode ter N Participantes
        // 1 Usuário pode participar de N Eventos
        public ICollection<UserEvent> Calendar { get; set; }

        // Estabelecendo a relação One-to-One com `EventAddress`
        [ForeignKey("Address")]
        public int AddressId { get; set; }
        [Required]
        public EventAddress Address { get; set; }

        // Estabelecendo a relação "Many-to-One" com `Sport`
        // 1 esporte pode compor N eventos
        // 1 evento tem apenas 1 esporte
        [ForeignKey("SportGame")]
        public int SportId { get; set; }
        [Required]
        public SportGame SportGame { get; set; }
    }
}
