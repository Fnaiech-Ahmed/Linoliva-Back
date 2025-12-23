using TSE_Consultant_INT_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Repository;
using tech_software_engineer_consultant_int_backend.DTO.SIsDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class SIService : ISIService
    {
        private readonly ISIRepository<SI> siRepository;
        

        public SIService(ISIRepository<SI> repository)
        {
            siRepository = repository;
        }

        public async Task<bool> AddSI(SICreateDTO siCreateDTO)
        {
            SI si = siCreateDTO.ToSIEntity();
            int SIId = await siRepository.AddSI(si);
            if (SIId > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<SIDTO>> GetSIs()
        {
            List<SI> SIs = await siRepository.GetSIs();
            if (SIs == null || SIs.Count == 0)
            {
                return new List<SIDTO>();
            }
            else
            {
                List<SIDTO> SIsDTO = new List<SIDTO>();
                foreach (SI si in SIs)
                {
                    SIDTO siDTO = SIDTO.FromSIEntity(si);
                    SIsDTO.Add(siDTO);
                }
                return SIsDTO;
            }
            
        }

        public async Task<SIDTO?> GetSIById(int id)
        {
            SI? existingSI = await siRepository.GetSIById(id);

            if (existingSI == null)
            {
                return new SIDTO();
            }
            else
            {
                return SIDTO.FromSIEntity(existingSI);
            }
            
        }

        

        public async Task<bool> UpdateSI(int SIId, SIUpdateDTO SIUpdateDTO)
        {
            SI? existingSI = await siRepository.GetSIById(SIId);
           

            if (existingSI != null)
            {
                SI updatedSI = SIUpdateDTO.ToSIEntity();
                updatedSI.Id = SIId;

                existingSI.Name = updatedSI.Name;
                existingSI.Description = updatedSI.Description;
                existingSI.Adresse_Image_SI = updatedSI.Adresse_Image_SI;
                

                bool resUpdateSI = await siRepository.UpdateSI(existingSI);  // Appel de la méthode UpdateSI correcte              
                

                return resUpdateSI;

            }
            else
            {
                //MessageBox.Show("Le si est null. Impossible de mettre à jour.");
                return false;
            }
        }

        public async Task<bool> DeleteSI(int id)
        {
            return await siRepository.DeleteSI(id);
        }
    }
}
