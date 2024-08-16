namespace Samples.TinyUrl.Domain.Abstractions.Services;

public interface ISequenceGenerator
{
    ValueTask<long> IncrementAsync(string key, long amount = 1);
}
