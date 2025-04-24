using IdentifyFrameworkInWebAPI.Models;
using IdentifyFrameworkInWebAPI.Models.Authentication.Login;
using IdentifyFrameworkInWebAPI.Models.Authentication.SignUp;
using IdentityFrameworkInWebAPI.Service.Models;
using IdentityFrameworkInWebAPI.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace IdentifyFrameworkInWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;

        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IConfiguration configuration

            )
        {
            this._userManager = userManager;
            this._roleManger = roleManager;
            this._configuration = configuration;
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
                    var message = new Message(new String[] { user.Email }, "Confirmation of Email","Verify By Cliking link : "+confirmationalLink);
                    _emailService.SendEmail(message);



                    return StatusCode(StatusCodes.Status201Created,
                               new Response
                               {
                                   Status = "Success",
                                   Message = $"User created and Emailt sent to{user.Email} successfully !"
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
            else
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                      new Response
                      {
                          Status = "",
                          Message = "Role does not exists!"
                      });


            }

        }



        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "sucess", Message = "Email verified successfully " });
                }


            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "This user does not exists !" });

        }



        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {

            //Checking the user
            var user = await _userManager.FindByNameAsync(loginModel.Username);


            //checking password
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {

                //claimlist creation
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };


                //We add roles to the claim list
                var usrRoles = await _userManager.GetRolesAsync(user);

                if (usrRoles != null)
                {
                    foreach (var role in usrRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }


                //generate the token with the class
                var jwtToken = GetToken(authClaims);


                //returning the token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo,
                });
            }





            return Unauthorized();

        }

        //create method to generat token 
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)



                );

            return token;

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

