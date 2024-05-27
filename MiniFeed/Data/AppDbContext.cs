using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniFeed.Models;

namespace MiniFeed.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users {  get; set; }
        public DbSet<Post> Posts {  get; set; }
        public DbSet<Follow> Follows {  get; set; }
        public DbSet<Like> Likes {  get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //seed roles into db
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                },
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            // establish relationship between User and Follow tables
            builder.Entity<Follow>()
            .HasKey(f => new { f.FollowerId, f.FollowedId });

            builder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.UsersFollowed)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follow>()
                .HasOne(f => f.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);

            // establish relationship between User and Like tables
            builder.Entity<Like>()
            .HasKey(l => new { l.UserId, l.PostId });

            builder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.LikedPosts)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
