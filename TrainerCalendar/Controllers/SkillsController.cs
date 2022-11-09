using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Contexts;
using TrainerCalendar.Models;

namespace TrainerCalendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Skills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            if (_context.Skills == null)
            {
                return NotFound();
            }
            return await _context.Skills.ToListAsync();
        }

        [Route("GetSkillsBy/")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkillsBy([FromQuery] int? courseId = -1)
        {
            List<Skill> res = new List<Skill>();
            if(courseId != -1)
            {
                var skills = await _context.Skills.Include(s => s.Courses).ToListAsync();
                foreach(var skill in skills)
                {
                    foreach(var course in skill.Courses)
                    {
                        if (course.CourseId == courseId)
                        {
                            res.Add(skill);
                            return res;
                        }
                    }
                }
            }
            return Ok(await _context.Skills.ToListAsync());
        }

        // GET: api/Skills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
          if (_context.Skills == null)
          {
              return NotFound();
          }
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            return skill;
        }

        // PUT: api/Skills/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(int id, Skill skill)
        {
            if (id != skill.SkillId)
            {
                return BadRequest();
            }

            _context.Entry(skill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(id))
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

        // POST: api/Skills
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Skill>> PostSkill(Skill skill)
        {
            if (_context.Skills == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Skills'  is null.");
            }
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkill", new { id = skill.SkillId }, skill);
        }

        // DELETE: api/Skills/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            if (_context.Skills == null)
            {
                return NotFound();
            }
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SkillExists(int id)
        {
            return (_context.Skills?.Any(e => e.SkillId == id)).GetValueOrDefault();
        }
    }
}
