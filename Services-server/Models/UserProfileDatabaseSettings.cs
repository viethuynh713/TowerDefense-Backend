namespace Service.Models
{
    public class UserProfileDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UserProfileCollectionName { get; set; } = null!;
    }
}
