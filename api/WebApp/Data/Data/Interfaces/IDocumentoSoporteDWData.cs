using Models.Dto.DocumentoSoporteDto;
using Models.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IDocumentoSoporteDWData
    {
        Task<PacienteDto?> GetSoporteOfima(string prefijo, int noEntrega);
    }
}
