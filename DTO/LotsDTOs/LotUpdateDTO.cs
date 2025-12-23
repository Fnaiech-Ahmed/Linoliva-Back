using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.LotsDTOs
{
    public class LotUpdateDTO
    {
        public string Reference { get; set; }
        public int IDProduit { get; set; }
        public int Quantite { get; set; }

        public DateTime Date { get; set; }


        public LotUpdateDTO() { }


        public static LotUpdateDTO FromLotEntity(Lot lot)
        {
            return new LotUpdateDTO
            {
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
                Reference = Reference,
                IDProduit = IDProduit,
                Quantite = Quantite,
                Date = Date,
            };
        }
    }
}
