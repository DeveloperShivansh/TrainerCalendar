using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Authentications;
using TrainerCalendar.Contexts;
using TrainerCalendar.Middlewares;
using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Db
{
    public interface ITrainerDb
    {
        public Task<object> PostTrainer(TrainerDto trainerDto);
        public Task<object> GetTrainers(int Id = -1, string SkillName = "", int SessionId = -1, int SkillId=-1);
        public Task<object> AddSkillsToTrainer(Trainer t, List<int> SkillIds);
    }
    public class TrainerDb : ITrainerDb
    {
        private IJwtAuthenticationManager jwtAuthenticationManager;
        private ApplicationDbContext dbContext;
        public TrainerDb(IJwtAuthenticationManager jwtAuthenticationManager, ApplicationDbContext dbContext)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.dbContext = dbContext;
        }
        public async Task<object> PostTrainer(TrainerDto trainerDto)
        {
            if (trainerDto == null) return new
            {
                Status = false,
                Message = "No data found in request"
            };

            User? u = jwtAuthenticationManager.Authenticate(trainerDto);

            ResponseDto responseDto = new ResponseDto();
            if (CurrentRequest.CurrentUser != null && CurrentRequest.CurrentUser.Role == "Admin")
            {
                if (u != null)
                {
                    responseDto.Status = false;
                    responseDto.Message = "Trainer with the given details already exists";
                    return responseDto;
                }
                else
                {
                    var result = trainerDto.ValidateCreation();
                    if (result.Status == true)
                    {
                        Trainer? t = new Trainer();
                        t.TrainerEmail = trainerDto.TrainerEmail;
                        t.TrainerName = trainerDto.TrainerName;
                        t.PhoneNumber = trainerDto.PhoneNumber;
                        foreach (int skillId in trainerDto.Skills)
                        {
                            Skill? s = dbContext.Skills.FirstOrDefault(x => x.SkillId == skillId);
                            if (s != null) t.Skills.Add(s);
                        }
                        await dbContext.Trainers.AddAsync(t);
                        await dbContext.SaveChangesAsync();

                        responseDto.Status = true;
                        responseDto.Message = "Trainer Created Successfully";
                        responseDto.Data = t;
                        return responseDto;
                    }
                    else return result;
                }
            }
            else
            {
                responseDto.Status = false;
                responseDto.Message = "Only Admins Can Post Trainers";
                return responseDto;
            }
        }

        public async Task<object> GetTrainers(int Id=-1, string SkillName = "", int SessionId=-1, int SkillId=-1)
        {
            if (Id != -1)
                return await dbContext.Trainers
                    .Where(t => t.TrainerId == Id)
                    .Include(t => t.Sessions)
                    .Include(t => t.Skills)
                    .ToListAsync();

            else if (SkillName != "")
            {
                //code to get trainer by skill name
            }


            else if (SkillId != -1)
            {
                //code to get trainer by skill id
            }

            else if (SessionId != -1)
            {
                //code to get trainer by session id
            }

            else
                return await dbContext.Trainers
                    .Include(t => t.Skills)
                    .Include(t => t.Sessions)
                    .ToListAsync();
            return null;
        }

        public async Task<object> AddSkillsToTrainer(Trainer t, List<int> SkillIds)
        {
            if (SkillIds.Count == 0) return new
            {
                Status = false,
                Message = "No Skills Found In Request"
            };

            List<Skill> Skills = await dbContext.Skills.Where(s => SkillIds.Contains(s.SkillId)).ToListAsync();
            foreach(Skill skill in Skills)
            {
                t.Skills.Add(skill);
            }

            await dbContext.SaveChangesAsync();

            return new {
                Status = true,
                Message = "Skills added to trainer Successfully"
            };
        }
    }
}
