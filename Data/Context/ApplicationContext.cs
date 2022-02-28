using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
        }
    }
}
