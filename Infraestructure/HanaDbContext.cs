using Microsoft.Extensions.Configuration;
using Sap.Data.Hana;

namespace BaseApi.WebApi.Infraestructure
{
    public class HanaDbContext
    {
        private readonly IConfiguration _config;
        public string ConnectionString { get; private set; }
        public HanaConnection Conn { get; private set; }

        public HanaDbContext(IConfiguration config)
        {
            _config = config;
            ConnectionString = _config["connectionStringHana"];
            Conn = new HanaConnection();
            Conn.ConnectionString = ConnectionString;
        }
    }

}
