using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Contexts;
using TrainerCalendar.Models;

namespace TrainerCalendar.Db
{
    public interface ICourseDb
    {
        public Task<List<Course>> GetCourses(int? trainerId=-1, string? skillName="");
    }
    public class CourseDb : ICourseDb
    {
        private ApplicationDbContext _context;
        public CourseDb(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public async Task<List<Course>> GetCourses(int? trainerId=-1, string? skillName="")
        {
            List<Course> courses = new List<Course>();
            if (skillName != "" && skillName != null) return await _context.Courses.Include(c => c.Skill).Where(c => c.Skill.SkillName == skillName).ToListAsync();
            else if (trainerId != -1 && trainerId != null)
            {
                Trainer t = await _context.Trainers.Include(t => t.Skills).FirstOrDefaultAsync(t => t.TrainerId == trainerId);
                if (t != null)
                {
                    foreach (Skill s in t.Skills)
                    {
                        foreach (var course in _context.Courses.Include(c => c.Skill))
                        {
                            if (course.Skill.SkillName == s.SkillName)
                            {
                                courses.Add(course);
                            }
                        }
                    }
                }
            }
            else courses = await _context.Courses.Include(c => c.Skill).ToListAsync();
            return courses;
        }
    }
}
