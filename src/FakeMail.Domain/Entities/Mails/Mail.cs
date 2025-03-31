using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeMail.Domain.Entities.Mails;

public class Mail
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("email")]
    public required string Email { get; init; }
    
    [BsonElement("password")]
    public required string Password { get; init; }
    
    [BsonElement("token")]
    public required string Token { get; set; }
}