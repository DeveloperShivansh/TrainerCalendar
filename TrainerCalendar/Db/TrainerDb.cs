using Microsoft.EntityFrameworkCore;
using TrainerCalendar.Authentications;
using TrainerCalendar.Contexts;
using TrainerCalendar.Middlewares;
using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;
using TrainerCalendar.Tools;

namespace TrainerCalendar.Db
{
    public interface ITrainerDb
    {
        public Task<ResponseDto> PostTrainer(TrainerDto trainerDto);
        public Trainer GetTrainerById(int Id);
        public Task<List<Trainer>> GetTrainers(int Id = -1, string SkillName = "", int SessionId = -1, int SkillId=-1);
        public Task<object> AddSkillsToTrainer(Trainer t, List<int> SkillIds);
        public Task<ResponseDto> UpdateTrainer(TrainerDto trainerDto, int Id = -1, Trainer? Trainer = null, User? User = null);
        public Task<ResponseDto> DeleteTrainerById(int Id);
    }
    public class TrainerDb : ITrainerDb
    {
        private IJwtAuthenticationManager jwtAuthenticationManager;
        private ApplicationDbContext dbContext;
        private IBasicTools tools;
        public TrainerDb(IJwtAuthenticationManager jwtAuthenticationManager, ApplicationDbContext dbContext, IBasicTools _tools)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.dbContext = dbContext;
            this.tools = _tools;
        }
        public async Task<ResponseDto> PostTrainer(TrainerDto trainerDto)
        {
            if (trainerDto == null || trainerDto.TrainerEmail == null || trainerDto.PhoneNumber == null || trainerDto.TrainerName == null) 
                return new ResponseDto(false, "Please fill the required details");


            //this is the object of user which we will get when the trainer already present in
            //aspnetuser table otherwise this u will be null.
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
                    //create new trainer only if he does not exist in the trainer table.
                    //then the below result.status will return false

                    var result = trainerDto.ValidateCreation();
                    if(!tools.IsValidUserName(trainerDto.TrainerName))
                    {
                        return new ResponseDto(false, "Invalid Username");
                    }
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

        public Trainer? GetTrainerById(int Id)
        {
            return dbContext.Trainers.Include(t=> t.Skills).Include(t=>t.Sessions).FirstOrDefault(t => t.TrainerId == Id);
        }

        public async Task<List<Trainer>> GetTrainers(int Id=-1, string SkillName = "", int SessionId=-1, int SkillId=-1)
        {
            if (Id != -1)
                return await dbContext.Trainers
                    .Where(t => t.TrainerId == Id)
                    .Include(t => t.Sessions)
                    .Include(t => t.Skills)
                    .ToListAsync();

            else if (SkillName != "")
            {
                List<Trainer> res = new List<Trainer>();
                var trainers = dbContext.Trainers
                    .Include(t => t.Skills);
                foreach (var trainer in trainers)
                {
                    foreach(var skill in trainer.Skills)
                    {
                        if(skill.SkillName == SkillName)
                        {
                            res.Add(trainer); ;
                        }
                    }
                }
                return res;
            }


            else if (SkillId != -1)
            {
                List<Trainer> res = new List<Trainer>();
                var trainers = dbContext.Trainers.Include(t => t.Skills);
                foreach(var trainer in trainers)
                {
                    foreach(var skill in trainer.Skills)
                    {
                        if (SkillId == skill.SkillId) res.Add(trainer);
                    }
                }
                return res;
            }

            else if (SessionId != -1)
            {
                List<Trainer> res = new List<Trainer>();
                var trainers = dbContext.Trainers.Include(t => t.Sessions).ToList();
                foreach(var trainer in trainers)
                {
                    foreach(var session in trainer.Sessions)
                    {
                        if(session.SessionId == SessionId) res.Add(trainer);
                    }
                }
                return res;
            }

            else
                return await dbContext.Trainers
                    .Include(t => t.Skills)
                    .Include(t => t.Sessions)
                    .ToListAsync();
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

        public async Task<ResponseDto> UpdateTrainer(TrainerDto trainerDto, int Id = -1, Trainer? Trainer=null, User? User=null)
        {
            Trainer? trainer = null;
            if(User != null) trainer = dbContext.Trainers.Include(x => x.Skills).FirstOrDefault(x => x.TrainerEmail == User.Email);
            else if (Id != -1) trainer = dbContext.Trainers.Include(x => x.Skills).FirstOrDefault(x => x.TrainerId == Id);
            else if (Trainer != null) trainer = Trainer;
            if(trainer == null) return new ResponseDto(false, "No Such Trainer Exist", null);

            ResponseDto responseDto = new ResponseDto();

            if (trainerDto.TrainerEmail != null) trainer.TrainerEmail = trainerDto.TrainerEmail;
            if (trainerDto.TrainerName != null) trainer.TrainerName = trainerDto.TrainerName;
            if(trainerDto.PhoneNumber != null) trainer.PhoneNumber = trainerDto.PhoneNumber;
            if (trainerDto.Skills.Count > 0) await this.AddSkillsToTrainer(trainer, trainerDto.Skills);

            try
            {
                await dbContext.SaveChangesAsync();
                responseDto.Status = true;
                responseDto.Message = "Trainer Updated Successfully";
                responseDto.Data = trainer;
                return responseDto;

            } catch (Exception ex)
            {
                responseDto.Status = false;
                responseDto.Message = ex.Message;
                responseDto.Data = ex.Data;
                return responseDto;
            }
        }

        public async Task<ResponseDto> DeleteTrainerById(int Id)
        {
            ResponseDto responseDto = new ResponseDto();
            List<Trainer> trainers = await this.GetTrainers(Id: Id);
            if(trainers.Count == 0)
            {
                responseDto.Status = false;
                responseDto.Message = "Trainer with this Id does not exists";
                return responseDto;
            }
            dbContext.Remove(trainers[0]);
            await dbContext.SaveChangesAsync();
            responseDto.Status = true;
            responseDto.Message = "Trainer Deleted Successfully";
            return responseDto;
        }
    }
}
