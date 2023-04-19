using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace Service.Models
{
    public class Users
    {
        [BsonId]
        [DataMember]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string email { get; set; } = null!;
        public string password { get; set; } = null!;
        public string userId { get; set; } = null!;
        public string? nickName { get; set; }
        public decimal currency { get; set; } = 0;
        public List<int>? cards { get; set; }
        public List<string>? friends { get; set; }

    }
}
