using IdentifyFrameworkInWebAPI.Models;
using IdentifyFrameworkInWebAPI.Models.Authentication.SignUp;
using IdentityFrameworkInWebAPI.Service.Models;
using IdentityFrameworkInWebAPI.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace IdentifyFrameworkInWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly IEmailService _emailService;


        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,IEmailService emailService
            )
        {
            this._userManager = userManager;
            this._roleManger = roleManager;

            this._emailService = emailService;
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {

            //check for user existance
            var userExist = await _userManager.FindByNameAsync(registerUser.email);

            if ((userExist != null))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response
                    {
                        Status = "Error",
                        Message = "User already exists !"
                    });
            }


            //add user in database
            IdentityUser user = new()
            {
                UserName = registerUser.userName,
                Email = registerUser.email,
                SecurityStamp = Guid.NewGuid().ToString(),

            };

            if (await _roleManger.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.password);

                if (result.Succeeded)
                {


                    await _userManager.AddToRoleAsync(user, role);

                    //Add token to verify the email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationalLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
                    var message = new Message(new String[] { user.Email }, "Confirmation email link", confirmationalLink);
                    _emailService.SendEmail(message);



                    return StatusCode(StatusCodes.Status201Created,
                               new Response
                               {
                                   Status = "Success",
                                   Message =$"User created and Emailt sent to{user.Email} successfully !"
                               });
                }
                else
                {

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(" ", error.Description);
                    }
                    //return StatusCode(StatusCodes.Status500InternalServerError,
                    //        new Response
                    //        {
                    //            Status = "Error",
                    //            Message = "User creation failed !"
                    //        });

                    return BadRequest(result.Errors);
                }
            }
            else {

                return StatusCode(StatusCodes.Status500InternalServerError,
                      new Response
                      {
                          Status = "",
                          Message = "Role does not exists!"
                      });


            }

        }



        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email) { 
        
        var user= await _userManager.FindByEmailAsync(email);
            if (user != null) { 
                var result= await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "sucess", Message = "Email verified successfully " });
                }

            
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "This user does not exists !" });

        }



        //[HttpGet]
        //public async Task<IActionResult> TestEmail() {

        //    var message = new Message(new string[] {
        //   "jeet01.ujgare@gmail.com"
        //    }, "Test", "<h1>Please verify email for Identity framework application</h1>");
           

        //   _emailService.SendEmail(message);
        //    return StatusCode(StatusCodes.Status200OK,
        //        new Response
        //        {
        //            Status = "Success",
        //            Message = "Email sent successfully !"
        //        }); 
        //}
    }
}

