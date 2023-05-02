using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace Service.Models
{
    public class CardModel
    {
        [BsonId]
        [DataMember]
        public MongoDB.Bson.ObjectId _id { get; set; }

        public string? cardId { get; set; }

        public string cardName { get; set; }

        public int cardLevel { get; set; }
    }
}
