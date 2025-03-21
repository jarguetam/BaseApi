using System;
using BaseApi.WebApi.Features.Users.Entities;
using BaseApi.WebApi.Features.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.WebApi.Features.Users
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController : ControllerBase
    {
        private PermissionService _permissionService;
        public PermissionController(PermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var result = _permissionService.Get();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetTypePermission")]
        public IActionResult GetTypePermission()
        {
            try
            {
                var result = _permissionService.GetTypePermission();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("ById/{PermissionId}")]
        public IActionResult GetById(int PermissionId)
        {
            try
            {
                var result = _permissionService.GetById(PermissionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add([FromBody] Permission permission)
        {
            try
            {
                var result = _permissionService.Add(permission);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        public IActionResult Edit([FromBody] Permission permission)
        {
            try
            {
                var result = _permissionService.Edit(permission);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
