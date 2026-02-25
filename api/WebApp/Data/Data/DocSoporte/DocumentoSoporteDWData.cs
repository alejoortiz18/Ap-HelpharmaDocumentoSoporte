using Data.Interfaces;
using Microsoft.Data.SqlClient;
using Models.Dto.DocumentoSoporteDto;
using Models.Dto.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Data.DocSoporte
{
    public class DocumentoSoporteDWData : IDocumentoSoporteDWData
    {
        private readonly DbConnectionFactory _connectionFactory;

        public DocumentoSoporteDWData(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PacienteDto?> GetSoporteDW(string prefijo, int noEntrega)
        {
            PacienteDto? paciente = null;

            using var connection = _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                SELECT m.ORDEN AS ordenes
	                ,m.IdMedicamento AS producto
	                ,m.nombre 
	                ,CAST(m.QtyEntrega AS INT) as cantidad
	                ,m.IdLote AS lote
	                ,cn.NOMBRE AS convenio
	                ,CONVERT(VARCHAR(10), T.FECHA, 103) AS fecha
	                ,T.IdBodega AS bodega
	                ,tv.Nombre AS tipoEntrega
	                ,T.IdCartera AS cartera
	                ,c.NOMBRE AS nombrePaciente
	                ,c.IdTipoId AS tipoId
	                ,c.IdPaciente AS paciente
	                ,c.DIRECCION AS direccionPaciente
	                ,c.Telefono AS telefonoPaciente
	                ,c.CELULAR AS celularPaciente
	                ,c.Direccion2 AS complemento
	                ,T.Observacion AS observacion
	                ,T.ValorCM AS valorCM
	                ,M.Valor AS valorMx
	                ,T.IdUsuario AS usuario
                FROM MvEntregas M WITH (NOLOCK)
	                INNER JOIN Entregas T ON M.Prefijo=T.Prefijo AND M.NoEntrega=T.NoEntrega
	                INNER JOIN Convenios CN ON T.IdConvenio=CN.IdConvenio
	                INNER JOIN Pacientes C ON T.IdPaciente=C.IdPaciente
	                INNER JOIN Carteras K ON T.IdCartera=K.IdCartera
	                INNER JOIN TiposEntrega TV ON T.IdTipoEntrega=TV.IdTipoEntrega
                WHERE M.Prefijo=@prefijo AND M.NoEntrega=@noEntrega
                ";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@prefijo", SqlDbType.VarChar).Value = prefijo;
            command.Parameters.Add("@noEntrega", SqlDbType.VarChar).Value = noEntrega;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                // ✅ Crear paciente una sola vez
                if (paciente == null)
                {
                    paciente = new PacienteDto
                    {
                        NombrePaciente = Convert.ToString(reader["nombrePaciente"]).Trim(),
                        TipoId = Convert.ToString(reader["tipoId"]).Trim(),
                        Paciente = Convert.ToString(reader["paciente"]).Trim(),
                        DireccionPaciente = Convert.ToString(reader["direccionPaciente"]).Trim(),
                        TelefonoPaciente = Convert.ToString(reader["telefonoPaciente"]).Trim(),
                        CelularPaciente = Convert.ToString(reader["celularPaciente"]).Trim(),
                        Complemento = Convert.ToString(reader["complemento"]).Trim(),
                        ValorCMTotal = 0,

                        Factura = new FacturaDto
                        {
                            Convenio = Convert.ToString(reader["convenio"]).Trim(),
                            Fecha = reader.IsDBNull(reader.GetOrdinal("fecha"))
                                    ? DateTime.MinValue
                                    : reader.GetDateTime(reader.GetOrdinal("fecha")),
                            Bodega = Convert.ToString(reader["bodega"]).Trim(),
                            TipoEntrega = Convert.ToString(reader["tipoEntrega"]).Trim(),
                            Cartera = Convert.ToString(reader["cartera"]).Trim(),
                            Observacion = Convert.ToString(reader["observacion"]).Trim(),
                            Usuario = Convert.ToString(reader["usuario"]).Trim(),
                            Ordenes = new List<OrdenDto>(),
                        },

                    };
                }
                var valorMx = reader["valorMx"] == DBNull.Value
                                ? 0
                                : Convert.ToDecimal(reader["valorMx"]);

                var producto = Convert.ToString(reader["producto"]).Trim();

                // ✅ regla de negocio: cuota moderadora positiva
                if (producto == "S3500")
                {
                    valorMx = Math.Abs(valorMx);
                }

                var orden = new OrdenDto
                {
                    Ordenes = Convert.ToString(reader["ordenes"]).Trim(),
                    Producto = producto.Trim(),
                    Nombre = Convert.ToString(reader["nombre"]).Trim(),
                    Cantidad = reader["cantidad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["cantidad"]),
                    Lote = Convert.ToString(reader["lote"]).Trim(),
                    ValorMx = valorMx
                };

                paciente.Factura.Ordenes.Add(orden);

                // ✅ ACUMULAR correctamente
                paciente.ValorCMTotal += producto == "S3500" ? orden.ValorMx : 0;
            }

            return paciente;
        }
    }
}

