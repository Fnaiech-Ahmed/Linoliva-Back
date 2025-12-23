using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.BLDTO;
using tech_software_engineer_consultant_int_backend.Repositories;
using System.Collections.Generic;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class BLService : IBLService
    {
        private readonly IBLRepository blRepository;
        private readonly ISequenceRepository<Sequence> sequenceRepository;

        public BLService(IBLRepository repository, ISequenceRepository<Sequence> _sequenceRepository)
        {
            blRepository = repository;
            sequenceRepository = _sequenceRepository;
        }

        public async Task<bool> AddBonDeLivraison(BLCreateDTO bonDeLivraisonDTO)
        {
            BonDeLivraison bonDeLivraison = bonDeLivraisonDTO.ToBonDeLivraisonEntity();

            // Génération de la référence et mise à jour de la séquence de façon synchrone
            string RefBL;
            lock (sequenceRepository)  // Assure qu'aucun autre thread ne modifie la séquence en même temps
            {
                RefBL = this.GenerateNextRef();  // reste synchrone
            }
            bonDeLivraison.Reference = RefBL;

            bonDeLivraison.MontantTotalHTBL = this.CalculerMontantTotalHT(bonDeLivraison);
            bonDeLivraison.NetHT = this.CalculerNetHT(bonDeLivraison);
            bonDeLivraison.MontantTotalTTCBL = this.CalculerMontantTotalTTC(bonDeLivraison);

            // Ajout du Bon de Livraison (asynchrone)
            bool resultAjout = await blRepository.AddBonDeLivraison(bonDeLivraison);

            return resultAjout;
        }


        public string GenerateNextRef()
        {
            var sequence = sequenceRepository.GetSequenceByName("BonDeLivraison");
            

            if (sequence == null)
            {
                sequence = new Sequence { Name = "BonDeLivraison", NextValue = 1 };

                sequenceRepository.AddSequence(sequence);
            }
            else
            {
                sequence.NextValue++;

                sequenceRepository.UpdateSequence(sequence);
            }

            return $"BLTSECINT-{sequence.NextValue:D5}";
        }




        public async Task<List<BLDTO>> GetBonDeLivraisons()
        {
            List<BonDeLivraison> ListeBLs = await blRepository.GetBonDeLivraisons();

            if (ListeBLs == null)
                return new List<BLDTO> { };
            else
            {
                List<BLDTO> bLDTOs = new List<BLDTO>();
                foreach (BonDeLivraison item in ListeBLs)
                {
                    BLDTO bLDTO = BLDTO.FromBonDeLivraisonEntity(item);
                    bLDTOs.Add(bLDTO);
                }
                return bLDTOs;
            }
        }


        public async Task<List<BLDTO>> GetListeBonsDeLivraisonByOwner(string ownerId)
        {
            List<BonDeLivraison> bonDeLivraisons = await blRepository.GetListeBonsDeLivraisonByOwner(ownerId);

            List<BLDTO> bonDeLivraisonDTOs = bonDeLivraisons.Select(bl => BLDTO.FromBonDeLivraisonEntity(bl)).ToList();
            return bonDeLivraisonDTOs;
        }


        // Méthode pour récupérer les BonDeLivraison par leurs références
        public async Task<List<BonDeLivraison>> GetBonDeLivraisonsByRefs(List<string> refsBLs)
        {
            if (refsBLs == null || !refsBLs.Any())
            {
                return new List<BonDeLivraison>();
            }

            return await blRepository.GetByReferences(refsBLs);
        }

        public async Task<BLDTO?> GetBonDeLivraisonById(int id)
        {
            BonDeLivraison? existingBonDeLivraison = await blRepository.GetBonDeLivraisonById(id);
            if (existingBonDeLivraison == null) 
                return null;
            else
                return BLDTO.FromBonDeLivraisonEntity(existingBonDeLivraison);
        }

        public async Task<BonDeLivraison?> GetBonDeLivraisonByIdAndOwner(int id, string ownerId)
        {
            return await blRepository.GetBonDeLivraisonByIdAndOwner(id, ownerId);
        }

        public async Task<BonDeLivraison?> GetBonDeLivraisonByReferenceAndOwner(string reference, string ownerId)
        {
            return await blRepository.GetBonDeLivraisonByReferenceAndOwner(reference, ownerId);
        }





        public async Task<bool> UpdateBonDeLivraison(int bonDeLivraisonId, BLUpdateDTO bonDeLivraisonUpdateDTO)
        {
            BonDeLivraison? existingBonDeLivraison = await blRepository.GetBonDeLivraisonById(bonDeLivraisonId);

            if (existingBonDeLivraison != null)
            {
                BonDeLivraison bonDeLivraison = bonDeLivraisonUpdateDTO.ToBonDeLivraisonEntity();
                existingBonDeLivraison.TitreClient = bonDeLivraison.TitreClient;
                existingBonDeLivraison.NomClient = bonDeLivraison.NomClient; 
                existingBonDeLivraison.AdresseClient= bonDeLivraison.AdresseClient; 
                existingBonDeLivraison.MFClient = bonDeLivraison.MFClient;
                existingBonDeLivraison.GSMClient = bonDeLivraison.GSMClient;
                
                existingBonDeLivraison.Commandes = bonDeLivraison.Commandes;
                existingBonDeLivraison.TVA = bonDeLivraison.TVA;
                existingBonDeLivraison.RemiseBL = bonDeLivraison.RemiseBL;
                
                existingBonDeLivraison.MontantTotalHTBL = this.CalculerMontantTotalHT(bonDeLivraison); 

                existingBonDeLivraison.NetHT = this.CalculerNetHT(existingBonDeLivraison);                              
                
                existingBonDeLivraison.MontantTotalTTCBL = this.CalculerMontantTotalTTC(existingBonDeLivraison);

                //MessageBox.Show("Service::: Le MT. Net HTBL de " + existingBonDeLivraison.TitreClient.ToString() + " " + bonDeLivraison.NomClient + " est: " + existingBonDeLivraison.NetHT.ToString() + ". MTTTC: " + existingBonDeLivraison.MontantTotalTTCBL.ToString());

                bool resultUpdate = await blRepository.UpdateBonDeLivraison(existingBonDeLivraison);
                return resultUpdate;
            }
            else
            {
                // Vous pouvez gérer le cas où le bon de livraison n'existe pas ici.
                //MessageBox.Show("BL inexistante avec l'id: " +  bonDeLivraisonId);
                return false;
            }
        }

        public async Task<bool> DeleteBonDeLivraison(int id)
        {
            bool resultatSuppBL = await blRepository.DeleteBonDeLivraison(id);
            return resultatSuppBL;
        }




        public async Task<BLDTO?> GetBonDeLivraisonByReference(string reference)
        {
            BonDeLivraison? existingBL = await blRepository.GetBonDeLivraisonByReference(reference);
            if (existingBL == null)
            {
                return null;
            }
            else
            {
                return BLDTO.FromBonDeLivraisonEntity(existingBL);
            }
            
        }





        public decimal CalculerMontantTotalTTC(BonDeLivraison bonDeLivraison)
        {
            if (bonDeLivraison != null)
            {

                List<Commande> ListeCommandes = bonDeLivraison.Commandes;

                if (ListeCommandes != null)
                {
                    decimal AmountTotal_TTC = (bonDeLivraison.TVA * bonDeLivraison.NetHT / 100) + bonDeLivraison.NetHT;                    

                    return AmountTotal_TTC; // Retourne le montant total TTC calculé
                }
                else
                {
                    //MessageBox.Show("liste commande vide.");
                    return 0; // Aucune commande, retourne zéro
                }
            }
            //MessageBox.Show("bon de livraison vide.");
            return 0; // bon de livraison nul, retourne zéro
        }

        public decimal CalculerMontantTotalHT(BonDeLivraison bonDeLivraison)
        {
            if (bonDeLivraison != null)
            {
                decimal AmountTotal_HT = 0;

                List<Commande> ListeCommandes = bonDeLivraison.Commandes;

                if (ListeCommandes != null)
                {
                    var CommandesAmountsHTList = ListeCommandes.Select(t => (t.MontantTotalHT)).ToList();

                    foreach (decimal CommandeAmountHT in CommandesAmountsHTList)
                    {
                        AmountTotal_HT = AmountTotal_HT + CommandeAmountHT;
                    }

                    return AmountTotal_HT; // Retourne le montant total HT calculé
                }
                else
                {
                    return 0; // Aucune Commande, retourne zéro
                }
            }

            return 0; // Bon De Livraison nul, retourne zéro
        }


        public decimal CalculerNetHT(BonDeLivraison bonDeLivraison)
        {
            if (bonDeLivraison != null)
            {
                decimal AmountNetHT;

                AmountNetHT = (bonDeLivraison.MontantTotalHTBL - (bonDeLivraison.RemiseBL * bonDeLivraison.MontantTotalHTBL) /100);

                return AmountNetHT;            
            }

            //MessageBox.Show("Liste des commandes de bon De Livraison Vide.");
            return 0; // Bon De Livraison nul, retourne zéro
        }

    }
}
