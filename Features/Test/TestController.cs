using Microsoft.AspNetCore.Mvc;
using System;

namespace BaseApi.WebApi.Common.Test
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {
           
        }
        [HttpGet]
        public IActionResult Test()
        {
            return Ok(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }
    }
}
