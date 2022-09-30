using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainerCalendar.Authentications;
using TrainerCalendar.Contexts;
using TrainerCalendar.Db;
using TrainerCalendar.Middlewares;
using TrainerCalendar.Models;
using TrainerCalendar.Models.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrainerCalendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<User> userManager;
        private ApplicationDbContext dbContext;
        private IJwtAuthenticationManager jwtAuthenticationManager;
        private ITrainerDb trainerDb;
        public AccountController(UserManager<User> userManager, ApplicationDbContext dbContext, IJwtAuthenticationManager jwtAuthenticationManager, ITrainerDb trainerDb)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.trainerDb = trainerDb;
        }
        // Post: api/account/gettoken/
        [Route("gettoken/")]
        [HttpPost]
        public object GetToken(UserDto userDto)
        {
            User? user = jwtAuthenticationManager.Authenticate(userDto);
            if (user != null)
            {
                var result = jwtAuthenticationManager.Generate(user).GetAwaiter().GetResult();
                ResponseDto responseDto = new ResponseDto();
                responseDto.Status = true;
                responseDto.Message = "Token Generation Successfull";
                responseDto.Data = result;
                return responseDto;
            }
            else return new
            {
                Status = false,
                Message = "User with the provided credential not found"
            };
        }


        //For Testing Purpose Only
        [Route("CreateAdmin/")]
        [HttpPost]
        public async Task<object> CreateAdmin(UserDto userDto)
        {
            User u = new User();
            u.Email = userDto.Email;
            u.PhoneNumber = userDto.PhoneNumber;
            u.Role = "Admin";
            u.UserName = userDto.UserName;

            var result = await userManager.CreateAsync(u, userDto.Password);
            if (result.Succeeded) return new
            {
                Status = true,
                Message = "User Created Successfullt",
                Data = u
            };
            else
            {
                return new
                {
                    Status = false,
                    Message = "Failed to create the user",
                    Data = result.Errors.ToList()
                };
            }
        }

        // POST api/account/trainer/
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("CreateTrainer/")]
        [HttpPost]
        public object CreateTrainer(TrainerDto? trainerDto)
        {
            return trainerDb.PostTrainer(trainerDto);
        }

        [Route("SetTrainerPassword/")]
        [HttpPost]
        public async Task<object> SetTrainerPassword(TrainerDto trainerDto)
        {
            ResponseDto responseDto = new ResponseDto();
            if(trainerDto.ValidateForSettingPassword())
            {
                Trainer? t = null;
                if(trainerDto.TrainerEmail != null ) t = dbContext.Trainers.FirstOrDefault(t => t.TrainerEmail == trainerDto.TrainerEmail);
                else if(trainerDto.PhoneNumber != null) t= dbContext.Trainers.FirstOrDefault(t=>t.PhoneNumber == trainerDto.PhoneNumber);

                if(t == null)
                {
                    responseDto.Status = false;
                    responseDto.Message = "Trainer Does Not Exists";
                    return responseDto;
                } 
                else
                {
                    User user = new User();
                    user.Email = t.TrainerEmail;
                    user.PhoneNumber = t.PhoneNumber;
                    user.Role = "Trainer";
                    user.UserName = t.TrainerName;

                    //Here Send OTP On trainer Phone

                    var result = await userManager.CreateAsync(user, trainerDto.Password);
                    if(result.Succeeded)
                    {
                        responseDto.Status=true;
                        responseDto.Message = "Password Set Successfull";
                        responseDto.Data = t;
                        return responseDto;
                    } else
                    {
                        responseDto.Status = false;
                        responseDto.Message = "Failed to Set Password";
                        responseDto.Data = result.Errors.ToList();
                        return responseDto;
                    }
                }
            }
            else
            {
                responseDto.Status = false;
                responseDto.Message = "Failed Set Password";
                responseDto.Data = "Phone or Email and Password Field is Required";
                return responseDto;
            }
        }

        // PUT api/<AccountController>/5
        [Route("testauth/")]
        [HttpGet]
        public object Get()
        {
            return new
            {
                status = true,
                message = "authorization successfull"
            };
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
