using System;
using System.Collections.Generic;
using System.Linq;
using BaseApi.WebApi.Features.Users.Dto;
using BaseApi.WebApi.Features.Users.Entities;
using BaseApi.WebApi.Helpers;
using BaseApi.WebApi.Infraestructure;

namespace BaseApi.WebApi.Features.Users
{
    public class RoleService
    {
        private readonly BaseApiDbContext _baseApiDbContext;
        public RoleService(BaseApiDbContext logisticaBtdDbContext)
        {
            _baseApiDbContext = logisticaBtdDbContext;
        }
        public List<Role> Get()
        {
            var roles = _baseApiDbContext.Role.ToList();
            return roles;
        }
        public List<Role> GetActiveOnly()
        {
            var roles = _baseApiDbContext.Role.Where(x=> x.Active).ToList();
            return roles;
        }

        public RoleDto RoleWithDetail(int RoleId)
        {
            var role = _baseApiDbContext.Role.Where(x=> x.RoleId == RoleId).Select(x=> new RoleDto { Active = x.Active, Description = x.Description, RoleId = x.RoleId, Detail = new List<TreeNodeDto>() }).FirstOrDefault();
            if(role == null) role = new RoleDto{ Active = true, Description = "", RoleId = 0, Detail= new List<TreeNodeDto>()};
            var rolesPermissions = _baseApiDbContext.RolePermission.Where(x=> x.RoleId == role.RoleId).ToList();
            List<Permission> permissions = _baseApiDbContext.Permission.ToList();
            var roleWithPermissions = (from p in permissions
                                       join rp in rolesPermissions on p.PermissionId equals rp.PermissionId into rolePermisionData
                                       from rp in rolePermisionData.DefaultIfEmpty()
                                       select new PermissionDto
                                       {
                                           RoleId = rp?.RoleId ?? 0,
                                           Active = rp?.Active??false,
                                           Description = p.Description,
                                           FatherId = p.FatherId,
                                           Path = p.Path,
                                           PermissionId = p.PermissionId,
                                           TypeId = p.TypeId,
                                           Icon = p.Icon,
                                           Position = p.Position
                                           
                                       }
                                       ).ToList();

            role.Detail = roleWithPermissions.Where(x => x.FatherId == 0).Select(x => new TreeNodeDto
            {
                Icon = x.Icon,
                Label = x.Description,
                PermissionId = x.PermissionId,
                Key = x.PermissionId.ToString(),
                Data = x.Path,
                Expanded = false,
                TypeId = x.TypeId,
                Active = x.Active,
                PositionId = x.Position,
                FatherId = x.FatherId,
                Children = GenerateChildren(roleWithPermissions.Where(x => x.FatherId != 0).Where(c => c.FatherId == x.PermissionId).ToList(), roleWithPermissions)
            }).ToList();

            return role;
        }

        public List<TreeNodeDto> GenerateChildren(List<PermissionDto> permissions, List<PermissionDto> originalPermissions)
        {
            // Inicializa una lista para almacenar los nodos generados
            var data = permissions.Select(x => new TreeNodeDto
            {
                // Asigna las propiedades del nodo TreeNodeDto
                Icon = x.Icon,
                Label = x.Description,
                PermissionId = x.PermissionId,
                Key = x.PermissionId.ToString(),
                Data = x.Path,
                Expanded = false,
                FatherId = x.FatherId,
                TypeId = x.TypeId,
                Active = x.Active,
                PositionId = x.Position,
                // La propiedad Children se inicializa llamando recursivamente GenerateChildren
                Children = GenerateChildren(originalPermissions.Where(c => c.FatherId == x.PermissionId).ToList(), originalPermissions).Count() == 0
                    ? null  // Si no hay hijos, se establece en null
                    : GenerateChildren(originalPermissions.Where(c => c.FatherId == x.PermissionId).ToList(), originalPermissions)
                // Si hay hijos, se llama recursivamente a GenerateChildren para obtener los hijos
            }).ToList();

            // Devuelve la lista de nodos generados
            return data;
        }



        public List<Role> Add(RoleDto role)
        {
            try
            {
                role.IsValid();
                _baseApiDbContext.Database.BeginTransaction();
                var permissionIds = Helper.TreeNodeToList(role.Detail);

                var permissionsActive = permissionIds.Where(x => x.Active).ToList();
                if (permissionsActive.Count() == 0) throw new System.Exception("Debe seleccionar al menos un permiso");

                _baseApiDbContext.Role.Add(role);
                _baseApiDbContext.SaveChanges();

                List<RolePermission> rolePermissions = permissionsActive.Select(x => new RolePermission {
                    Active = true,
                    PermissionId = x.PermissionId,
                    RoleId  = role.RoleId,
                    RolePermissionId = 0
                }).ToList();
                _baseApiDbContext.RolePermission.AddRange(rolePermissions);
                _baseApiDbContext.SaveChanges();
                _baseApiDbContext.Database.CommitTransaction();
            }
            catch(Exception ex)
            {
                _baseApiDbContext.Database.RollbackTransaction();
                throw new System.Exception(ex.Message);
            }
            return Get();
        }

        public List<Role> Edit(RoleDto role)
        {
            try
            {
                if (role.IsValid())
                {
                    _baseApiDbContext.Database.BeginTransaction();
                    var permissionIds = Helper.TreeNodeToList(role.Detail);
                    var permissionsActive = permissionIds.Where(x => x.Active).ToList();
                    if (permissionsActive.Count() == 0) throw new System.Exception("Debe seleccionar al menos un permiso");

                    var currentRole = _baseApiDbContext.Role.Where(x => x.RoleId == role.RoleId).FirstOrDefault();
                    if (currentRole == null) throw new Exception("Al parecer el rol no existe");

                    currentRole.Description = role.Description;
                    currentRole.Active = role.Active;
                    List<RolePermission> rolePermissions = permissionsActive.Select(x => new RolePermission
                    {
                        Active = true,
                        PermissionId = x.PermissionId,
                        RoleId = currentRole.RoleId,
                        RolePermissionId = 0
                        
                    }).ToList();

                    var currentPermissions = _baseApiDbContext.RolePermission.Where(x => x.RoleId == currentRole.RoleId).ToList();
                    _baseApiDbContext.RolePermission.RemoveRange(currentPermissions);
                    _baseApiDbContext.RolePermission.AddRange(rolePermissions);

                    _baseApiDbContext.SaveChanges();
                    _baseApiDbContext.Database.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                _baseApiDbContext.Database.RollbackTransaction();
                throw new System.Exception(ex.Message);
            }
            return Get();
        }
    }
}
