using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace Service.Models
{
    public class GameSessionModel
    {

        [BsonId]
        [DataMember]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string sessionId { get; set; } = null!;
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public float totalTime { get; set; }
        public string? playerA { get; set; }
        public string? playerB { get; set; }
        public List<int>? listCardPlayerA { get; set; }
        public List<int>? listCardPlayerB { get; set; }
        public string? playerWin { get; set; }

    }
}
