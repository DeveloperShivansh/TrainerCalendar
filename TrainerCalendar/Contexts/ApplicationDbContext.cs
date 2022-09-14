using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Models;

namespace TrainerCalendar.Contexts
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Session> Sessions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //many to many relationship between courses and skills
            builder.Entity<Course>()
                .HasMany<Skill>(s => s.Skills)
                .WithMany(c => c.Courses);

            //multiple session can be scheduled for single skill (skill to session one to many)
            builder.Entity<Session>()
                .HasOne<Skill>(s => s.Skill)
                .WithMany(s => s.Sessions)
                .HasForeignKey(s => s.SkillId)
                .HasConstraintName("Fk_Session_To_SkillId");

            builder.Entity<Trainer>()
                .HasMany<Skill>(s => s.Skills)
                .WithMany(t => t.Trainers);

            base.OnModelCreating(builder);
        }
    }
}
