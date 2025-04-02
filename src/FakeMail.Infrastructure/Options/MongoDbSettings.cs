namespace FakeMail.Repositories.Options;

public class MongoDbSettings
{
    public required string User { get; init; }
    public required string Password { get; init; }
    public int Port { get; init; }
}