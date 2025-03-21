using ApiChatbot.WebApi.Features.DataMaster.Dto;
using ApiChatbot.WebApi.Infraestructure;
using Sap.Data.Hana;
using System.Collections.Generic;

namespace ApiChatbot.WebApi.Features.DataMaster.Services
{
    public class DataMasterServices
    {
        private readonly HanaDbContext _context;

        public DataMasterServices(HanaDbContext context)
        {
            _context = context;
        }

        public List<DataMasterDto> GetSupplier()
        {
            List<DataMasterDto> supplierList = new List<DataMasterDto>();
            _context.Conn.Open();
            string query = $@"SELECT T0.""CardCode"", T0.""CardName"" FROM ""FERTICA_TEST"".""OCRD"" T0 WHERE T0.""CardType"" ='S' AND  T0.""CardCode"" LIKE 'PN%'";
            HanaCommand selectCmd = new HanaCommand(query, _context.Conn);
            HanaDataReader dr = selectCmd.ExecuteReader();
            while (dr.Read())
            {
                DataMasterDto data = new DataMasterDto
                {
                    Code = dr.GetString(0),
                    Name = dr.GetString(1)
                };
                supplierList.Add(data);
            }
            dr.Close();
            _context.Conn.Close();
            return supplierList;
        }

        public List<DataMasterDto> GetItems()
        {
            List<DataMasterDto> itemsList = new List<DataMasterDto>();          
            _context.Conn.Open();
            string query = $@"SELECT T0.""ItemCode"", T0.""ItemName"" FROM ""FERTICA_TEST"".""OITM"" T0 WHERE T0.""ItemCode"" LIKE 'P%'";
            HanaCommand selectCmd = new HanaCommand(query, _context.Conn);
            HanaDataReader dr = selectCmd.ExecuteReader();
            while (dr.Read())
            {
                DataMasterDto data = new DataMasterDto
                {
                    Code = dr.GetString(0),
                    Name = dr.GetString(1)
                };
                itemsList.Add(data);
            }
            dr.Close();
            _context.Conn.Close();
            return itemsList;
        }

    }
}
