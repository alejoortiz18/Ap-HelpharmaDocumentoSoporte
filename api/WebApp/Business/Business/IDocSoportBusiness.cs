using Models.Dto.DocumentoSoporteDto;

namespace Business
{
    public interface IDocSoportBusiness
    {
        Task<PacienteDto?> GetSoporte(string DCTOPRV);
    }
}
