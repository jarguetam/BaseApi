using System;
using ApiChatbot.WebApi.Features.Users.Entities;

namespace ApiChatbot.WebApi.Features.Users.Dto
{
    public class PermissionDto: Permission
    {
        public int RoleId { get; set; }
    }
}
