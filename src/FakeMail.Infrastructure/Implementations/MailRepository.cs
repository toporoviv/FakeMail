using System.Security.Cryptography;
using System.Text;
using FakeMail.Domain.Entities.Mails;
using FakeMail.Repositories.Dtos;
using FakeMail.Repositories.Interfaces;
using FakeMail.Repositories.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeMail.Repositories.Implementations;

internal class MailRepository(IOptions<MongoDbSettings> mongoDbSettings) : MongoDbBase(mongoDbSettings), IMailRepository
{
    private const string CollectionName = "mails";
    
    public async Task<Mail> CreateAsync(CreateMailDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password);

        if (await GetAsync(dto.Email, cancellationToken) is not null)
        {
            throw new Exception($"Запись с email={dto.Email} уже существует");
        }
        
        var mail = new Mail
        {
            Email = dto.Email,
            Password = dto.Password,
            Token = string.Empty
        };

        mail.Token = GenerateTokenFromId(mail.Id);

        await GetDatabase()
            .GetCollection<Mail>(CollectionName)
            .InsertOneAsync(mail, cancellationToken: cancellationToken);

        return mail;
    }

    public async Task<IEnumerable<Mail>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await (await GetDatabase()
                .GetCollection<Mail>(CollectionName)
                .FindAsync(Builders<Mail>.Filter.Empty, cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);
    }

    public async Task<Mail?> GetAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var filter = Builders<Mail>.Filter.Eq(mail => mail.Email, email);
        
        return await (await GetDatabase()
                .GetCollection<Mail>(CollectionName)
                .FindAsync(filter, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(string email, UpdateMailDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Password);

        var filter = Builders<Mail>.Filter.Eq(mail => mail.Email, email);

        var update = Builders<Mail>.Update
            .Set(mail => mail.Email, dto.Email)
            .Set(mail => mail.Password, dto.Password);
        
        await GetDatabase()
            .GetCollection<Mail>(CollectionName)
            .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
    
    private IMongoDatabase GetDatabase() => GetDatabase("mails_db");
    
    private static string GenerateTokenFromId(string id)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(id));
        return Convert.ToBase64String(hashBytes);
    }
}