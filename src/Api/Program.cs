using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost(
    "/v1/things",
    Results<Ok<CreateThingResponse>, BadRequest>(CreateThing createThing) =>
    {
        // some random validations, could be better done, with a problem details payload and that sort of thing
        if (
            string.IsNullOrWhiteSpace(createThing.Name)
            || createThing.AcquiredAt == default
            || createThing.AcquiredAt > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(
            new CreateThingResponse(
                Guid.NewGuid(),
                createThing.Name,
                createThing.Description,
                createThing.AcquiredAt));
    });

app.UseSwaggerUI(options => { options.SwaggerEndpoint("/v1/openapi.yaml", "v1"); });

var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".yaml"] = "application/x-yaml";
contentTypeProvider.Mappings[".yml"] = "application/x-yaml";

app.UseStaticFiles(new StaticFileOptions
{
    // we need to serve unknown file types, or pass in a non-default content type provider, otherwise, *.yaml isn't returned
    //ServeUnknownFileTypes = true,
    ContentTypeProvider = contentTypeProvider
});

app.Run();

public record CreateThing(
    string Name,
    string Description,
    DateOnly AcquiredAt);

public record CreateThingResponse(
    Guid Id,
    string Name,
    string Description,
    DateOnly AcquiredAt);


// for tests
public partial class Program
{
}