using BaseApi.WebApi.Features.DataMaster.Dto;
using BaseApi.WebApi.Infraestructure;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseApi.WebApi.Features.DataMaster.Services
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
            string query = $@"SELECT T0.""CardCode"", T0.""CardName"" FROM ""PRUEBAS_INTERCOSMO"".""OCRD"" T0 WHERE T0.""CardType"" ='S' AND  T0.""CardCode"" LIKE 'PN%'";
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
            string query = $@"SELECT T0.""ItemCode"", T0.""ItemName"" FROM ""PRUEBAS_INTERCOSMO"".""OITM"" T0 WHERE T0.""ItemCode"" LIKE 'P%'";
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
