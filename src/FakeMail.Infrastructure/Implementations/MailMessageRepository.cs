using FakeMail.Domain.Entities.Mails;
using FakeMail.Repositories.Dtos;
using FakeMail.Repositories.Interfaces;
using FakeMail.Repositories.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeMail.Repositories.Implementations;

internal class MailMessageRepository(IOptions<MongoDbSettings> mongoDbSettings) : MongoDbBase(mongoDbSettings),
    IMailMessageRepository
{
    private const string CollectionName = "mail_messages";
    
    public async Task<MailMessage> CreateAsync(CreateMailMessageDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Message);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.ReceiverEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.SenderEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Title);

        var mailMessage = new MailMessage
        {
            Message = dto.Message,
            Title = dto.Title,
            CreatedAt = DateTime.UtcNow,
            ReceiverEmail = dto.ReceiverEmail,
            SenderEmail = dto.SenderEmail,
            Attachments = dto.Attachments
        };

        await GetDatabase()
            .GetCollection<MailMessage>(CollectionName)
            .InsertOneAsync(mailMessage, cancellationToken: cancellationToken);

        return mailMessage;
    }

    public async Task<IEnumerable<MailMessage>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await (await GetDatabase()
                .GetCollection<MailMessage>(CollectionName)
                .FindAsync(Builders<MailMessage>.Filter.Empty, cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MailMessage>> GetAsync(
        GetMailMessageDto dto,
        CancellationToken cancellationToken = default)
    {
        var filters = new List<FilterDefinition<MailMessage>>();
        
        if (dto.ReceiverEmail is not null)
        {
            filters.Add(Builders<MailMessage>.Filter.Eq(mailMessage => mailMessage.ReceiverEmail, dto.ReceiverEmail!));
        }

        if (dto.SenderEmail is not null)
        {
            filters.Add(Builders<MailMessage>.Filter.Eq(mailMessage => mailMessage.SenderEmail, dto.SenderEmail!));
        }

        var filter = Builders<MailMessage>.Filter.And(filters);

        return await (await GetDatabase()
                .GetCollection<MailMessage>(CollectionName)
                .FindAsync(filter, cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);
    }

    public async Task<MailMessage?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var filter = Builders<MailMessage>.Filter.Eq(mailMessage => mailMessage.Id, id);

        return await (await GetDatabase()
                .GetCollection<MailMessage>(CollectionName)
                .FindAsync(filter, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateMailMessageDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.ReceiverEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.SenderEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Message);

        var filter = Builders<MailMessage>.Filter.And(
            Builders<MailMessage>.Filter.Eq(mailMessage => mailMessage.ReceiverEmail, dto.ReceiverEmail),
            Builders<MailMessage>.Filter.Eq(mailMessage => mailMessage.SenderEmail, dto.SenderEmail)
        );

        var update = Builders<MailMessage>.Update
            .Set(mailMessage => mailMessage.Message, dto.Message)
            .Set(mailMessage => mailMessage.CreatedAt, dto.CreatedAt);

        await GetDatabase()
            .GetCollection<MailMessage>(CollectionName)
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var filter = Builders<MailMessage>.Filter.Eq(mailMessage => mailMessage.Id, id);
        
        await GetDatabase()
            .GetCollection<MailMessage>(CollectionName)
            .DeleteOneAsync(filter, cancellationToken: cancellationToken);
    }

    private IMongoDatabase GetDatabase() => GetDatabase("mails_db");
}