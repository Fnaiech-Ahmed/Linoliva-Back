namespace tech_software_engineer_consultant_int_backend.Models
{
    public class ActivationDevice
    {
        public int Id { get; set; }

        // L'appareil appartient à un code d'activation
        public int CodeActivationId { get; set; }
        public CodeActivation CodeActivation { get; set; }

        // Identité de l'appareil
        public string DeviceId { get; set; }          // Identifiant unique (GUID généré par l'app)
        public string DeviceName { get; set; }        // Nom de l’appareil (EX : “PC Bureau”)
        public string OS { get; set; }                // “Windows 11”, “Android”, iOS…
        public string AppVersion { get; set; }        // Version de ton app installée

        // Statuts
        public bool IsActive { get; set; } = true;    // Désactivation distante
        public DateTime ActivationDate { get; set; }
        public DateTime LastUsage { get; set; }
    }
}
