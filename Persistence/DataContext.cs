using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
	public class DataContext: IdentityDbContext<AppUser>
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Activity> Activities { get; set; }
		public DbSet<ActivityAttendee> ActivityAttendees{ get; set; }
		public DbSet<Photo> Photos { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<UserFollowing> UsersFollowings { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<ActivityAttendee>(
				x => x.HasKey(aa => new {aa.AppUserId, aa.ActivityId}));
			builder.Entity<ActivityAttendee>()
				.HasOne(u => u.AppUser).WithMany(a => a.Activities)
				.HasForeignKey(aa => aa.AppUserId);

			builder.Entity<ActivityAttendee>()
				.HasOne(u => u.Activity).WithMany(a => a.Attendees)
				.HasForeignKey(aa => aa.ActivityId);

			builder.Entity<Comment>()
				.HasOne(a => a.Activity)
				.WithMany(c => c.Comments)
				.OnDelete(DeleteBehavior.Cascade);
			// this means that if we delete an activity, all related comments will also be deleted
			builder.Entity<UserFollowing>
				(x =>
				{
					x.HasKey(uf => new { uf.ObserverId, uf.TargetId });
					x.HasOne(o => o.Observer)
						.WithMany(f => f.Followings)
						.HasForeignKey(o => o.ObserverId)
						.OnDelete(DeleteBehavior.Cascade);
					x.HasOne(t => t.Target)
						.WithMany(f => f.Followers)
						.HasForeignKey(t => t.TargetId)
						.OnDelete(DeleteBehavior.Cascade);
				});
		}
	}
}
