using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeMail.Domain.Entities.Mails;

public class MailMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();
    
    [BsonElement("sender_email")]
    public required string SenderEmail { get; init; }
    
    [BsonElement("title")]
    public required string Title { get; init; }
    
    [BsonElement("message")]
    public required string Message { get; init; }
    
    [BsonElement("receiver_email")]
    public required string ReceiverEmail { get; init; }
    
    [BsonElement("created_at")]
    public required DateTime CreatedAt { get; init; }
    
    [BsonElement("attachments")]
    public ICollection<FileAttachment> Attachments { get; init; } = new List<FileAttachment>();
}

public class FileAttachment
{
    [BsonElement("file_name")]
    public required string FileName { get; init; }
    
    [BsonElement("content_type")]
    public required string ContentType { get; init; }
    
    [BsonElement("data")]
    public required byte[] Data { get; init; }
}