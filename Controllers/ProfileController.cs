using be_magang.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using be_magang.Models;
using Microsoft.AspNetCore.Authorization;

namespace be_magang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetProfile()
        {
            var userId = int.Parse(User.Identity.Name);
            var user = await _profileService.GetProfile(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<User>> UpdateProfile([FromBody] User model)
        {
            var userId = int.Parse(User.Identity.Name);
            var user = await _profileService.UpdateProfile(userId, model.Name, model.Email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
