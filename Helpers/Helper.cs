using BaseApi.WebApi.Features.Users.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BaseApi.WebApi.Helpers
{
    public static class Helper
    {
        public static T ToObject<T>(this Object fromObject)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(fromObject));
        }


        public static List<T> ToObjectList<T>(this Object fromObject)
        {
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(fromObject));
        }


        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration config)
        {
            string security = config["secret"];
            var key = Encoding.ASCII.GetBytes(security);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "localhost",
                    ValidAudience = "localhost"
                };
            });

            return services;
        }

        public static string EncryptPassword(string Contrasena, IConfiguration _configuration)
        {
            // Obtenemos los valores necesarios de la configuración
            string HashContrasena = _configuration["HashContrasena"];
            string SaltContrasena = _configuration["SaltContrasena"];
            string KeyContrasena = _configuration["KeyContrasena"];

            // Convertimos la contraseña y la sal a bytes
            byte[] ContrasenaEnBytes = Encoding.UTF8.GetBytes(Contrasena);
            byte[] SaltEnBytes = new Rfc2898DeriveBytes(HashContrasena, Encoding.ASCII.GetBytes(SaltContrasena)).GetBytes(256 / 8);

            // Configuramos el algoritmo de cifrado Rijndael
            var simetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };

            // Creamos el encriptador con la clave y el vector de inicialización (IV)
            var encriptador = simetricKey.CreateEncryptor(SaltEnBytes, Encoding.ASCII.GetBytes(KeyContrasena));

            // Variable para almacenar la contraseña cifrada
            byte[] contrasenaFinal;

            // Usamos un MemoryStream para almacenar la contraseña cifrada
            using (var memoria = new MemoryStream())
            {
                // Usamos CryptoStream para cifrar los datos y escribirlos en el MemoryStream
                using (var memoriaEncriptada = new CryptoStream(memoria, encriptador, CryptoStreamMode.Write))
                {
                    // Escribimos la contraseña en bytes en el flujo encriptado
                    memoriaEncriptada.Write(ContrasenaEnBytes, 0, ContrasenaEnBytes.Length);
                    // Finalizamos la escritura y liberamos recursos
                    memoriaEncriptada.FlushFinalBlock();

                    // Obtenemos la contraseña cifrada como un array de bytes
                    contrasenaFinal = memoria.ToArray();
                    // Cerramos el CryptoStream
                    memoriaEncriptada.Close();
                }
                // Cerramos el MemoryStream
                memoria.Close();
            }

            // Convertimos la contraseña cifrada a Base64 y la devolvemos
            return Convert.ToBase64String(contrasenaFinal);
        }

        public static int GetFromTokenUserId(string Token)
        {
            Token = Token.ToString().Replace("Bearer ", "");
            var token = new JwtSecurityToken(jwtEncodedString: Token);
            var userID = token.Claims.First(c => c.Type == "nameid").Value;
            if (userID == null) throw new Exception("Token no valido");
            return int.Parse(userID);
        }

        public static List<TreeNodeDto> TreeNodeToList(List<TreeNodeDto> nodes)
        {
            List<TreeNodeDto> list = new List<TreeNodeDto>();
            foreach (TreeNodeDto node in ToIEnumerable(nodes))
                list.Add(node);
            return list;
        }
        public static IEnumerable<TreeNodeDto> ToIEnumerable(List<TreeNodeDto> nodes)
        {
            foreach (TreeNodeDto c1 in nodes)
            {
                yield return c1;
                foreach (TreeNodeDto c2 in ToIEnumerable(c1.Children ?? new List<TreeNodeDto>()))
                {
                    yield return c2;
                }
            }
        }

        public static string GenerateIdEnconde(long plainText)
        {

            string clearText = "";

            string EncryptionKey = "EEAK42SPBNI99212"; //TODO  Cambiar
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText.ToString());
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string DecodeIdEnconde(string cipherText)
        {
            cipherText = cipherText.Replace(' ', '+');
            string EncryptionKey = "EEAK42SPBNI99212";//TODO  Cambiar
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
