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
using TrainerCalendar.Middlewares;

namespace TrainerCalendar.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISessionDb _sdb;

        public SessionsController(ApplicationDbContext context , ISessionDb sdb)
        {
            _context = context;
            _sdb = sdb;
        }

        // GET: api/Sessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Session>>> GetSessions()
        {
          if (_context.Sessions == null)
          {
              return NotFound();
          }
            return await _context.Sessions.ToListAsync();
        }

        // GET: api/Sessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Session>> GetSession(int id)
        {
          if (_context.Sessions == null)
          {
              return NotFound();
          }
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return session;
        }
        [Route("GetSessionsBy/")]
        [HttpGet]
        public async Task<ActionResult<Session>> GetSessionBy([FromQuery] SessionByDto? SessionBy)
        {
            var sessions = await _sdb.GetSessions(SessionBy);
            if(sessions != null)
            {
                return Ok(sessions);
            }
            return NoContent();
        }
        // PUT: api/Sessions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSession(int id, SessionDto sessionDto)
        {
            ResponseDto data = await _sdb.UpdateSession(sessionId:id, sessionDto:sessionDto);
            if (data.Status == false) return NotFound(data);
            else return Ok(data);
        }

        // POST: api/Sessions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Session>> PostSession(SessionDto sessionDto)
        {
            if (_context.Sessions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sessions'  is null.");
            }
            Trainer t = CurrentRequest.CurrentUser.GetTrainer(_context);

            if (sessionDto.TrainerId == null)
            {
                if (CurrentRequest.CurrentUser.Role == "Trainer")
                {
                    sessionDto.TrainerId = t.TrainerId;
                }
                else return BadRequest(new ResponseDto(false, "TrainerId is Required To Create New Session"));
            }
            if(sessionDto.SkillId == null)
            {
                var skills = _context.Skills.Include(s => s.Courses).ToList();
                foreach(var skill in skills)
                {
                    foreach(var course in skill.Courses)
                    {
                        if(course.CourseId == sessionDto.CourseId)
                        {
                            sessionDto.SkillId = skill.SkillId;
                            break;
                        }
                    }
                }
            }
            Session? session = sessionDto.GetSession();
            if (session != null)
            {

                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetSession", new { id = session.SessionId }, session);
        }

        // DELETE: api/Sessions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            ResponseDto responseDto = new ResponseDto();
            if (_context.Sessions == null)
            {
                return NotFound();
            }
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            responseDto.Status = true;
            responseDto.Message = "Session Deleted Sucessfully";
            responseDto.Data = session;
            return Ok(responseDto);
        }
    }
}
