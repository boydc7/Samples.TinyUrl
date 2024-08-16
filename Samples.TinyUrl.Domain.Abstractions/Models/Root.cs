using Samples.TinyUrl.Common.Interfaces;

namespace Samples.TinyUrl.Domain.Abstractions.Models;

public class Root : IHasStringId
{
    public const string? Default = "https://surl.com/";
    
    public required string Id { get; init; }
}
