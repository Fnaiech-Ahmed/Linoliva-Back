using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class BonDeSortie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ReferenceBS { get; set; }

        [NotMapped]
        public List<Facture> ListeFactures { get; set; }

        public string MatriculeDeVoiture { get; set; }
        public DateTime DateDebutCirculation { get; set; }
        public DateTime DateFinCirculation { get; set; }

        // Property for serialized ListeFactures
        public string ListeFacturesSerialized
        {
            get => SerializeListeFactures(ListeFactures);
            set => ListeFactures = string.IsNullOrEmpty(value) ? new List<Facture>() : DeserializeListeFactures(value);
        }


        public BonDeSortie()
        {
            ListeFactures = new List<Facture>();
        }

        public string SerializeListeFactures(List<Facture> listeFactures)
        {
            return JsonConvert.SerializeObject(listeFactures);
        }

        public List<Facture> DeserializeListeFactures(string json)
        {
            return JsonConvert.DeserializeObject<List<Facture>>(json);
        }
    }
}
