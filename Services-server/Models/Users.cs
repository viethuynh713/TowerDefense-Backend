using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Service.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string userId { get; set; } = null!;

        public string? nickName { get; set; }

        public string email { get; set; } = null!;

        public decimal currency { get; set; } = 0;

        public List<int>? cards { get; set; }

    }
}
