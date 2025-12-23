using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.LotsDTOs
{
    public class LotCreateDTO
    {
        public string Reference { get; set; }
        public int IDProduit { get; set; }
        public int Quantite { get; set; }

        public DateTime Date { get; set; }


        public LotCreateDTO() { }


        public static LotCreateDTO FromLotEntity(Lot lot)
        {
            return new LotCreateDTO
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
