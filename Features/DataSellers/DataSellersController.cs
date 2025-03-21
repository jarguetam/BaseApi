using ApiChatbot.WebApi.Features.DataSellers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ApiChatbot.WebApi.Features.DataSellers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSellersController : ControllerBase
    {
        private DataSellerServices _services;

        public DataSellersController(DataSellerServices services)
        {
            _services = services;
        }

        [HttpPost("ValidateSeller")]
        public IActionResult ValidateSeller(string phone)
        {
            try
            {
                var result = _services.ValidatePhoneSeller(phone);
                return Ok(new { response = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("SalesDayBySeller")]
        public IActionResult SalesDayBySeller(string codigo)
        {
            try
            {
                var result = _services.GetVentasDiariasVendedorAsync(codigo);
                return Ok(new { response = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("SalesSixMonthBySeller")]
        public IActionResult SalesSixMonth(string codigo)
        {
            try
            {
                var result = _services.GetSalesSixMonth(codigo);
                return Ok(new { response = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("TopCustomerBySeller")]
        public IActionResult TopCustomerBySeller(string codigo)
        {
            try
            {
                var result = _services.GetTopCustomers(codigo);
                return Ok(new { response = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("TopProductBySeller")]
        public IActionResult TopProductBySeller(string codigo)
        {
            try
            {
                var result = _services.GetTopProduct(codigo);
                return Ok(new { response = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetTransitProduct")]
        public IActionResult GetTransitProduct(string codigo)
        {
            try
            {
                var result = _services.GetTransitProduct(codigo);
                return Ok(new { response = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
