using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using TestStore.Models;

namespace TestStore.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        ApplicationContext db;
        public UserController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet("/api/getusers")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<User> users = await db.Users.ToListAsync();

            if (users != null)
            {
                return Ok(users);
            }

            return BadRequest(new { errorText = "Нет ни одного пользователя" });
        }

        [HttpGet("/api/getuser/{userid}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int userId)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest(new { errorText = "Пользователь не найден" });
        }

        [HttpPost("/api/newuser/")]
        public async Task<IActionResult> NewUser([FromBody] User user)
        {
            if (user != null)
            {
                db.Users.Add(user);

                await db.SaveChangesAsync();

                return Ok(user);
            }

            return BadRequest(new { errorText = "Некорректные исходные данные" });
        }

        [HttpPost("/api/token")]
        public async Task<IActionResult> Token([FromBody] User user)
        {
            var login = user.Login;
            var password = user.Password;

            User dbUser = await db.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

            AuthOptions tokenAuthorization = new AuthOptions();

            var identity = tokenAuthorization.GetIdentity(dbUser);

            if (identity == null)
            {
                return BadRequest(new { errorText = "Неверное имя пользователя или пароль." });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var accessToken = encodedJwt;

            return Ok(accessToken);
        }

        [HttpPut("/api/updateuser/")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { errorText = "Некорректные данные." });
            }
            if (!db.Users.Any(u => u.UserId == user.UserId))
            {
                return NotFound(new { errorText = "Пользователь не найден." });
            }

            db.Update(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("/api/deleteuser/{userid}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            User user = db.Users.FirstOrDefault(u => u.UserId == userId);
           
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден." });
            }
           
            db.Users.Remove(user);
            await db.SaveChangesAsync();
          
            return Ok(user);
        }
    }
}
