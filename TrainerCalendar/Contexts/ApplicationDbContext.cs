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
            //multiple courses for single skill (skill to course one to many)
            builder.Entity<Course>()
                .HasOne<Skill>(c => c.Skill)
                .WithMany(c => c.Courses)
                .HasForeignKey(s => s.SkillId)
                .HasConstraintName("Fk_session_To_Course_Course_Id");

            //multiple session can be scheduled for single skill (skill to session one to many)
            builder.Entity<Session>()
                .HasOne<Skill>(session => session.Skill)
                .WithMany(skill => skill.Sessions)
                .HasForeignKey(session => session.SkillId)
                .HasConstraintName("Fk_Session_To_Skill_Skill_Id");

            builder.Entity<Trainer>()
                .HasMany<Skill>(s => s.Skills)
                .WithMany(t => t.Trainers);

            base.OnModelCreating(builder);
        }
    }
}
