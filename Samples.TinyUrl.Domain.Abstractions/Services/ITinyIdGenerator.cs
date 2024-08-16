namespace Samples.TinyUrl.Domain.Abstractions.Services;

public interface ITinyIdGenerator
{
    public ValueTask<string> GetIdAsync(string? group = null);
}
