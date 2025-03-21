using DBaseApi.Features.Auth.Dto;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BaseApi.WebApi.Features.Auth
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authenticationService;

        public AuthController(AuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public IActionResult Auth([FromBody] UserDto User)
        {
            try
            {
                var user = _authenticationService.Auth(User);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
