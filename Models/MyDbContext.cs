using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class MyDbContext : IdentityDbContext<User, Role, int>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<LoginRequest> LoginRequests { get; set; }
        

        public DbSet<RolePolicy> RolePolicies { get; set; }
        public DbSet<RolePolicyRole> RolePolicyRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        public DbSet<Portefeuille> Portefeuilles { get; set; }
        public DbSet<UserPortefeuilleAssociation> UserPortefeuilleAssociations { get; set; }
        public DbSet<Dossier> Dossiers { get; set; }
        public DbSet<CodeActivation> CodeActivations { get; set; } // Pour Table Per Hierarchy (TPH)
        public DbSet<Abonnement> Abonnements { get; set; } // Optionnel, si vous voulez accéder directement aux Abonnements
        public DbSet<Licence> Licences { get; set; } // Optionnel, si vous voulez accéder directement aux Licences
        public DbSet<ActivationDevice> ActivationDevices { get; set; }
        public DbSet<Echeance> Echeances { get; set; }
        public DbSet<Impaye> Impayes { get; set; }



        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rapport> Rapports { get; set; }
        public DbSet<Message> Messages { get; set; }
        
        

        
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroupAssociation> UserGroupAssociations { get; set; }


        public DbSet<CompteBancaire> CompteBancaires { get; set; }


        public DbSet<Sequence> Sequences { get; set; }
        




        public DbSet<SI> SIs { get; set; }


        public DbSet<Product> Products { get; set; }
        public DbSet<InventaireProduit> InventaireProduits { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<LotsTransactions> LotsTransactions { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<BonDeLivraison> BonDeLivraisons { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<BonDeSortie> BonDeSorties { get; set; }

        public DbSet<TransactionFinancière> TransactionsFinancières { get; set; }
        public DbSet<Depense> Depenses { get; set; }
        public DbSet<Recette> Recettes { get; set; }
        public DbSet<RapportJournalier> RapportsJournalier { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ajout de cette ligne pour configurer les entités Identity correctement


            // Renommer les tables Identity
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");

            // Vous pouvez également renommer les tables des revendications, des rôles, etc.
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            // Configuration de la table UserRoles pour IdentityUserRole
            modelBuilder.Entity<IdentityUserRole<int>>()
                .ToTable("UserRoles")
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);




            // Configuration de la clé primaire composite pour RolePolicyRole
            modelBuilder.Entity<RolePolicyRole>()
                .HasKey(rpr => new { rpr.RoleId, rpr.RolePolicyId });

            // Configuration des relations
            modelBuilder.Entity<RolePolicyRole>()
                .HasOne(rpr => rpr.Role)
                .WithMany(r => r.RolePolicyRoles)
                .HasForeignKey(rpr => rpr.RoleId);

            modelBuilder.Entity<RolePolicyRole>()
                .HasOne(rpr => rpr.RolePolicy)
                .WithMany(rp => rp.RolePolicyRoles)
                .HasForeignKey(rpr => rpr.RolePolicyId);




            modelBuilder.Entity<User>()
                .HasMany(u => u.ListeNotifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ListeUserPortefeuilles)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserGroupAssociation>()
                .HasKey(uga => new { uga.UserId, uga.GroupId });

            modelBuilder.Entity<UserGroupAssociation>()
                .HasOne(uga => uga.User)
                .WithMany(u => u.UserGroupAssociations)
                .HasForeignKey(uga => uga.UserId);

            modelBuilder.Entity<UserGroupAssociation>()
                .HasOne(uga => uga.Group)
                .WithMany(g => g.UserGroupAssociations)
                .HasForeignKey(uga => uga.GroupId);




            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReceiverGroup)
                .WithMany(g => g.MessagesList)
                .HasForeignKey(m => m.ReceiverGroupId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId);




            modelBuilder.Entity<Portefeuille>()
                .HasMany(p => p.UserPortefeuilleAssociations)
                .WithOne(up => up.Portefeuille)
                .HasForeignKey(up => up.PortefeuilleId);

            modelBuilder.Entity<Portefeuille>()
                .HasMany(p => p.ListeDossiers)
                .WithOne(d => d.Portefeuille)
                .HasForeignKey(d => d.PortefeuilleId);

            modelBuilder.Entity<Portefeuille>()
                .HasMany(p => p.ListeRapports)
                .WithOne(r => r.Portefeuille)
                .HasForeignKey(r => r.PortefeuilleId);




            modelBuilder.Entity<Dossier>()
                 .HasMany(d => d.ListeImpayes)
                 .WithOne(i => i.Dossier)
                 .HasForeignKey(i => i.DossierId);


            /*modelBuilder.Entity<Abonnement>()
                .HasOne(a => a.Dossier)
                .WithMany(d => d.ListeAbonnements)
                .HasForeignKey(a => a.DossierId)
                .OnDelete(DeleteBehavior.SetNull);*/

            // Configuration de l'entité Dossier
            /*modelBuilder.Entity<Dossier>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.HasMany(d => d.ListeAbonnements)
                    .WithOne(a => a.Dossier)
                    .HasForeignKey(a => a.DossierId);
            });*/

            // TPH pour CodeActivation / Abonnement / Licence
            modelBuilder.Entity<CodeActivation>()
                .HasDiscriminator<string>("Type") // Nom de la colonne discriminante
                .HasValue<Abonnement>("Abonnement")
                .HasValue<Licence>("Licence");

            // ActivationDevice
            modelBuilder.Entity<ActivationDevice>(entity =>
            {
                entity.ToTable("ActivationDevices");

                entity.HasKey(d => d.Id);

                entity.Property(d => d.DeviceId).HasMaxLength(200).IsRequired();
                entity.Property(d => d.DeviceName).HasMaxLength(200);
                entity.Property(d => d.OS).HasMaxLength(100);
                entity.Property(d => d.AppVersion).HasMaxLength(50);

                entity.HasOne(d => d.CodeActivation)
                    .WithMany(c => c.ActivationDevices)
                    .HasForeignKey(d => d.CodeActivationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });




            modelBuilder.Entity<CompteBancaire>()
                .Property(c => c.SoldeDisponible)
                .HasColumnType("decimal(18, 5)"); // Adjust precision and scale as needed



            modelBuilder.Entity<Echeance>()
                .HasOne(e => e.Impaye)
                .WithOne(i => i.Echeance)
                .HasForeignKey<Impaye>(i => i.EcheanceId)
                .IsRequired(false);


            modelBuilder.Entity<BonDeLivraison>()
                .Property(b => b.MontantTotalHTBL)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<BonDeLivraison>()
                .Property(b => b.MontantTotalTTCBL)
                .HasPrecision(18, 3);



            modelBuilder.Entity<Facture>()
                .Ignore(f => f.ListeBL)
                .Property(f => f.ListeBLSerialized);



            modelBuilder.Entity<BonDeSortie>()
                .Ignore(b => b.ListeFactures)
                .Property(b => b.ListeFacturesSerialized);

            // Configuration Many-to-Many entre Role et RolePolicy
            /*modelBuilder.Entity<RolePolicy>()
                .HasMany(rp => rp.Roles)
                .WithMany(r => r.RolePolicies)
                .UsingEntity(j => j.ToTable("RolePolicyRoles"));*/
        }
    }
}
