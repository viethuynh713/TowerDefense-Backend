namespace Service.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UserModelCollectionName { get; set; } = null!;

        public string GameSessionModelCollectionName { get; set; } = null!;

        public string CardModelCollectionName { get; set; } = null!;
    }
}
