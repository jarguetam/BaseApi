using System;
using BaseApi.WebApi.Features.Users.Entities;

namespace BaseApi.WebApi.Features.Users.Dto
{
    public class PermissionDto: Permission
    {
        public int RoleId { get; set; }
    }
}
