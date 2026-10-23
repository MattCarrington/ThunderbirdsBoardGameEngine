namespace ThunderbirdsBoardGameEngine.GameState.DatabaseProvisioner
{
    public sealed class DatabaseProvisioningOptions
    {
        public const string SectionName = "GameState:DatabaseProvisioning";

        public string AdministratorConnectionString { get; set; } = string.Empty;

        public string RuntimeUsername { get; set; } = string.Empty;

        public string RuntimePassword { get; set; } = string.Empty;
    }
}