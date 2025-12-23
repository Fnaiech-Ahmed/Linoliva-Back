using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.DTO.BonsDeSortieDTOs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class BonDeSortieService : IBonDeSortieService
    {
        private readonly IBonDeSortieRepository bonDeSortieRepository;
        private readonly ISequenceRepository<Sequence> sequenceRepository;

        public BonDeSortieService(IBonDeSortieRepository repository, ISequenceRepository<Sequence> _sequenceRepository)
        {
            bonDeSortieRepository = repository;
            sequenceRepository = _sequenceRepository;
        }

        public async Task<bool> AddBonDeSortie(BonDeSortieCreateDTO bonDeSortieCreateDTO)
        {
            BonDeSortie bonDeSortie = bonDeSortieCreateDTO.ToBSEntity();

            string RefBS = this.GenerateNextRef();
            bonDeSortie.ReferenceBS = RefBS;

            return await bonDeSortieRepository.AddBonDeSortie(bonDeSortie);
        }

        public string GenerateNextRef()
        {
            var sequence = sequenceRepository.GetSequenceByName("BonDeSortie");
            

            if (sequence == null)
            {
                sequence = new Sequence { Name = "BonDeSortie", NextValue = 1 };

                sequenceRepository.AddSequence(sequence);
            }
            else
            {
                sequence.NextValue++;

                sequenceRepository.UpdateSequence(sequence);
            }

            return $"BSRK-{sequence.NextValue:D5}";
        }

        public async Task<List<BonDeSortieDTO>> GetBonDeSorties()
        {
            List<BonDeSortie> ListBS = await bonDeSortieRepository.GetBonDeSorties();
            if (ListBS != null)
            {
                List<BonDeSortieDTO> bonDeSortieDTOs = new List<BonDeSortieDTO>();
                foreach(BonDeSortie bonDeSortie in ListBS)
                {
                    bonDeSortieDTOs.Add(BonDeSortieDTO.FromBSEntity(bonDeSortie));
                }
                return bonDeSortieDTOs;
            }
            else 
            {
                return new List<BonDeSortieDTO>();
            }
        }

        public async Task<BonDeSortieDTO?> GetBonDeSortieById(int id)
        {
            BonDeSortie? existingBonDeSortie = await bonDeSortieRepository.GetBonDeSortieById(id);
            if (existingBonDeSortie != null)
                return BonDeSortieDTO.FromBSEntity(existingBonDeSortie);
            else 
                return null;
        }

        public async Task<bool> UpdateBonDeSortie(int bonDeSortieId, BonDeSortieUpdateDTO bonDeSortieUpdateDTO)
        {
            BonDeSortie? existingBonDeSortie = await bonDeSortieRepository.GetBonDeSortieById(bonDeSortieId);

            if (existingBonDeSortie == null) { 
                return false; 
            }

            existingBonDeSortie.ListeFactures = bonDeSortieUpdateDTO.ListeFactures;
            existingBonDeSortie.MatriculeDeVoiture = bonDeSortieUpdateDTO.MatriculeDeVoiture;
            existingBonDeSortie.DateDebutCirculation = bonDeSortieUpdateDTO.DateDebutCirculation;
            existingBonDeSortie.DateFinCirculation = bonDeSortieUpdateDTO.DateFinCirculation;

            bool resultatUpdate = await bonDeSortieRepository.UpdateBonDeSortie(existingBonDeSortie);
            return resultatUpdate;
        }

        public async Task<bool> DeleteBonDeSortie(int id)
        {
            return await bonDeSortieRepository.DeleteBonDeSortie(id);
        }

        public async Task<BonDeSortieDTO?> GetBonDeSortieByReference(string reference)
        {
            BonDeSortie? existingBonDeLivraison = await bonDeSortieRepository.GetBonDeSortieByReference(reference);
            if (existingBonDeLivraison == null) 
            { 
                return null; 
            }
            return BonDeSortieDTO.FromBSEntity(existingBonDeLivraison);
        }
    }
}
