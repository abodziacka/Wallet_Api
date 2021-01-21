using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly AuthenticationContext _context;


        public UserController(AuthenticationContext context, UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _context = context;
        }

        [HttpPost]
        [Route("registration")]
        //POST: /user/registration

        public async Task<Object> PostUser([FromBody] UserModel model)
        {
            var user = new User()
            {
                UserName = model.Username,
                Email = model.Email
            };

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                var createUser = await _userManager.FindByEmailAsync(model.Email);
                Category categoryFood = new Category();

                categoryFood.Name = "Żywność";
                categoryFood.Description = "Produkty spożywcze";
                categoryFood.UserId = createUser.Id;
                _context.Categories.Add(categoryFood);

                Category categoryClothes = new Category();
                categoryClothes.Name = "Odzież";
                categoryClothes.Description = "Ubrania";
                categoryClothes.UserId = createUser.Id;
                _context.Categories.Add(categoryClothes);

                Category categoryExpenses = new Category();
                categoryExpenses.Name = "Wydatki";
                categoryExpenses.Description = "Rachunki bieżące";
                categoryExpenses.UserId = createUser.Id;
                _context.Categories.Add(categoryExpenses);
                _context.SaveChanges();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("login")]
        //POST: /user/login

        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });

            }
            else
            {
                return BadRequest(new { message = "Username or password incorect." });
            }
        }


    }

    

}
