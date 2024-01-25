using System.Collections.Generic;
using BaseApi.WebApi.Features.Users.Entities;
using BaseApi.WebApi.Features.Common.Dto;

namespace DBaseApi.Features.Auth.Dto
{
    public class UserDto : User
    {
        public string Theme { get; set; }
        public string Role { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<MenuDto> Menu { get; set; }
        public string Token { get; set; }
    }
}
