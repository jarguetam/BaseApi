using Microsoft.Extensions.Configuration;
using Sap.Data.Hana;

namespace BaseApi.WebApi.Infraestructure
{
    public class HanaDbContext
    {
        private readonly IConfiguration _config;
        public string Conexion { get; private set; }
        public HanaConnection Conn { get; private set; }

        public HanaDbContext(IConfiguration config)
        {
            _config = config;
            Conexion = _config["connectionStringHana"];
            Conn = new HanaConnection();
            Conn.ConnectionString = Conexion;
        }
    }

}
