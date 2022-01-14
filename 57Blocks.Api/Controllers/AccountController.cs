using _57Blocks.Api.Helpers;
using _57Blocks.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace _57Blocks.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApiContext _apiContext;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string EmailRegex = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        private readonly string PasswordRegex = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@]).{10,}$";

        public AccountController(ILogger<AccountController> logger, ApiContext context, IConfiguration configuration)
        {
            _apiContext = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userModel)
        {
            try
            {
                //Validate email is a valid email address
                if (!Regex.IsMatch(userModel.Email, EmailRegex))
                    return BadRequest("The email isn't correct formate");

                //Validate password formate
                if (!Regex.IsMatch(userModel.Password, PasswordRegex))
                    return BadRequest("The password isn't correct formate");

                //Validate email is not already registered in the database
                var validate = await _apiContext.Users.AsNoTracking().Where(x => x.Email == userModel.Email).FirstOrDefaultAsync();

                if (validate != null)
                    return BadRequest("Email used by another user.");


                //Insert user
                Users user = new Users();

                user.ID = Guid.NewGuid();
                user.Email = userModel.Email;
                user.Password = userModel.Password;
                user.Name = userModel.Name;
                user.LastName = userModel.LastName;

                await _apiContext.Users.AddAsync(user);
                await _apiContext.SaveChangesAsync();

                return Created("", user);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate(UserValidateDTO userModel)
        {
            //Validate email is a valid email address
            if (!Regex.IsMatch(userModel.Email, EmailRegex))
                return BadRequest("The email isn't correct formate");

            //Validate password formate
            if (!Regex.IsMatch(userModel.Password, PasswordRegex))
                return BadRequest("The password isn't correct formate");

            //Validate email is not already registered in the database
            var validate = await _apiContext.Users.AsNoTracking().Where(x => x.Email == userModel.Email && x.Password == userModel.Password).FirstOrDefaultAsync();

            if (validate == null)
                return BadRequest("Email or password incorrect");

            //Generate token with 20 minutes life
            // read secret_key 
            var secretKey = _configuration.GetValue<string>("SecretKey");
            var key = Encoding.ASCII.GetBytes(secretKey);

            ClaimsIdentity claims = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Email, userModel.Email)
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(createdToken);

            //Create object result
            UserResultDTO userResult = new UserResultDTO();
            userResult.Email = userModel.Email;
            userResult.Token = token;

            return Ok(userResult);
        }


    }
}
