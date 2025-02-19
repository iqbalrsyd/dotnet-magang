using be_magang.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using be_magang.Models;

namespace be_magang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLogin model)
        {
            var token = await _authService.Authenticate(model); // Mengirimkan objek model
            if (token == null)
                return Unauthorized("Invalid email or password.");

            return Ok(new { Token = token });
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody]UserRegister model)
        {
            var user = await _authService.Register(model);
            return Ok(user);
        }
    }
}
