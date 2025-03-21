using ApiChatbot.WebApi.Features.TypeDocuments.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiChatbot.WebApi.Features.TypeDocuments.Dto
{
    public class TypeDocumentDto: TypeDocument
    {
        public string CreatedByName { get; set; }
    }
}
