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
        public string gameId { get; set; } = null!;
        public ModeGame mode { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public float totalTime { get; set; }
        public string? playerA { get; set; }
        public string? playerB { get; set; }
        public List<string>? listCardPlayerA { get; set; }
        public List<string>? listCardPlayerB { get; set; }
        public string? playerWin { get; set; }

    }
    public enum ModeGame
    {
        None,
        Adventure,
        Arena

    }
}
