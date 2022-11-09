using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Contexts;
using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Db
{
    public interface ISessionDb
    {
        public Task<List<Session>> GetSessions(SessionByDto? SessionBy = null);
        public Task<List<Session>> GetSessions(int courseId=-1,int trainerId=-1, int skillId=-1, int sessionId=-1);

        public Task<ResponseDto> UpdateSession(int sessionId = -1, SessionDto? sessionDto=null);
    }
    public class SessionDb : ISessionDb
    {

        private readonly ApplicationDbContext _dbContext;
        public SessionDb(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Session>> GetSessions(SessionByDto? SessionBy)
        {
            int? skillId = SessionBy.SkillId;
            int? courseId = SessionBy.CourseId;
            int? trainerId = SessionBy.TrainerId;
            int? sessionId = SessionBy.SessionId;

            if(skillId != -1 && courseId != -1 && trainerId!= -1)
            {
                var result = _dbContext.Sessions.Where(s => s.SkillId == skillId && s.CourseId == courseId && s.TrainerId == trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }

            }
            else if (skillId != -1 && courseId == -1 && trainerId == -1)
            {
                var result = _dbContext.Sessions.Where(s => s.SkillId == skillId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }
            }
            else if (skillId == -1 && courseId != -1 && trainerId == -1)
            {

                var result = _dbContext.Sessions.Where(s => s.CourseId == courseId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (skillId == -1 && courseId == -1 && trainerId != -1)
            {

                var result = _dbContext.Sessions.Where(s => s.TrainerId == trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (skillId != -1 && courseId != -1 && trainerId == -1)
            {

                var result = _dbContext.Sessions.Where(s => s.CourseId == courseId && s.SkillId == skillId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (skillId == -1 && courseId != -1 && trainerId != -1)
            {

                var result = _dbContext.Sessions.Where(s => s.CourseId == courseId && s.TrainerId==trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (skillId != -1 && courseId == -1 && trainerId != -1)
            {

                var result = _dbContext.Sessions.Where(s => s.SkillId == skillId && s.TrainerId ==trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (sessionId != -1)
            {
                List<Session> sessions = new List<Session>();
                Session? session = _dbContext.Sessions.FirstOrDefault(s => s.SessionId == sessionId);
                if (session != null)
                {
                    sessions.Add(session);
                    return sessions;
                }
            }
            else if(SessionBy != null)
            {
                var result = _dbContext.Sessions.Where(s => s.StartTime >= SessionBy.StartTime && s.EndTime <= SessionBy.EndTime);

                if(result != null)
                {
                    return await result.ToListAsync();
                }
            }

            return null;
        }

        public async Task<List<Session>> GetSessions(int courseId = -1, int trainerId = -1, int skillId = -1, int sessionId = -1)
        {
            SessionByDto sessionByDto = new SessionByDto();
            sessionByDto.CourseId = courseId;
            sessionByDto.TrainerId = trainerId;
            sessionByDto.SkillId = skillId;
            sessionByDto.SessionId = sessionId;
            var result = await GetSessions(sessionByDto);
            return result;
        }

        public async Task<ResponseDto> UpdateSession(int sessionId, SessionDto sessionDto)
        {
            ResponseDto responseDto = new ResponseDto();
            if(sessionId != -1)
            {
                Session? session = _dbContext.Sessions
                    .Include(s => s.Course)
                    .Include(s => s.Trainer)
                    .Include(s => s.Skill)
                    .FirstOrDefault(s => s.SessionId == sessionId);

                if(session != null)
                {
                    if(sessionDto.SessionName != null) session.SessionName = sessionDto.SessionName;
                    if(sessionDto.TrainerId != null) session.TrainerId = sessionDto.TrainerId;
                    if(sessionDto.CourseId != null) session.CourseId = sessionDto.CourseId;
                    if(sessionDto.SkillId != null) session.SkillId = sessionDto.SkillId;
                    if(sessionDto.TrainingMode != null) session.TrainingMode = sessionDto.TrainingMode;
                    if(sessionDto.TrainingLocation != null) session.TrainingLocation = sessionDto.TrainingLocation;
                    if(sessionDto.StartTime != new DateTime(day: 1, month: 1, year: 1)) session.StartTime = sessionDto.StartTime;
                    if(sessionDto.EndTime != new DateTime(day: 1, month: 1, year: 1)) session.EndTime = sessionDto.EndTime;
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                        responseDto.Status = true;
                        responseDto.Message = "Session Updated Successfully";
                        responseDto.Data = session;
                        return responseDto;
                    }
                    catch (Exception ex)
                    {
                        responseDto.Status = false;
                        responseDto.Message = ex.Message;
                        responseDto.Data = ex.StackTrace;
                        return responseDto;
                    }
                }
            }
            responseDto.Status = false;
            responseDto.Message = "sessionId is required";
            return responseDto;
        }
    }
}
