using System.Collections.Generic;
using ApiChatbot.WebApi.Features.Users.Entities;
using ApiChatbot.WebApi.Features.Common.Dto;

namespace DApiChatbot.Features.Auth.Dto
{
    public class UserDto : User
    {
        public string CreatedByName { get; set; }
        public string Theme { get; set; }
        public string Role { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<MenuDto> Menu { get; set; }
        public string Token { get; set; }
    }
}
