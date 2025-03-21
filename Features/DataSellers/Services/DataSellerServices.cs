using ApiChatbot.WebApi.Infraestructure;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiChatbot.WebApi.Features.DataSellers.Services
{
    public class DataSellerServices
    {
        private ApiChatbotDbContext _context;
        private HanaDbContext _hanaDbContext;

        public DataSellerServices(ApiChatbotDbContext context, HanaDbContext hanaDbContext)
        {
            _context = context;
            _hanaDbContext = hanaDbContext;
        }

        public string ValidatePhoneSeller(string phone)
        {
            try
            {
                // Validar si el teléfono está vacío o es nulo
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return "El número de teléfono es obligatorio.";
                }
                // Remover el código de país (+504), espacios y guiones
                string cleanPhone = phone
                    .Replace("+504", "")
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Trim();

                // Validar longitud exacta de 8 dígitos
                if (cleanPhone.Length != 8)
                {
                    throw new Exception("El número de teléfono debe tener exactamente 8 dígitos.");
                }

                // Validar que todos sean números
                if (!cleanPhone.All(char.IsDigit))
                {
                    throw new Exception("El número de teléfono solo debe contener dígitos.");
                }

                // Buscar el vendedor en la base de datos
                var seller = _context.Seller
                    .Where(s => s.TelefonoVendedor.Trim() == cleanPhone && s.Activo)
                    .Select(s => new
                    {
                        s.CodigoVendedor,
                        s.NombreVendedor
                    })
                    .FirstOrDefault();
                if (seller == null)
                {
                    return "No se encontró un vendedor activo con el número de teléfono proporcionado.";
                }
                // Retornar la información del vendedor en formato deseado
                return seller.NombreVendedor;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return $"Error al validar el teléfono: {ex.Message}";
            }
        }

        public async Task<List<dynamic>> GetVentasDiariasVendedorAsync(string codigo)
        {
            var parameters = new Dictionary<string, object>
            {
                { "fechaini", DateTime.Now.Date },
                { "fechafin", DateTime.Now.Date },
                { "codigoVendedor", codigo}
            };

            return await _hanaDbContext.ExecuteStoredProcedureAsync("FERTICA_PRD.SP_VENTA_DIARIA_POR_VENDEDOR", parameters);
        }

        public async Task<List<dynamic>> GetSalesSixMonth(string codigo)
        {
            string query = @"SELECT 
                        MONTHNAME(T0.""DocDate"") AS ""Mes"",
                        YEAR(T0.""DocDate"") AS ""Año"",
                        SUM(T0.""TM"") AS ""TM"",
                        'Fertica' ""Empresa"" 
                    FROM ""FERTICA_PRD"".""MARGENRENTABILIDADFER"" T0
                    WHERE T0.""U_OS_CODIGO"" = ?
                    AND T0.""DocDate"" BETWEEN ADD_MONTHS(CURRENT_DATE, -6) AND CURRENT_DATE
                    GROUP BY MONTHNAME(T0.""DocDate""), YEAR(T0.""DocDate""), MONTH(T0.""DocDate"")
                    UNION ALL
                    SELECT 
                        MONTHNAME(T0.""DocDate"") AS ""Mes"",
                        YEAR(T0.""DocDate"") AS ""Año"",
                        SUM(T0.""(Quantity)"") AS ""TM"",
                        'Cadelga' ""Empresa"" 
                    FROM ""CADELGA_PRD"".""MARGENRENTABILIDADCAD"" T0
                    WHERE T0.""U_OS_CODIGO"" = ?
                    AND T0.""DocDate"" BETWEEN ADD_MONTHS(CURRENT_DATE, -6) AND CURRENT_DATE
                    GROUP BY MONTHNAME(T0.""DocDate""), YEAR(T0.""DocDate""), MONTH(T0.""DocDate"")";

            // Crear lista de parámetros
            // Agregamos el mismo parámetro dos veces, uno para cada parte de la consulta UNION
            List<object> parameters = new List<object> { codigo, codigo };

            return await _hanaDbContext.ExecuteQueryAsync(query, parameters);
        }

        public async Task<List<dynamic>> GetTopCustomers(string codigo)
        {
            string query = @"SELECT 
                                T0.""CardCode"" AS ""CodigoCliente"",
                                T0.""CardName"" AS ""NombreCliente"",
                                SUM(T0.""TM"") AS ""TotalVentas"",
                                'Fertica' AS ""Empresa""
                            FROM ""FERTICA_PRD"".""MARGENRENTABILIDADFER"" T0
                            WHERE T0.""U_OS_CODIGO"" = ?
                            AND YEAR(T0.""DocDate"") = YEAR(CURRENT_DATE)
                            GROUP BY T0.""CardCode"", T0.""CardName""
                            UNION ALL
                            SELECT 
                                T0.""CardCode"" AS ""CodigoCliente"",
                                T0.""CardName"" AS ""NombreCliente"",
                                SUM(T0.""VentaNeta""/T1.""Rate"") AS ""TotalVentas"",
                                'Cadelga' AS ""Empresa""
                            FROM ""CADELGA_PRD"".""MARGENRENTABILIDADCAD"" T0
                            LEFT JOIN ""CADELGA_PRD"".""ORTT"" T1 ON T1.""RateDate"" =T0.""DocDate"" AND T1.""Currency"" ='USD'
                            WHERE T0.""U_OS_CODIGO"" = ?
                            AND YEAR(T0.""DocDate"") = YEAR(CURRENT_DATE)
                            GROUP BY T0.""CardCode"", T0.""CardName""
                            ORDER BY ""TotalVentas"" DESC
                            LIMIT 10";

            // Crear lista de parámetros
            // Agregamos el mismo parámetro dos veces, uno para cada parte de la consulta UNION
            List<object> parameters = new List<object> { codigo, codigo };

            return await _hanaDbContext.ExecuteQueryAsync(query, parameters);
        }

        public async Task<List<dynamic>> GetTopProduct(string codigo)
        {
            string query = @"SELECT 
                                T0.""ItemCode"" AS ""CodigoProducto"",
                                T0.""Dscription"" AS ""NombreProducto"",
                                SUM(T0.""TM"") AS ""TotalVentas"",
                                'Fertica' AS ""Empresa""
                            FROM ""FERTICA_PRD"".""MARGENRENTABILIDADFER"" T0
                            WHERE T0.""U_OS_CODIGO"" = ?
                            AND YEAR(T0.""DocDate"") = YEAR(CURRENT_DATE)
                            GROUP BY T0.""ItemCode"", T0.""Dscription""
                            UNION ALL
                            SELECT 
                                T0.""ItemCode"" AS ""CodigoProducto"",
                                T0.""Dscription"" AS ""NombreProducto"",
                                SUM(T0.""VentaNeta""/T1.""Rate"") AS ""TotalVentas"",
                                'Cadelga' AS ""Empresa""
                            FROM ""CADELGA_PRD"".""MARGENRENTABILIDADCAD"" T0
                            LEFT JOIN ""CADELGA_PRD"".""ORTT"" T1 ON T1.""RateDate"" =T0.""DocDate"" AND T1.""Currency"" ='USD'
                            WHERE T0.""U_OS_CODIGO"" = ?
                            AND YEAR(T0.""DocDate"") = YEAR(CURRENT_DATE)
                            GROUP BY T0.""ItemCode"", T0.""Dscription""
                            ORDER BY ""TotalVentas"" DESC
                            LIMIT 10";

            // Crear lista de parámetros
            // Agregamos el mismo parámetro dos veces, uno para cada parte de la consulta UNION
            List<object> parameters = new List<object> { codigo, codigo };

            return await _hanaDbContext.ExecuteQueryAsync(query, parameters);
        }

        public async Task<List<dynamic>> GetTransitProduct(string empresa)
        {
            string query;
            List<object> parameters = new List<object>();

            // Seleccionar la consulta según la empresa
            if (string.Equals(empresa.ToLower(), "cadelga", StringComparison.OrdinalIgnoreCase))
            {
                query = @"SELECT
                    DISTINCT 
                    T1.""ItemCode"" ""Codigo"",
                    T1.""Dscription"" ""Descripcion"",
                    T1.""InvQty"" AS ""Cantidad"",
                    T3.""UomName"" AS ""UnidadVenta"",
                    T0.""DocDueDate"" AS ""FechaEstimada""
                FROM CADELGA_PRD.OPOR T0 
                INNER JOIN CADELGA_PRD.POR1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
                INNER JOIN CADELGA_PRD.OITM T2 ON T1.""ItemCode"" = T2.""ItemCode"" 
                INNER JOIN CADELGA_PRD.OUOM T3 ON T2.""SUoMEntry"" = T3.""UomEntry""
                WHERE T0.""CANCELED"" = 'N' 
                AND T0.""DocStatus"" = 'O' 
                AND T1.""OpenQty"" != 0
                AND T1.""ItemCode"" LIKE '01%' 
                AND YEAR(T0.""DocDate"") = YEAR(CURRENT_DATE) 
                AND T2.""SellItem"" = 'Y'
                ORDER BY T1.""ItemCode""";
            }
            else if (string.Equals(empresa.ToLower(), "fertica", StringComparison.OrdinalIgnoreCase))
            {
                query = @"SELECT
                    DISTINCT 
                    T1.""ItemCode"" ""Codigo"",
                    T1.""Dscription"" ""Descripcion"",
                    T1.""InvQty"" AS ""Cantidad"",
                    T3.""UomName"" AS ""UnidadVenta"",
                    T0.""DocDueDate"" AS ""FechaEstimada""
                FROM FERTICA_PRD.OPOR T0 
                INNER JOIN FERTICA_PRD.POR1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
                INNER JOIN FERTICA_PRD.OITM T2 ON T1.""ItemCode"" = T2.""ItemCode"" 
                INNER JOIN FERTICA_PRD.OUOM T3 ON T2.""SUoMEntry"" = T3.""UomEntry""
                WHERE T0.""CANCELED"" = 'N' 
                AND T0.""DocStatus"" = 'O' 
                AND T1.""OpenQty"" != 0
                AND T1.""ItemCode"" LIKE '01%' 
                AND YEAR(T0.""DocDate"") = YEAR(CURRENT_DATE) 
                AND T2.""SellItem"" = 'Y'
                AND T2.""ItmsGrpCod"" NOT IN (107)
                ORDER BY T1.""ItemCode""";
            }
            else
            {
                // Si la empresa no es reconocida, lanzar una excepción
                throw new ArgumentException($"Empresa no válida: {empresa}. Las opciones válidas son 'cadelga' o 'fertica'.");
            }
            // Ejecutar la consulta
            return await _hanaDbContext.ExecuteQueryAsync(query, parameters);
        }


    }
}
