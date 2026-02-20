using Data.Interfaces;
using Models.Dto.DocumentoSoporteDto;
using Models.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.DocSoporteBusiness
{
    public class DocSuportBusiness : IDocSoportBusiness
    {
        private readonly IDocumentoSoporteData _docSopData;

        public DocSuportBusiness(IDocumentoSoporteData doc)
        {
            _docSopData = doc;
        }

        public Task<PacienteDto?> GetSoporte(string DCTOPRV)
        {
            return _docSopData.GetSoporte(DCTOPRV);
        }

        public Task<PacienteDto?> GetSoporteTrade(TradeDto trade)
        {
            return _docSopData.GetSoporteTrade(trade);
        }

    }
}
