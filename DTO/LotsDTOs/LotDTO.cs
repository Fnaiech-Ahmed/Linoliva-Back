using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.LotsDTOs
{
    public class LotDTO
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public int IDProduit { get; set; }
        public int Quantite { get; set; }

        public DateTime Date { get; set; }


        public LotDTO() { }


        public static LotDTO FromLotEntity(Lot lot)
        {
            return new LotDTO { 
                Id = lot.Id,
                Reference = lot.Reference, 
                IDProduit = lot.IDProduit,
                Quantite = lot.Quantite,
                Date = lot.Date,
            };
        }


        public Lot ToLotEntity()
        {
            return new Lot
            {
                Id = Id,
                Reference = Reference,
                IDProduit = IDProduit,
                Quantite = Quantite,
                Date = Date,
            };
        }
    }
}
