using Domain.Models;
using Domain.Table;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataIdentity.DataContext
{
    public class WebSportContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public WebSportContext(DbContextOptions<WebSportContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventAddress> EventAddresses { get; set; }
        public DbSet<SportGame> SportGames { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<FriendsList> friendsLists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer(@"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WebSport;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Declarando chave composta
            builder.Entity<UserEvent>().HasKey(userEvent => new { userEvent.UserId, userEvent.EventId });
            // Many-to-Many entre `Event` e `User`
            builder.Entity<UserEvent>()
                .HasOne(userEvent => userEvent.User)
                .WithMany(user => user.Calendar)
                .HasForeignKey(userEvent => userEvent.UserId)
                .IsRequired();

            builder.Entity<UserEvent>()
                .HasOne(userEvent => userEvent.Event)
                .WithMany(_event => _event.Calendar)
                .HasForeignKey(userEvent => userEvent.EventId)
                .IsRequired();

            builder.Entity<FriendsList>()
            .HasOne(c => c.ApplicationUser)
             .WithMany(x => x.friends)
             .HasForeignKey(f => f.ApplicationUserId)
             .HasConstraintName("ApplicationUserId")
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();

            builder.Entity<Event>()
             .HasOne(a => a.Address)
             .WithOne(b => b.Event)
             .HasForeignKey<EventAddress>(b => b.EventId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
