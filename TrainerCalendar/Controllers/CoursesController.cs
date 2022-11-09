using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Contexts;
using TrainerCalendar.Db;
using TrainerCalendar.Middlewares;
using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICourseDb courseDb;

        public CoursesController(ApplicationDbContext context, ICourseDb courseDb)
        {
            _context = context;
            this.courseDb = courseDb;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            if (_context.Courses == null)
            {
                return NotFound();
            }
            return await _context.Courses.ToListAsync();
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
          if (_context.Courses == null)
          {
              return NotFound();
          }
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        [Route("GetCoursesBy/")]
        [HttpGet]
        public async Task<IActionResult> GetCoursesBy([FromQuery] int? trainerId=-1, string? skillName="")
        {
            List<Course> courses = new List<Course>();
            if (_context.Courses == null) return NotFound(new ResponseDto(false, "Course not exists"));
            try
            {
                //if the user is admin
                if (CurrentRequest.CurrentUser.Role == "Admin")
                {
                    if (trainerId != -1)
                    {
                        courses = await courseDb.GetCourses(trainerId: trainerId);
                        return Ok(courses);
                    }
                    else if (skillName != "")
                    {
                        courses = await courseDb.GetCourses(skillName: skillName);
                        return Ok(courses);
                    }
                    else return Ok(await courseDb.GetCourses());
                }
                //if the user is trainer
                else
                {
                    if (trainerId != -1 || skillName != "") return Unauthorized(new ResponseDto(false, "Only Admins Can Get Courses By TrainerId and SkillName"));
                    
                    Trainer t = CurrentRequest.CurrentUser.GetTrainer(_context);

                    if(t.User == null)
                    {
                        return Unauthorized(new ResponseDto(false, "You have to reset your password for getting access to features"));
                    }

                    if (t != null)
                    {
                        courses = await courseDb.GetCourses(trainerId: t.TrainerId);
                        return Ok(courses);
                    }
                    else return BadRequest(new ResponseDto(false, "No Such Trainer Exist, May be you are not logged in"));
                }
                return BadRequest(new ResponseDto(false, "Something Wrong Happened, May be You are not logged in"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto(false, ex.Message));
            }

        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (id != course.CourseId)
                {
                    return BadRequest();
                }

                _context.Entry(course).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            else return Unauthorized(new ResponseDto(false, "Only Admin can Update Courses"));
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (_context.Courses == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Courses'  is null.");
                }
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
            }
            else return Unauthorized(new ResponseDto(false, "Only Admin can create new courses"));
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (_context.Courses == null)
                {
                    return NotFound();
                }
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else return Unauthorized(new ResponseDto(false, "Only Admin can delete courses"));
        }

        private bool CourseExists(int id)
        {
            return (_context.Courses?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}
