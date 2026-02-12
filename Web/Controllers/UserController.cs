using Application.Services;
using AutoMapper;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.ViewModels;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(UserService us, IMapper m) : ControllerBase
    {
        private readonly UserService _userService = us;
        private readonly IMapper _mapper = m;

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]CreateAuthUserVM user)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userCreated = await _userService.CreateAuthUser(user.Pseudo, user.Mail, user.Password);

            return Ok(_mapper.Map<AuthUserDetailVM>(userCreated));
        }

        [HttpGet]
        public async Task<IActionResult> ValidatePseudo(string pseudo)
        {
            var isAvailable = await _userService.IsPseudoAvailable(pseudo);

            return Ok(isAvailable);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateMail(string mail)
        {
            var isAvailable = await _userService.IsMailAvailable(mail);
            return Ok(isAvailable);
        }
    }
    public class CreateAuthUserVM
    {
        [Required]
        public string Mail { get; set; }

        [Required]
        [MinLength(3)]
        public string Pseudo { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
