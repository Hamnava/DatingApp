using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, int,
                                      IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>,
                                      IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // For Identity Part
            builder.Entity<ApplicationUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<ApplicationRole>()
               .HasMany(ur => ur.UserRoles)
               .WithOne(ur => ur.Role)
               .HasForeignKey(ur => ur.RoleId)
               .IsRequired();


            // For User Like part
            builder.Entity<UserLike>().HasKey(k => new { k.SourceUserId, k.LikedUserId });

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(d => d.LikedUsers)
                .HasForeignKey(f => f.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<UserLike>()
               .HasOne(s => s.LikedUser)
               .WithMany(d => d.LikedByUsers)
               .HasForeignKey(f => f.LikedUserId)
               .OnDelete(DeleteBehavior.Cascade);

            // For message part
            builder.Entity<Message>()
                .HasOne(u=> u.Recipeint)
                .WithMany(u=> u.MessageReceived)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(u => u.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
