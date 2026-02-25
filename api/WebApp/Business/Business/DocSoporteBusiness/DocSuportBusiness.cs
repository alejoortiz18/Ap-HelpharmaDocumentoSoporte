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
        private readonly IDocumentoSoporteOfimaData _docSopData;
        private readonly IDocumentoSoporteDWData _docSopDWData;


        public DocSuportBusiness(IDocumentoSoporteOfimaData doc, IDocumentoSoporteDWData docSopDWData)
        {
            _docSopData = doc;
            _docSopDWData = docSopDWData;
        }

        public Task<PacienteDto?> GetSoporte(SoporteDto request)
        {
            var soporte = request.Soporte;

            string origen = char.IsDigit(soporte[2]) ? "ofima" : "DW";

            if(origen == "ofima")
            {

                return _docSopData.GetSoporteOfima(soporte);
            }
            else
            {
                string soporteDW = soporte.Substring(0, 3);
                int noEntrega = int.Parse(soporte.Substring(3));
                return _docSopDWData.GetSoporteDW(soporteDW,noEntrega);
            }

        }

        public Task<PacienteDto?> GetSoporteTrade(TradeDto trade)
        {
            return _docSopData.GetSoporteTradeOfima(trade);
        }

    }
}
