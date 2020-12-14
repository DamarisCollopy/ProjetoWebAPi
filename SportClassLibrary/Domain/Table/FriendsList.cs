using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Table
{
    public class FriendsList
    {
        [Key]
        public int FriendsId { get; set; }
        //Foreign Key entity
        public string ApplicationUserId { get; set; }
        // Um amigo tem muitas listas
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
