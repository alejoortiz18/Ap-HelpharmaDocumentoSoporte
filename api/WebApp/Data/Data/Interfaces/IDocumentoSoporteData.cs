using Models.Dto.DocumentoSoporteDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IDocumentoSoporteData
    {
        Task<PacienteDto?> GetSoporte(string DCTOPRV);
    }
}
