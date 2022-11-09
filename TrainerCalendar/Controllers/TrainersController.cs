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
    // the role can be also be specified in this attribute but since i am returning the reason along with
    // the response that's why manual.
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

        [Route("current/")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentTrainer()
        {
            CurrentRequest.CurrentUser.Print();
            Trainer? t = _context.Trainers.Include(t => t.Sessions).Include(t => t.Skills).FirstOrDefault(t => t.TrainerEmail == CurrentRequest.CurrentUser.Email);
            if (t != null) return Ok(t);
            else return NotFound(new ResponseDto(false, "Current User is not trainer"));
        }

        // GET: api/Trainers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetTrainers()
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (_context.Trainers == null)
                {
                    return NotFound();
                }
                return await _context.Trainers.Include(x => x.Skills).ThenInclude(x => x.Courses).ToListAsync();
            } 

            else
            {
                return Unauthorized(new ResponseDto(false, "Only Admin Can Get All Trainers"));
            }
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

            return Ok(trainer[0]);
        }
        [Route("GetTrainersBySkillName/")]
        [HttpGet]
        public async Task<ActionResult<List<Trainer>>> GetTrainersBySkillName(string SkillName)
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (_context.Trainers == null)
                {
                    return NotFound();
                }
                List<Trainer> trainer = (List<Trainer>)await trainerDb.GetTrainers(SkillName: SkillName);

                if (trainer.Count == 0)
                {
                    return NotFound();
                }

                return Ok(trainer);
            } else return Unauthorized(new ResponseDto(false, "Only Admin Can Get All Trainers"));
        }

        // PUT: api/Trainers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrainer(int id, TrainerDto? trainerDto)
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (id != trainerDto.TrainerId)
                {
                    trainerDto.TrainerId = id;
                }

                ResponseDto response = await trainerDb.UpdateTrainer(trainerDto, Id: id);
                return Ok(response);
            }
            else return Unauthorized(new ResponseDto(false, "Only Admin Can Update Trainer Profile"));
        }

        [HttpPost]
        public async Task<IActionResult> PostTrainer(TrainerDto? trainerDto)
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (_context.Trainers == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Trainers'  is null.");
                }

                ResponseDto responseDto = await trainerDb.PostTrainer(trainerDto);

                if(responseDto.Status == false) return BadRequest(responseDto);

                else return CreatedAtAction("PostTrainer", responseDto);
            }
            else return Unauthorized(new ResponseDto(false, "Only Admin Can Create Trainers"));
        }

        // DELETE: api/Trainers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
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
            else return Unauthorized(new ResponseDto(false, "Only Admin Can Delete Trainers"));
        }

        [Route("AddSkills/")]
        [HttpPost]
        public async Task<object> AddSkills(TrainerDto trainerDto)
        {
            List<int> SkillIds = trainerDto.Skills;
            if (SkillIds.Count == 0) return new
            {
                Status = false,
                Message = "No Skill Id found in the request"
            };
            Trainer? t = null;
            if (trainerDto.TrainerId != -1) t = trainerDb.GetTrainerById(trainerDto.TrainerId);
            if(t == null) return new ResponseDto(false, "Trainer with the given id not exists", null);

            foreach (int skillId in SkillIds)
            {
                Skill? s = _context.Skills.FirstOrDefault(s => s.SkillId == skillId);
                if (s != null)
                {
                    if (t.Skills.FindIndex(s => s.SkillId == skillId) < 0) t.Skills.Add(s);
                }
                else continue;
            }

            await _context.SaveChangesAsync();
            return new {
                Status = true,
                Message = "Skill updated Successfully"
            };
        }
    }
}
