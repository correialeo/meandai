namespace MeandAI.Api.Models;

/// <summary>
/// Represents a hypermedia link following HATEOAS guidelines.
/// </summary>
/// <param name="Rel">Relation between the resource and the link.</param>
/// <param name="Href">Absolute URL that can be invoked.</param>
/// <param name="Method">HTTP verb to use when following the link.</param>
public record LinkModel(string Rel, string Href, string Method);
