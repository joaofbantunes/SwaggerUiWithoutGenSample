using Contract.Generated.V1;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Contract;

public class ExampleContractTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _appFactory;

    public ExampleContractTest(WebApplicationFactory<Program> appFactory)
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task BasicContractTest()
    {
        using var baseClient = _appFactory.CreateClient();
        var client = new ThingsClient(baseClient);

        var createThing = new Generated.V1.CreateThing
        {
            Name = "Some Awesome Thing",
            Description = "This is the best thing we have ever gotten",
            AcquiredAt = new DateTimeOffset(
                new DateOnly(2023, 03, 21),
                new TimeOnly(0),
                new TimeSpan(0))
        };
        
        var response = await client.CreateThingAsync(createThing);

        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be(createThing.Name);
        response.Description.Should().Be(createThing.Description);
        response.AcquiredAt.Should().Be(createThing.AcquiredAt);
    }
}