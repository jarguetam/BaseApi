using System;
namespace ApiChatbot.WebApi.Features.Mails.Dto
{
    public class AttachmentEmail
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public Byte[] File { get; set; }
    }
}
