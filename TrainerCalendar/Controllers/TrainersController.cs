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
using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ITrainerDb trainerDb;

        public TrainersController(ApplicationDbContext context, ITrainerDb trainerDb)
        {
            _context = context;
            this.trainerDb = trainerDb;
        }

        // GET: api/Trainers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetTrainers()
        {
            if (_context.Trainers == null)
            {
                return NotFound();
            }
            return await _context.Trainers.Include(x => x.Skills).ThenInclude(x => x.Courses).ToListAsync();
        }

        // GET: api/Trainers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Trainer>> GetTrainer(int id)
        {
          if (_context.Trainers == null)
          {
              return NotFound();
          }
            List<Trainer> trainer = (List<Trainer>) await trainerDb.GetTrainers(Id: id);

            if (trainer.Count == 0)
            {
                return NotFound();
            }

            return trainer[0];
        }
        [Route("GetTrainersBySkillName/")]
        [HttpGet]
        public async Task<ActionResult<List<Trainer>>> GetTrainersBySkillName(string SkillName)
        {
            Console.WriteLine("Your Skill Naame: " + SkillName);
            if (_context.Trainers == null)
            {
                return NotFound();
            }
            List<Trainer> trainer = (List<Trainer>) await trainerDb.GetTrainers(SkillName: SkillName);

            if (trainer.Count == 0)
            {
                return NotFound();
            }

            return trainer;
        }

        // PUT: api/Trainers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrainer(int id, Trainer trainer)
        {
            if (id != trainer.TrainerId)
            {
                return BadRequest();
            }

            _context.Entry(trainer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainerExists(id))
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

        // POST: api/Trainers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<object> PostTrainer(TrainerDto trainerDto)
        {
          if (_context.Trainers == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Trainers'  is null.");
          }
          return await trainerDb.PostTrainer(trainerDto);
        }

        // DELETE: api/Trainers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            if (_context.Trainers == null)
            {
                return NotFound();
            }
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("AddSkill/")]
        [HttpPost]
        public object AddSkill(List<int> SkillIds)
        {
            if (SkillIds.Count == 0) return new
            {
                Status = false,
                Message = "No Skill Id found in the request"
            };

            foreach (int skillId in SkillIds)
            {
                Console.WriteLine("Searhing skill with id: " + skillId.ToString());
                Skill? s = _context.Skills.FirstOrDefault(s => s.SkillId == skillId);
                if (s != null) {
                    Console.WriteLine("Found: " + s.SkillName);
                }
            }
            _context.SaveChanges();
            return new {
                Status = true,
                Message = "Skill updated Successfully"
            };
        }

        private bool TrainerExists(int id)
        {
            return (_context.Trainers?.Any(e => e.TrainerId == id)).GetValueOrDefault();
        }
    }
}
