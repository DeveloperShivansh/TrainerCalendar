using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetToken(UserDto userDto, [FromQuery] string? view = "")
        {
            User? user = jwtAuthenticationManager.Authenticate(userDto);

            ResponseDto responseDto = new ResponseDto();
            if (user != null)
            {
                if (view == "") return BadRequest(new ResponseDto(false, "view is required in query string"));
                else if (user.Role == "Trainer" && view != "trainer") return Unauthorized(new ResponseDto(false, "You cannot login as a trainer from admin page."));
                else if (user.Role == "Admin" && view != "admin") return Unauthorized(new ResponseDto(false, "You cannot loging as admin from trainer page"));

                var result = jwtAuthenticationManager.Generate(user).GetAwaiter().GetResult();
                
                responseDto.Status = true;
                responseDto.Message = "Token Generation Successfull";
                responseDto.Data = result;
                return Ok(responseDto);
            }
            else
            {
                responseDto.Status=false;
                responseDto.Message = "User with the provided credential not found";
                responseDto.Data = null;
                return NotFound(responseDto);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("IsValidToken/")]
        [HttpGet]
        public async Task<IActionResult> IsValidToken()
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                return Ok(
                    new ResponseDto(true, "Token Is Valid", new { 
                        Role = "Admin", 
                        Admin = new {
                            Name = CurrentRequest.CurrentUser.UserName,
                            Email = CurrentRequest.CurrentUser.Email,
                            PhoneNumber = CurrentRequest.CurrentUser.PhoneNumber
                        } 
                    }));
            }
            else
            {
                Trainer? t = CurrentRequest.CurrentUser.GetTrainer(dbContext);
                return Ok(new ResponseDto(true, "Token Is Valid", new { Role = "Trainer", Trainer = t }));
            }
        }

        [Route("IsTrainerPasswordSet/")]
        [HttpPost]
        public async Task<IActionResult> IsTrainerPasswordSet(TrainerDto? trainerDto)
        {
            string trainerEmail = trainerDto.TrainerEmail;
            string phoneNumber = trainerDto.PhoneNumber;

            User? user = null;
            try
            {
                if (trainerEmail != null) user = await userManager.FindByEmailAsync(trainerEmail);
                else if (phoneNumber != null) user = userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

                if (user != null) return Ok(new ResponseDto(true, "Trainer Already Authenticated", dbContext.Trainers.Include(t=>t.Skills).Include(t=>t.Sessions).FirstOrDefault(t => t.TrainerEmail == user.Email)));
                else
                {
                    Trainer? t = null;
                    if (trainerEmail != null) t = dbContext.Trainers.FirstOrDefault(t => t.TrainerEmail == trainerEmail);
                    else if (phoneNumber != null) t = dbContext.Trainers.FirstOrDefault(t => t.PhoneNumber == phoneNumber);
                    if (t != null) return Ok(new ResponseDto(false, "PasswordNotSet", t));
                    else return Ok(new ResponseDto(false, "Invalid Email or Phone"));
                }
            }
            catch (Exception ex) { return BadRequest(new ResponseDto(false, ex.Message)); }
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("GetUser/")]
        [HttpGet]
        public async Task<object> GetUser()
        {
            if (CurrentRequest.CurrentUser.Role == "Admin")
            {
                return new { 
                    User = CurrentRequest.CurrentUser.User,
                    IsTrainer = false,
                    IsAdmin = true
                };
            } 
            else
            {
                return new {
                    User = CurrentRequest.CurrentUser.GetTrainer(dbContext),
                    IsTrainer = true,
                    IsAdmin = false
                };
            }
        }

        // POST api/account/trainer/
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("CreateTrainer/")]
        [HttpPost]

        public async Task<object> CreateTrainer(TrainerDto? trainerDto)
        {
            return await trainerDb.PostTrainer(trainerDto);
        }

        [Route("SetTrainerPassword/")]
        [HttpPost]
        public async Task<IActionResult> SetTrainerPassword(TrainerDto? trainerDto)
        {
            ResponseDto responseDto = new ResponseDto();
            if(trainerDto.ValidateForSettingPassword())
            {
                Trainer? t = null;
                if(trainerDto.TrainerEmail != null ) t = dbContext.Trainers.FirstOrDefault(t => t.TrainerEmail == trainerDto.TrainerEmail);
                else if(trainerDto.PhoneNumber != null) t = dbContext.Trainers.FirstOrDefault(t=>t.PhoneNumber == trainerDto.PhoneNumber);

                //if trainer is logging in before admin created his/her account.
                if(t == null)
                {
                    responseDto.Status = false;
                    responseDto.Message = "Trainer not found, request admin to create one";
                    return NotFound(responseDto);
                } 
                else
                {
                    User user = new User();
                    user.Id = t.TrainerId.ToString();
                    user.Email = t.TrainerEmail;
                    user.PhoneNumber = t.PhoneNumber;
                    user.Role = "Trainer";
                    user.UserName = t.TrainerName;


                    //send otp on phone and remove the below code then we have to take the below 
                    //code in the next method where we will check if the otp is valid only then
                    //we create the user. but for now it is creating without check.

                    var result = await userManager.CreateAsync(user, trainerDto.Password);

                    if(result.Succeeded)
                    {
                        //adding foreign key in the trainer table
                        t.User = user;
                        await dbContext.SaveChangesAsync();
                        responseDto.Status=true;
                        responseDto.Message = "Password Reset Successfull";
                        responseDto.Data = t;
                        return Ok(responseDto);
                    } 
                    else
                    {
                        responseDto.Status = false;
                        responseDto.Message = "Failed to Set Password";
                        responseDto.Data = result.Errors.ToList();
                        return BadRequest(responseDto);
                    }
                }
            }
            else
            {
                responseDto.Status = false;
                responseDto.Message = "Failed Set Password";
                responseDto.Data = "Phone or Email and Password Field is Required";
                return BadRequest(responseDto);
            }
        }



        // DELETE api/<AccountController>/5
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(int id)
        {
            User u = CurrentRequest.CurrentUser.User;
            ResponseDto responseDto = new ResponseDto();
            if(u.Role == "Trainer")
            {
                responseDto.Status = false;
                responseDto.Message = "Only Admin Can Delete Trainers";
                responseDto.Data = null;
                return Unauthorized(responseDto);
            } 
            else
            {
                responseDto = await trainerDb.DeleteTrainerById(id);
                if (responseDto.Status == false) return NotFound(responseDto);
                else return Ok(responseDto);
            }
            
        }
    }
}
