using Models.Dto.DocumentoSoporteDto;
using Models.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IDocumentoSoporteData
    {
        Task<PacienteDto?> GetSoporte(string DCTOPRV);
        Task<PacienteDto?> GetSoporteTrade(TradeDto trade);
    }
}
