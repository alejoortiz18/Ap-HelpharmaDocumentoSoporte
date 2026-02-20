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
    public class DocumentoSoporteData : IDocumentoSoporteData
    {
        private readonly DbConnectionFactory _connectionFactory;

        public DocumentoSoporteData(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PacienteDto?> GetSoporte(string DCTOPRV)
        {
            PacienteDto? paciente = null;

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string query = @"
                SELECT
                    m.ORDENENTMV as ordenes,
                    m.producto,
                    m.nombre,
                    CAST(m.cantidad AS INT) as cantidad,
                    m.ORDENNRO as lote,
                    cn.NOMBRE as convenio,
                    CONVERT(VARCHAR(10), M.FECHA, 103) as fecha,
                    m.BODEGA as bodega,
                    tv.DESCRIPCIO as tipoEntrega,
                    T.TIPOCAR as cartera,
                    c.NOMBRE as nombrePaciente,
                    c.TIPODC as tipoId,
                    c.NIT as paciente,
                    c.DIRECCION as direccionPaciente,
                    c.TEL1 as telefonoPaciente,
                    c.CELULAR as celularPaciente,
                    c.COMENTARIO as complemento,
                    T.NOTA as observacion,
                    '' as valorCM,
                    M.VALORUNIT AS valorMx,
                    t.PASSWORDIN as usuario
                FROM trade t 
                    left join mvtrade m on t.origen = m.origen and t.tipodcto = m.tipodcto and t.nrodcto = m.nrodcto
                    left join trademas tm on t.tipodcto = tm.tipodcto and t.nrodcto = tm.nrodcto
                    INNER JOIN CANAL cn ON cn.CODCANAL = tm.CODCANAL 
                    left join mtprocli c on t.nit = c.nit
                    left join tipocar k on t.tipocar = k.codtc
                    inner join TIPOVTA TV ON TV.TIPOVTA = T.TIPOVTA
                WHERE
                    t.DCTOPRV = @DCTOPRV
                    and m.PRODUCTO <> 'S3501';";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@DCTOPRV", SqlDbType.VarChar).Value = DCTOPRV;

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
                            Fecha = reader["fecha"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(reader["fecha"]),
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
                paciente.ValorCMTotal += producto == "S3500" ? orden.ValorMx :0;
            }

            return paciente;
        }

        public async Task<PacienteDto?> GetSoporteTrade(TradeDto trade)
        {
            PacienteDto? paciente = null;

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            string query = @"
                SELECT
                    m.ORDENENTMV as ordenes,
                    m.producto,
                    m.nombre,
                    CAST(m.cantidad AS INT) as cantidad,
                    m.ORDENNRO as lote,
                    cn.NOMBRE as convenio,
                    CONVERT(VARCHAR(10), M.FECHA, 103) as fecha,
                    m.BODEGA as bodega,
                    tv.DESCRIPCIO as tipoEntrega,
                    T.TIPOCAR as cartera,
                    c.NOMBRE as nombrePaciente,
                    c.TIPODC as tipoId,
                    c.NIT as paciente,
                    c.DIRECCION as direccionPaciente,
                    c.TEL1 as telefonoPaciente,
                    c.CELULAR as celularPaciente,
                    c.COMENTARIO as complemento,
                    T.NOTA as observacion,
                    '' as valorCM,
                    M.VALORUNIT AS valorMx,
                    t.PASSWORDIN as usuario
                FROM trade t 
                    left join mvtrade m on t.origen = m.origen and t.tipodcto = m.tipodcto and t.nrodcto = m.nrodcto
                    left join trademas tm on t.tipodcto = tm.tipodcto and t.nrodcto = tm.nrodcto
                    INNER JOIN CANAL cn ON cn.CODCANAL = tm.CODCANAL 
                    left join mtprocli c on t.nit = c.nit
                    left join tipocar k on t.tipocar = k.codtc
                    inner join TIPOVTA TV ON TV.TIPOVTA = T.TIPOVTA
                WHERE
                    T.TIPODCTO= @TIPODCTO AND
                    T.NRODCTO= @NRODCTO
                    and m.PRODUCTO <> 'S3501';";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@TIPODCTO", SqlDbType.VarChar).Value = trade.Tipodcto;
            command.Parameters.Add("@NRODCTO", SqlDbType.VarChar).Value = trade.Nrodcto;

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
                            Fecha = reader["fecha"] == DBNull.Value
                                    ? DateTime.MinValue
                                    : Convert.ToDateTime(reader["fecha"]),
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
