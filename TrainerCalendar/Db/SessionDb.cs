using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Contexts;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Db
{
    public interface ISessionDb
    {
        public Task<object> GetSession(int SkillID = -1, int? courseid = -1, int? trainerid = -1);
        


    }
    public class SessionDb : ISessionDb
    {

        private readonly ApplicationDbContext _dbContext;
        public SessionDb(ApplicationDbContext dbContext)
        {

            _dbContext = dbContext;
        }
        public async Task<object> GetSession(int SkillID, int? courseid, int? trainerId)
        {
            ResponseDto responseDto = new ResponseDto();
            if(SkillID != 0 && courseid !=0 && trainerId!=0)
            {
                var result = _dbContext.Sessions.Where(s => s.SkillId == SkillID && s.CouresId == courseid && s.TrainerId == trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }

            }
            else if (SkillID != 0 && courseid == 0 && trainerId == 0)
            {
                var result = _dbContext.Sessions.Where(s => s.SkillId == SkillID);

                if (result != null)
                {

                    return await result.ToListAsync();

                }
            }
            else if (SkillID == 0 && courseid != 0 && trainerId == 0)
            {

                var result = _dbContext.Sessions.Where(s => s.CouresId == courseid);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (SkillID == 0 && courseid == 0 && trainerId != 0)
            {

                var result = _dbContext.Sessions.Where(s => s.TrainerId == trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (SkillID != 0 && courseid != 0 && trainerId == 0)
            {

                var result = _dbContext.Sessions.Where(s => s.CouresId == courseid && s.SkillId == SkillID);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (SkillID == 0 && courseid != 0 && trainerId != 0)
            {

                var result = _dbContext.Sessions.Where(s => s.CouresId == courseid && s.TrainerId==trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else if (SkillID != 0 && courseid == 0 && trainerId != 0)
            {

                var result = _dbContext.Sessions.Where(s => s.SkillId == SkillID && s.TrainerId ==trainerId);

                if (result != null)
                {

                    return await result.ToListAsync();

                }


            }
            else
            {
                responseDto.Status = false;
                responseDto.Message = "No data Found";
                return responseDto;
            }

            responseDto.Status = false;
            responseDto.Message = "No data Found";
            return responseDto;

        }
      

    }
}
