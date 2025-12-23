using tech_software_engineer_consultant_int_backend.DTO.BLDTO;
using tech_software_engineer_consultant_int_backend.DTO.FacturesDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IFactureService
    {
        Task<bool> AddFacture(FactureCreateDTO factureCreateDTO);
        Task<string> GenerateNextReference();



        Task<List<FactureDTO>> GetFactures();
        Task<List<FactureDTO>> GetListeFacturesByOwner(string ownerId);
        Task<FactureDTO?> GetFactureById(int id);
        Task<FactureDTO?> GetFactureByReference(string reference);



        Task<bool> UpdateFacture(int factureId, FactureUpdateDTO facture);



        Task<bool> DeleteFacture(int id);
        byte[] Generate(FacturePdfDTO request);



        decimal CalculerMontantTotalTTC(Facture facture);
        decimal CalculerMontantTotalHT(Facture facture);
    }
}
