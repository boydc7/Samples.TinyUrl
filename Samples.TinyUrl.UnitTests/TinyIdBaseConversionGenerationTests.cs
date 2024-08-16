using System.Collections.Concurrent;
using Samples.TinyUrl.Domain.Abstractions.Enums;
using Samples.TinyUrl.Domain.Memory.Services;

namespace Samples.TinyUrl.UnitTests;

public class TinyIdBaseConversionGenerationTests
{
    private InMemoryBaseConversionTinyIdGenerator _baseConversionGenerator;
    private readonly InMemorySequenceProvider _inMemorySequenceProvider = InMemorySequenceProvider.Create();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _baseConversionGenerator = new InMemoryBaseConversionTinyIdGenerator(_inMemorySequenceProvider);
    }

    [Test]
    public async Task TinyIdLengthIsCorrect()
    {
        var id = await _baseConversionGenerator.GetIdAsync();

        Assert.That(id, Has.Length.EqualTo(TinyIds.Length));
    }

    [Test]
    public async Task TinyIdsForDifferentGroupsSeedTheSame()
    {
        var group1A = await _baseConversionGenerator.GetIdAsync("group1");
        var group2A = await _baseConversionGenerator.GetIdAsync("group2");
        var group1B = await _baseConversionGenerator.GetIdAsync("group1");
        var group2B = await _baseConversionGenerator.GetIdAsync("group2");

        Assert.That(group1A, Is.EqualTo(group2A));
        Assert.That(group1B, Is.EqualTo(group2B));
        Assert.That(group1A, Is.Not.EqualTo(group1B));
        Assert.That(group2A, Is.Not.EqualTo(group2B));
    }

    [Test]
    public async Task TinyIdsGeneratedInParallelAreUnique()
    {
        var failedOn = (Task: 0, Id: string.Empty);
        var generated = new ConcurrentDictionary<string, int>(StringComparer.Ordinal);

        var tasks = Enumerable.Range(1, 5)
                              .Select(i => Task.Run(async () =>
                                                    {
                                                        for (var n = 0; n < 5000; n++)
                                                        {
                                                            if (failedOn.Task > 0)
                                                            {
                                                                break;
                                                            }

                                                            var id = await _baseConversionGenerator.GetIdAsync();

                                                            if (!generated.TryAdd(id, i))
                                                            {
                                                                failedOn = (i, id);

                                                                break;
                                                            }
                                                        }
                                                    }))
                              .ToArray();

        await Task.WhenAll(tasks);

        Assert.Multiple(() =>
                        {
                            Assert.That(failedOn.Task, Is.LessThanOrEqualTo(0));
                            Assert.That(generated, Has.Count.GreaterThanOrEqualTo(25000));
                        });
    }
}
