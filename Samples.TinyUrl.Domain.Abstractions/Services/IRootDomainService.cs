namespace Samples.TinyUrl.Domain.Abstractions.Services;

public interface IRootDomainService
{
    Task<string?> GetRootDomainAsync(string? rootDomain);
}
