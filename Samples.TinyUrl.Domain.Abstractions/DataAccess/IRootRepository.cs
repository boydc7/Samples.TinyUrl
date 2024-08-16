using Samples.TinyUrl.Domain.Abstractions.Models;

namespace Samples.TinyUrl.Domain.Abstractions.DataAccess;

public interface IRootRepository
{
    public Task CreateRootAsync(Root root);
    public Task<Root?> GetRootAsync(string id);
}
