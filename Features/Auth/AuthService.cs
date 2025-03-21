using DBaseApi.Features.Auth.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using BaseApi.WebApi.Infraestructure;
using BaseApi.WebApi.Features.Users.Entities;
using BaseApi.WebApi.Features.Common.Dto;
using System.Collections.Generic;
using BaseApi.WebApi.Helpers;

namespace BaseApi.WebApi.Features.Auth
{
    public class AuthService
    {
        private readonly BaseApiDbContext _baseApiDbContext;
        private readonly IConfiguration _configuration;
        public AuthService(BaseApiDbContext logisticaBtdDbContext, IConfiguration configuration)
        {
            _baseApiDbContext = logisticaBtdDbContext;
            _configuration = configuration;
        }

        public UserDto Auth(User User)
        {
            // Se cifra la contraseña del usuario utilizando un método EncryptPassword
            User.Password = Helper.EncryptPassword(User.Password, _configuration);

            // Se busca al usuario en la base de datos por nombre de usuario y contraseña
            var employee = _baseApiDbContext.User
                .Where(x => x.UserName.ToUpper() == User.UserName && x.Password == User.Password)
                .FirstOrDefault();

            // Se realizan validaciones
            if (employee == null) throw new Exception("Usuario o contraseña incorrecta.");
            if (!employee.Active) throw new Exception("Usuario Inactivo.");

            // Se obtienen los permisos asignados al usuario
            var userPermission = _baseApiDbContext.RolePermission.Where(x => x.RoleId == employee.RoleId).ToList();
            var permissionIds = userPermission.Select(x => x.PermissionId).ToList();

            // Se realizan validaciones adicionales relacionadas con los permisos
            if (userPermission.Count() == 0) throw new Exception("Su usuario no tiene ningún permiso asignado");
            var permissions = _baseApiDbContext.Permission.Where(x => permissionIds.Contains(x.PermissionId) && x.Active).ToList();

            // Se obtienen información adicional sobre el rol y el tema del usuario
            var role = _baseApiDbContext.Role.Where(x => x.RoleId == employee.RoleId).FirstOrDefault();
            var theme = _baseApiDbContext.Theme.Where(x => x.ThemeId == employee.ThemeId).FirstOrDefault();

            // Se realizan validaciones adicionales relacionadas con el rol
            if (role == null) throw new Exception("No se le ha asignado un rol");

            // Se crea un objeto UserDto con la información del usuario autenticado
            var dato = new UserDto
            {
                Name = employee.Name,
                UserId = employee.UserId,
                Email = employee.Email,
                UserName = employee.UserName,
                Permissions = permissions.OrderBy(x => x.Position).ToList(),
                Menu = GetMenu(permissions),
                RoleId = employee.RoleId,
                ThemeId = employee.ThemeId,
                Role = role.Description,
                Theme = theme.Code,
                Token = GenerateJwtToken(employee)
            };

            // Se devuelve el objeto UserDto
            return dato;
        }

        public List<MenuDto> GetMenu(List<Permission> permissions)
        {
            // Filtra los permisos que son de tipo "Pantalla" y no son botones
            var permissionsNoBtn = permissions.Where(x => x.TypeId == (int)TypePermissionEnum.Pantalla).OrderBy(x => x.PermissionId).ToList();
            // Crea la estructura principal del menú
            var data = permissions.Where(x => x.FatherId == 0 && x.TypeId == (int)TypePermissionEnum.Pantalla).Select(x => new MenuDto
            {
                Icon = x.Icon,
                Label = x.Description,
                MenuId = x.PermissionId,
                PositionId = x.Position,
                RouterLink = x.Path,
                Items = GetMenuItems(permissionsNoBtn.Where(c => c.FatherId == x.PermissionId).ToList(), permissionsNoBtn)
            }).ToList();
            // Retorna la estructura del menú
            return data;
        }

        public List<MenuDto> GetMenuItems(List<Permission> permissions, List<Permission> originalPermissions)
        {
            // Crea la estructura de los elementos de menú recursivamente
            var data = permissions.Select(x => new MenuDto
            {
                Icon = x.Icon,
                Label = x.Description,
                MenuId = x.PermissionId,
                RouterLink = x.Path,
                PositionId = x.Position,
                Items = GetMenuItems(originalPermissions.Where(c => c.FatherId == x.PermissionId).ToList(), originalPermissions).Count() == 0 ? null :
                    GetMenuItems(originalPermissions.Where(c => c.FatherId == x.PermissionId).ToList(), originalPermissions)
            }).ToList();
            // Retorna la estructura de los elementos de menú
            return data;
        }

        private string GenerateJwtToken(User User)
        {
            // Se obtiene la clave secreta para firmar el token desde la configuración
            var key = Encoding.ASCII.GetBytes(_configuration["secret"]);
            // Se crea un manejador de tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            // Se configura la información del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Se define la identidad del sujeto (usuario) del token con claims (reclamaciones)
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, User.UserName),                 // Claim para el correo electrónico
                    new Claim(ClaimTypes.NameIdentifier, User.UserId.ToString()) // Claim para el identificador del usuario
                // Puedes agregar más claims según sea necesario
                }),

                // Se establece la fecha de expiración del token (30 días en este caso)
                Expires = DateTime.UtcNow.AddDays(30),
                // Se especifica la audiencia y emisor del token
                Audience = "localhost",
                Issuer = "localhost",
                // Se configuran las credenciales de firma utilizando una clave simétrica
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            // Se crea el token utilizando la configuración proporcionada
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Se devuelve el token en forma de cadena
            return tokenHandler.WriteToken(token);
        }



    }
}
