using ApiChatbot.WebApi.Features.TypeDocuments.Dto;
using ApiChatbot.WebApi.Features.TypeDocuments.Entities;
using ApiChatbot.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiChatbot.WebApi.Features.TypeDocuments.Services
{
    public class TypeDocumentServices
    {
        private readonly ApiChatbotDbContext _context;

        public TypeDocumentServices(ApiChatbotDbContext context)
        {
            _context = context;
        }

        public List<TypeDocumentDto> GetTypeDocument()
        {
            var result = (from t in _context.TypeDocument
                          join u in _context.User on t.CreatedBy equals u.UserId
                          select new TypeDocumentDto
                          {
                              Id = t.Id,
                              Name = t.Name,
                              CreatedDate = t.CreatedDate,
                              CreatedBy = t.CreatedBy,
                              CreatedByName = u.Name
                          }).ToList();                
            return result;
        }

        public List<TypeDocumentDto> GetTypeDocumentById(int id)
        {
            var result = (from t in _context.TypeDocument
                          join u in _context.User on t.CreatedBy equals u.UserId
                          where t.Id == id
                          select new TypeDocumentDto
                          {
                              Id = t.Id,
                              Name = t.Name,
                              CreatedDate = t.CreatedDate,
                              CreatedBy = t.CreatedBy,
                              CreatedByName = u.Name
                          }).ToList();
            return result;
        }

        public List<TypeDocumentDto> Add(TypeDocument request)
        {
            request.IsValid();
            request.CreatedDate = DateTime.Now;
            _context.TypeDocument.Add(request);
            _context.SaveChanges();
            return GetTypeDocument();
        }


    }
}
