using System;
using System.Collections.Generic;
using System.Linq;
using DBaseApi.Features.Auth.Dto;
using BaseApi.WebApi.Features.Users.Entities;
using BaseApi.WebApi.Features.Common.Entities;
using BaseApi.WebApi.Infraestructure;
using Microsoft.Extensions.Configuration;
using BaseApi.WebApi.Helpers;
using Sap.Data.Hana;
using BaseApi.WebApi.Features.ServiceLayer;
using BaseApi.WebApi.Features.ServiceLayer.Dto;

namespace BaseApi.WebApi.Features.Users
{
    public class UserService
    {
        private readonly BaseApiDbContext _baseApiDbContext;
        private readonly IConfiguration _configuration;
        private readonly HanaDbContext _hanaDbContext;
        private readonly AuthSapServices _authSapService;
        private readonly OrderPurchaseServices _orderPurchaseServices;

        public UserService(BaseApiDbContext baseApiDbContext, IConfiguration configuration, HanaDbContext hanaDbContext, AuthSapServices authSapService, OrderPurchaseServices orderPurchaseServices)
        {
            _baseApiDbContext = baseApiDbContext;
            _configuration = configuration;
            _hanaDbContext = hanaDbContext;
            _authSapService = authSapService;
            _orderPurchaseServices = orderPurchaseServices;
        }

        public List<UserDto> Get()
        {
            var users = _baseApiDbContext.User.ToList();
            var themes = _baseApiDbContext.Theme.ToList();
            var roles = _baseApiDbContext.Role.ToList();

            var result = (from u in users
                          join r in roles on u.RoleId equals r.RoleId into userRole
                          from r in userRole.DefaultIfEmpty()
                          join t in themes on u.ThemeId equals t.ThemeId into themeUser
                          from t in themeUser.DefaultIfEmpty()
                          select new UserDto
                          {
                              Active = u.Active,
                              Email = u.Email,
                              Name = u.Name,
                              Password = null,
                              RoleId = u.RoleId,
                              ThemeId = u.ThemeId,
                              UserId = u.UserId,
                              UserName = u.UserName,
                              Role = r?.Description ??"ROL NO ASIGNADO",
                              Theme = t?.Description ?? "TEMA NO ASIGNADO"
                          }
                          ).ToList();
            return result;
        }

        public List<UserDto> Add(User user)
        {
            user.IsValid();
            if (string.IsNullOrEmpty(user.Password)) throw new Exception("Debe ingresar una contraseña");
            if (user.Password.Length <8) throw new Exception("Debe ingresar una contraseña que contenga al menos 8 caracteres");
            user.Active = true;
            user.Password = Helper.EncryptPassword(user.Password.Trim(), _configuration);
            user.UserName = user.UserName.Trim().ToLower();
            _baseApiDbContext.User.Add(user);
            _baseApiDbContext.SaveChanges();
            return Get();
        }


        public List<UserDto> Edit(User user)
        {
            user.IsValid();
            if (!string.IsNullOrEmpty(user.Password))
            {
                if (user.Password.Length < 8) throw new Exception("Debe ingresar una contraseña que contenga al menos 8 caracteres");
                user.Password = Helper.EncryptPassword(user.Password.Trim(), _configuration);
            }
            var currentUser = _baseApiDbContext.User.Where(x => x.UserId == user.UserId).FirstOrDefault();
            currentUser.Name = user.Name;
            currentUser.Email = user.Email;
            currentUser.RoleId = user.RoleId;
            currentUser.ThemeId = user.ThemeId;
            currentUser.Active = user.Active;
<<<<<<< HEAD
            _baseApiDbContext.User.Update(currentUser);
=======
            currentUser.Password = user.Password;
>>>>>>> 7d6f00e (Refactorizado)
            _baseApiDbContext.SaveChanges();
            return Get();
        }

        public List<Theme> GetThemes()
        {
            var themes = _baseApiDbContext.Theme.ToList();
            return themes;
        }

        public List<string> GetSellersSAP()
        {
            List<string> result = new List<string>();
            _hanaDbContext.Conn.Open();
            string query = $@"SELECT ""SlpName"" FROM ""TEST_CHAMER"".""OSLP"" ";
            HanaCommand selectCmd = new HanaCommand(query, _hanaDbContext.Conn);
            HanaDataReader dr = selectCmd.ExecuteReader();
            while (dr.Read())
            {
                string slpName = dr.GetString(0);
                result.Add(slpName);
            }
            dr.Close();
            _hanaDbContext.Conn.Close();
            return result;
        }

 

    }
}
