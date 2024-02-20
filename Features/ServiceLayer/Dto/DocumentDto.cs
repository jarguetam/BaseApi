using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseApi.WebApi.Features.ServiceLayer.Dto
{
    public class DocumentDto
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public int Serie { get; set; }
        public string U_FIS { get; set; }
        public List<DocumentDetailDto> DocumentLines { get; set; }
    }
}
