using BaseApi.WebApi.Features.DataMaster.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseApi.WebApi.Features.DataMaster
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataMasterController : ControllerBase
    {
        private readonly DataMasterServices _service;

        public DataMasterController(DataMasterServices service)
        {
            _service = service;
        }

        [HttpGet("GetItems")]
        public IActionResult GetItems()
        {
            try
            {
                var result = _service.GetItems();
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message= ex.Message });
            }
        }

        [HttpGet("GetSupplier")]
        public IActionResult GetSupplier()
        {
            try
            {
                var result = _service.GetSupplier();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
