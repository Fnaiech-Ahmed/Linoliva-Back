using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.DTO.FacturesDTOs;
using NuGet.Protocol.Core.Types;
using tech_software_engineer_consultant_int_backend.DTO.BLDTO;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Globalization;
using IContainer = QuestPDF.Infrastructure.IContainer;
using ImageScaling = QuestPDF.Infrastructure.ImageScaling;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class FactureService : IFactureService
    {
        private readonly IFactureRepository factureRepository;
        private readonly ISequenceRepository<Sequence> sequenceRepository;
        private readonly IBLService bLService;

        public FactureService(IFactureRepository repository, ISequenceRepository<Sequence> _sequenceRepository, IBLService _bLService)
        {
            factureRepository = repository;
            sequenceRepository = _sequenceRepository;
            bLService = _bLService;
        }

        public async Task<bool> AddFacture(FactureCreateDTO factureCreateDTO)
        {
            // Convertir FactureCreateDTO en Facture
            Facture facture = factureCreateDTO.ToFactureEntity();

            // Récupérer les BonDeLivraison à partir des références
            var bonDeLivraisons = await bLService.GetBonDeLivraisonsByRefs(factureCreateDTO.ListeRefsBLs);
            facture.ListeBL = bonDeLivraisons;

            // Générer la référence de la facture
            string referenceFacture = await this.GenerateNextReference();
            facture.ReferenceFacture = referenceFacture;

            // Calculer les montants
            facture.MontantHTFacture = this.CalculerMontantTotalHT(facture);
            facture.MontantTTCFacture = this.CalculerMontantTotalTTC(facture);

            // Ajouter la facture à la base de données
            return await factureRepository.AddFacture(facture);
        }


        public async Task<string> GenerateNextReference()
        {
            var sequence = sequenceRepository.GetSequenceByName("Facture");
            

            if (sequence == null)
            {
                sequence = new Sequence { Name = "Facture", NextValue = 1 };
                await sequenceRepository.AddSequence(sequence);
            }
            else
            {
                sequence.NextValue++;
                await sequenceRepository.UpdateSequence(sequence);
            }

            return $"FRK-{sequence.NextValue:D5}";
        }




        public async Task<List<FactureDTO>> GetFactures()
        {
            List<Facture> listFactures = await factureRepository.GetAllFactures();

            if (listFactures == null)
                return new List<FactureDTO>();
            else
            {
                List<FactureDTO> factureDTOs = new List<FactureDTO>();
                foreach (Facture facture in listFactures)
                {
                    factureDTOs.Add(FactureDTO.FromFactureEntity(facture));
                }
                return factureDTOs;
            }            
        }

        public async Task<List<FactureDTO>> GetListeFacturesByOwner(string ownerId)
        {
            List<Facture> factures = await factureRepository.GetListeFacturesByOwner(ownerId);

            List<FactureDTO> factureDTOs = factures.Select(facture => FactureDTO.FromFactureEntity(facture)).ToList();
            return factureDTOs;
        }

        public async Task<FactureDTO?> GetFactureById(int id)
        {
            Facture? existingFacture = await factureRepository.GetFactureById(id);
            if (existingFacture != null)
            {
                FactureDTO.FromFactureEntity(existingFacture);
            }
            return new FactureDTO();
        }
        
        public async Task<FactureDTO?> GetFactureByReference(string reference)
        {
            Facture? existingFacture = await factureRepository.GetFactureByReference(reference);
            if (existingFacture != null)
            {
                return FactureDTO.FromFactureEntity(existingFacture);
            }
            else
            { 
                return null; 
            }            
        }



        public async Task<bool> UpdateFacture(int factureId, FactureUpdateDTO factureUpdateDTO)
        {
            Facture? existingFacture = await factureRepository.GetFactureById(factureId);

            if (existingFacture != null)
            {
                // Convertir FactureUpdateDTO en Facture
                Facture facture = factureUpdateDTO.ToFactureEntity();                

                // Récupérer les BonDeLivraison à partir des références
                var bonDeLivraisons = await bLService.GetBonDeLivraisonsByRefs(factureUpdateDTO.ListeRefsBLs);
                existingFacture.ListeBL = bonDeLivraisons;

                existingFacture.ReferenceFacture = existingFacture.ReferenceFacture;

                existingFacture.TitreClient = facture.TitreClient;
                existingFacture.NomClient = facture.NomClient;
                existingFacture.AdresseClient = facture.AdresseClient;
                existingFacture.MFClient = facture.MFClient;
                existingFacture.GSMClient = facture.GSMClient;

                
                existingFacture.TVA = facture.TVA;
                existingFacture.Remise = facture.Remise;

                existingFacture.MontantHTFacture = this.CalculerMontantTotalHT(facture);
                existingFacture.MontantTTCFacture = this.CalculerMontantTotalTTC(facture);

                await factureRepository.UpdateFacture(existingFacture);
                return true;
            }
            else
            {
                // Gérer le cas où la facture n'existe pas ici.
                //MessageBox.Show("Facture inexistante avec l'id: " + factureId);
                return false;
            }
        }





        public async Task<bool> DeleteFacture(int id)
        {
            return await factureRepository.DeleteFacture(id);
        }
        


        public decimal CalculerMontantTotalTTC(Facture facture)
        {
            if (facture != null)
            {
                decimal AmountTotal_TTC = 0;

                List<BonDeLivraison> ListeBLs = facture.ListeBL;

                if (ListeBLs != null)
                {
                    var BLsAmountsTTCList = ListeBLs.Select(t => t.MontantTotalTTCBL).ToList();

                    foreach (var amountTTC in BLsAmountsTTCList)
                    {
                        AmountTotal_TTC = AmountTotal_TTC + amountTTC;
                    }
                    AmountTotal_TTC = AmountTotal_TTC - ((AmountTotal_TTC * facture.Remise) / 100) + 1;

                    return AmountTotal_TTC; // Retourne le montant total TTC calculé
                }
                else
                {
                    //MessageBox.Show("liste bl vide.");
                    return 0; // Aucune BL, retourne zéro
                }
            }
            //MessageBox.Show("facture vide.");
            return 0; // Facture nulle, retourne zéro
        }

        public decimal CalculerMontantTotalHT(Facture facture)
        {
            if (facture != null)
            {
                decimal AmountTotal_HT = 0;

                List<BonDeLivraison> ListeBLs = facture.ListeBL;

                if (ListeBLs != null)
                {
                    var BLsAmountsHTList = ListeBLs.Select(t => (t.MontantTotalHTBL)).ToList();

                    foreach (decimal BLAmountHT in BLsAmountsHTList)
                    {
                        AmountTotal_HT = AmountTotal_HT + BLAmountHT;
                    }

                    return AmountTotal_HT; // Retourne le montant total HT calculé
                }
                else
                {
                    return 0; // Aucune BL, retourne zéro
                }
            }

            return 0; // Facture nulle, retourne zéro
        }

        public byte[] Generate(FacturePdfDTO request)
        {
            byte[] logoData = File.ReadAllBytes("Images/logo.png");
            var mainColor = "#F0F0F0";
            request.TVA = 0;
            request.IncludeFodec = false;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    // ---------- HEADER ----------
                    page.Header()
                        .Background(mainColor)
                        .PaddingVertical(12)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .AlignMiddle()
                                .Height(40)
                                .Image(logoData, ImageScaling.FitHeight);

                            row.RelativeItem()
                                .AlignRight()
                                .AlignMiddle()
                                .PaddingRight(10)
                                .Text(request.CompanyName)
                                .FontSize(12)
                                .FontColor("#000000");
                        });

                    // ---------- CONTENT ----------
                    page.Content().Column(col =>
                    {
                        col.Spacing(18);

                        col.Item().PaddingTop(25)
                            .AlignCenter()
                            .Text("FACTURE")
                            .FontSize(22)
                            .Bold();

                        // ---------- INFOS ----------
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text(request.CompanyName).Bold().FontSize(13);
                                c.Item().Text(request.CompanyAddress);
                                c.Item().Text(request.CompanyPhone);
                            });

                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text($"N° Facture : {request.ReferenceFacture}").Bold();
                                c.Item().Text($"Date : {request.DateEmission:dd/MM/yyyy}");
                                c.Item().Text($"Client : {request.TitreClient} {request.NomClient}");
                                c.Item().Text($"Adresse : {request.AdresseClient}");
                                c.Item().Text(request.ClientCityZip);
                                c.Item().Text($"MF : {request.MFClient}");
                            });
                        });

                        col.Item().PaddingVertical(10)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);

                        // ---------- BL + COMMANDES ----------
                        foreach (var bl in request.ListeBL)
                        {
                            col.Item().PaddingTop(10)
                                .Text($"Bon de livraison : {bl.Reference}")
                                .Bold();

                            col.Item().Border(1)
                                .BorderColor(Colors.Grey.Lighten1)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(3);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                        c.RelativeColumn(2);
                                    });

                                    table.Header(h =>
                                    {
                                        h.Cell().Element(Header).Text("Commande").Bold();
                                        h.Cell().Element(Header).Text("Acheteur").Bold();
                                        h.Cell().Element(Header).Text("Adresse").Bold();
                                        h.Cell().Element(Header).Text("Mf").Bold();
                                        h.Cell().Element(Header).Text("Total").Bold();
                                    });
                                    bool hasMultipleRows = bl.Commandes.Count > 1;


                                    foreach (var cmd in bl.Commandes)
                                    {
                                        table.Cell().BorderBottom(hasMultipleRows ? 1 : 0).Text(cmd.ReferenceCommande);
                                        table.Cell().BorderBottom(hasMultipleRows ? 1 : 0).AlignRight().Text($"{cmd.NomAcheteur}");
                                        table.Cell().BorderBottom(hasMultipleRows ? 1 : 0).AlignRight().Text($"{cmd.AdresseAcheteur:0.000}");
                                        table.Cell().BorderBottom(hasMultipleRows ? 1 : 0).AlignRight().Text($"{cmd.MFAcheteur:0.000}");
                                        table.Cell().BorderBottom(hasMultipleRows ? 1 : 0).AlignRight().Text($"{cmd.MontantTotalHT:0.000} DT");
                                    }
                                });

                            col.Item().AlignRight()
                                .Text($"Total : {bl.MontantTotalHTBL:0.000} DT")
                                .Bold();
                        }

                        col.Item().PaddingVertical(10)
                            .LineHorizontal(1);

                        // ---------- TOTAL FACTURE ----------
                        col.Item()
                            .Background(mainColor)
                            .Padding(12)
                            .AlignRight()
                            .Column(c =>
                            {
                                /*c.Item().Text($"Montant HT : {request.MontantHTFacture:0.000} DT");
                                c.Item().Text($"TVA : {request.TVA:0.000} DT");

                                if (request.Remise > 0)
                                    c.Item().Text($"Remise : {request.Remise:0.000} DT");

                                if (request.IncludeFodec)
                                    c.Item().Text("FODEC inclus");*/

                                c.Item().Text($"TOTAL : {request.MontantHTFacture:0.000} DT")
                                    .FontSize(14)
                                    .Bold();
                            });

                        // ---------- NOTES ----------
                        if (!string.IsNullOrWhiteSpace(request.Notes))
                            col.Item().PaddingTop(15)
                                .Text(request.Notes)
                                .Italic();
                    });

                    // ---------- FOOTER ----------
                    page.Footer()
                        .AlignCenter()
                        .PaddingVertical(15)
                        .Text(text =>
                        {
                            text.Span("Page ").FontSize(10).FontColor(Colors.Grey.Medium);
                            text.CurrentPageNumber().FontSize(10).FontColor(Colors.Grey.Medium);
                            text.Span(" / ").FontSize(10).FontColor(Colors.Grey.Medium);
                            text.TotalPages().FontSize(10).FontColor(Colors.Grey.Medium);
                            text.Span(" - Merci pour votre confiance.").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                });
            });

            return document.GeneratePdf();
        }



        static IContainer Header(IContainer container) =>
            container.Background(Colors.Grey.Lighten2)
                     .Padding(6)
                     .BorderBottom(1)
                     .BorderColor(Colors.Grey.Lighten1)
                     .ShowOnce();

        static IContainer Cell(IContainer container) =>
            container.PaddingVertical(4)
                     .BorderBottom(1)
                     .BorderColor(Colors.Grey.Lighten3);
    }

}

