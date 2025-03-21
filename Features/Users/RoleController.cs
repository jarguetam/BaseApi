using System;
using BaseApi.WebApi.Features.Users.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.WebApi.Features.Users
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class RoleController : ControllerBase
    {
        private RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }


        [HttpGet("ActiveOnly")]
        public IActionResult GetActiveOnly()
        {
            try
            {
                var roles = _roleService.GetActiveOnly();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet]
        public IActionResult Get() {
            try
            {
                var roles = _roleService.Get();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

 
        [HttpGet("RoleWithDetail/{RoleId}")]
        public IActionResult RoleWithDetail(int RoleId)
        {
            try
            {
                var roles = _roleService.RoleWithDetail(RoleId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] RoleDto Role)
        {
            try
            {
                var roles = _roleService.Add(Role);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] RoleDto Role)
        {
            try
            {
                var roles = _roleService.Edit(Role);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
