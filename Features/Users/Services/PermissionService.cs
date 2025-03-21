using System;
using System.Collections.Generic;
using System.Linq;
using BaseApi.WebApi.Features.Users.Dto;
using BaseApi.WebApi.Features.Users.Entities;
using BaseApi.WebApi.Infraestructure;

namespace BaseApi.WebApi.Features.Users.Services
{
    public class PermissionService
    {
        private readonly BaseApiDbContext _baseApiDbContext;
        private readonly RoleService _roleService;
        public PermissionService(BaseApiDbContext logisticaBtdDbContext, RoleService roleService)
        {
            _baseApiDbContext = logisticaBtdDbContext;
            _roleService = roleService;
        }

        public Permission GetById(int PermissionId) {
            var permission = _baseApiDbContext.Permission.Where(x => x.PermissionId == PermissionId).FirstOrDefault();
            if (permission == null) return new Permission {Active = true, Description = "",Icon = "", FatherId = 0, Path = "", PermissionId = 0, TypeId = 0 };
            return permission;

        }

        public List<TypePermission> GetTypePermission()
        {
            var types = _baseApiDbContext.TypePermission.ToList();
            return types;
        }

        public List<TreeNodeDto> Get()
        {
            var permissions = _baseApiDbContext.Permission.Select(x=> new PermissionDto
            {
                Active = x.Active,
                Description = x.Description,
                FatherId = x.FatherId,
                Icon = x.Icon,
                Path = x.Path,
                PermissionId = x.PermissionId,
                RoleId = 0,
                Position = x.Position,
                TypeId = x.TypeId
            }).OrderBy(x => x.Position).ToList();

            var data = permissions.Where(x => x.FatherId == 0).Select(x => new TreeNodeDto
            {
                Icon = x.Icon,
                Label = x.Description,
                PermissionId = x.PermissionId,
                FatherId = x.FatherId,
                Key = x.PermissionId.ToString(),
                Data = x.Path,
                TypeId = x.TypeId,
                Expanded = false,
                Active = x.Active,
                PositionId = x.Position,
                Children = _roleService.GenerateChildren(permissions.Where(x => x.FatherId != 0).Where(c => c.FatherId == x.PermissionId).ToList(), permissions)
            }).OrderBy(x=> x.PositionId).ToList();

            return data;
        }

        public List<TreeNodeDto> Add(Permission permission)
        {
            permission.IsValid();
            permission.Active = true;
            _baseApiDbContext.Permission.Add(permission);
            _baseApiDbContext.SaveChanges();
            return Get();
        }

        public List<TreeNodeDto> Edit(Permission permission)
        {
            permission.IsValid();
            var currentPermission = _baseApiDbContext.Permission.Where(x => x.PermissionId == permission.PermissionId).FirstOrDefault();
            if (currentPermission == null) throw new Exception("El permiso seleccionado no existe");
            currentPermission.Active = permission.Active;
            currentPermission.Description = permission.Description;
            currentPermission.Path = permission.Path;
            currentPermission.Icon = permission.Icon;
            currentPermission.TypeId = permission.TypeId;
            currentPermission.PermissionId = permission.PermissionId;
            _baseApiDbContext.SaveChanges();
            return Get();
        }

    }
}
