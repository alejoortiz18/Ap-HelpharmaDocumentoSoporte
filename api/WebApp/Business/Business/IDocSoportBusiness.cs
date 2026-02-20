using Models.Dto.DocumentoSoporteDto;
using Models.Dto.Request;

namespace Business
{
    public interface IDocSoportBusiness
    {
        Task<PacienteDto?> GetSoporte(string DCTOPRV);
        Task<PacienteDto?> GetSoporteTrade(TradeDto trade);
    }
}
