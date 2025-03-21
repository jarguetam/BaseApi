using Microsoft.Extensions.Configuration;
using Sap.Data.Hana;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using System;

namespace ApiChatbot.WebApi.Infraestructure
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Sap.Data.Hana;

    public class HanaDbContext : IDisposable
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

        /// <summary>
        /// Ejecuta una consulta SQL contra SAP HANA y devuelve una lista de objetos dinámicos.
        /// </summary>
        /// <param name="query">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Lista opcional de parámetros para la consulta</param>
        /// <returns>Lista de objetos dinámicos con los resultados de la consulta</returns>
        public async Task<List<dynamic>> ExecuteQueryAsync(string query, List<object> parameters = null)
        {
            var result = new List<dynamic>();

            // Verificar si la conexión está abierta, si no, abrirla
            if (Conn.State != ConnectionState.Open)
            {
                await Conn.OpenAsync();
            }

            try
            {
                using (var command = Conn.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;

                    // Agregar parámetros - en HANA los parámetros se agregan por posición
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var paramValue in parameters)
                        {
                            var hanaParam = new HanaParameter
                            {
                                Value = paramValue ?? DBNull.Value
                            };

                            // Establecer el tipo según el tipo del valor
                            if (paramValue != null)
                            {
                                hanaParam.HanaDbType = GetHanaDbType(paramValue.GetType());
                            }
                            command.Parameters.Add(hanaParam);
                        }
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Convertir los resultados a una lista de objetos dinámicos
                        result = await ReadDataAsync(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes agregar código para registrar la excepción
                throw new Exception($"Error ejecutando consulta en SAP HANA: {ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado en SAP HANA y devuelve una lista de objetos dinámicos.
        /// </summary>
        /// <param name="procedureName">Nombre del procedimiento almacenado</param>
        /// <param name="parameters">Diccionario con los parámetros del procedimiento (nombre y valor)</param>
        /// <returns>Lista de objetos dinámicos con los resultados del procedimiento</returns>
        public async Task<List<dynamic>> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object> parameters = null)
        {
            var result = new List<dynamic>();

            // Verificar si la conexión está abierta, si no, abrirla
            if (Conn.State != ConnectionState.Open)
            {
                await Conn.OpenAsync();
            }

            try
            {
                using (var command = Conn.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedureName;

                    // Agregar parámetros con nombres
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var param in parameters)
                        {
                            var hanaParam = new HanaParameter
                            {
                                ParameterName = param.Key,
                                Value = param.Value ?? DBNull.Value
                            };

                            // Establecer el tipo según el tipo del valor
                            if (param.Value != null)
                            {
                                hanaParam.HanaDbType = GetHanaDbType(param.Value.GetType());
                            }
                            command.Parameters.Add(hanaParam);
                        }
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Convertir los resultados a una lista de objetos dinámicos
                        result = await ReadDataAsync(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes agregar código para registrar la excepción
                throw new Exception($"Error ejecutando procedimiento almacenado en SAP HANA: {ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// Lee los datos de un DataReader y los convierte en una lista de objetos dinámicos
        /// </summary>
        private async Task<List<dynamic>> ReadDataAsync(HanaDataReader reader)
        {
            var result = new List<dynamic>();

            // Obtener los nombres y tipos de las columnas
            var schemaTable = reader.GetSchemaTable();
            var columnNames = new List<string>();
            var columnTypes = new List<Type>();

            if (schemaTable != null)
            {
                foreach (DataRow row in schemaTable.Rows)
                {
                    columnNames.Add(row["ColumnName"].ToString());
                    columnTypes.Add((Type)row["DataType"]);
                }
            }

            // Leer los datos
            while (await reader.ReadAsync())
            {
                dynamic dynamicObject = new ExpandoObject();
                var expandoDict = (IDictionary<string, object>)dynamicObject;

                for (int i = 0; i < columnNames.Count; i++)
                {
                    var columnName = columnNames[i];
                    var ordinal = reader.GetOrdinal(columnName);

                    if (reader.IsDBNull(ordinal))
                    {
                        expandoDict[columnName] = null;
                    }
                    else
                    {
                        // Leer el valor según el tipo de datos
                        expandoDict[columnName] = GetValueByType(reader, ordinal, columnTypes[i]);
                    }
                }

                result.Add(dynamicObject);
            }

            return result;
        }

        /// <summary>
        /// Determina el tipo HanaDbType basado en el tipo .NET
        /// </summary>
        private HanaDbType GetHanaDbType(Type type)
        {
            if (type == typeof(string))
                return HanaDbType.NVarChar;
            else if (type == typeof(int) || type == typeof(Int32))
                return HanaDbType.Integer;
            else if (type == typeof(long) || type == typeof(Int64))
                return HanaDbType.BigInt;
            else if (type == typeof(decimal))
                return HanaDbType.Decimal;
            else if (type == typeof(double))
                return HanaDbType.Double;
            else if (type == typeof(DateTime))
                return HanaDbType.TimeStamp;
            else if (type == typeof(bool))
                return HanaDbType.Boolean;
            else if (type == typeof(byte[]))
                return HanaDbType.VarBinary;
            else if (type == typeof(float))
                return HanaDbType.Real;
            else if (type == typeof(short) || type == typeof(Int16))
                return HanaDbType.SmallInt;
            else if (type == typeof(TimeSpan))
                return HanaDbType.Time;
            else
                return HanaDbType.NVarChar; // Default
        }

        /// <summary>
        /// Obtiene el valor del DataReader según el tipo de datos
        /// </summary>
        private object GetValueByType(HanaDataReader reader, int ordinal, Type type)
        {
            if (type == typeof(string))
                return reader.GetString(ordinal);
            else if (type == typeof(int) || type == typeof(Int32))
                return reader.GetInt32(ordinal);
            else if (type == typeof(long) || type == typeof(Int64))
                return reader.GetInt64(ordinal);
            else if (type == typeof(decimal))
                return reader.GetDecimal(ordinal);
            else if (type == typeof(double))
                return reader.GetDouble(ordinal);
            else if (type == typeof(DateTime))
                return reader.GetDateTime(ordinal);
            else if (type == typeof(bool))
                return reader.GetBoolean(ordinal);
            else if (type == typeof(byte[]))
                return reader.GetBytes(ordinal, 0, null, 0, 0);
            else if (type == typeof(char))
                return reader.GetChar(ordinal);
            else if (type == typeof(float))
                return reader.GetFloat(ordinal);
            else if (type == typeof(short) || type == typeof(Int16))
                return reader.GetInt16(ordinal);
            else if (type == typeof(Guid))
                return reader.GetGuid(ordinal);
            else
                return reader.GetValue(ordinal); // Default
        }

        /// <summary>
        /// Método para cerrar explícitamente la conexión
        /// </summary>
        public void CloseConnection()
        {
            if (Conn != null && Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
        }

        /// <summary>
        /// Método para liberar recursos cuando se termina de usar
        /// </summary>
        public void Dispose()
        {
            CloseConnection();
            Conn?.Dispose();
        }
    }

}
