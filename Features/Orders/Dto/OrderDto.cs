using BaseApi.WebApi.Features.Orders.Entitie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseApi.WebApi.Features.Orders.Dto
{
    public class OrderDto: Order
    {
        public string CreatedByName { get; set; }
    }
}
