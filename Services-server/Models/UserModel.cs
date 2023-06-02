using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace Service.Models
{
    public class UserModel
    {
        [BsonId]
        [DataMember]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string email { get; set; } = null!;
        public string password { get; set; } = null!;
        public string userId { get; set; } = null!;
        public string nickName { get; set; } = null!;
        public int gold { get; set; } = 0;
        
        public int rank { get; set; } = 0;
        public List<string>? cardListID { get; set; }
        public List<string>? friendListID { get; set; }

    }
}
