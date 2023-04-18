﻿namespace Service.Models
{
    public class UsersDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string MythicEmpireCollectionName { get; set; } = null!;
    }
}
