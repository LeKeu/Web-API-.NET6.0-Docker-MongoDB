using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ultimo.Models
{
    public class Cartao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Conta { get; set; }
        public string? Agencia { get; set; }
    }
}
