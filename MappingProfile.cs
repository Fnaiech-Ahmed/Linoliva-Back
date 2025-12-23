// MappingProfiles.cs

using AutoMapper;
using tech_software_engineer_consultant_int_backend.DTO;
using tech_software_engineer_consultant_int_backend.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Echeance, EcheanceDto>();
        CreateMap<CreateEcheanceDto, Echeance>();
        CreateMap<UpdateEcheanceDto, Echeance>();
        // Ajoutez d'autres mappings au besoin
    }
}
