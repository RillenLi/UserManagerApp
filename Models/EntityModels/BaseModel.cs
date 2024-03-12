using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UserManagerApp.Models.EntityModels
{
    public class BaseMongoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}
