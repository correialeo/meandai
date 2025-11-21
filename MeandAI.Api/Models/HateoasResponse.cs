namespace MeandAI.Api.Models;

/// <summary>
/// Wraps a single resource instance with the hypermedia links needed to navigate the API.
/// </summary>
/// <typeparam name="T">Type of the resource.</typeparam>
public class HateoasResponse<T>
{
    /// <summary>
    /// Resource returned by the endpoint.
    /// </summary>
    public T Resource { get; init; } = default!;

    /// <summary>
    /// Hypermedia links that describe the actions available for the resource.
    /// </summary>
    public List<LinkModel> Links { get; init; } = new();

    /// <summary>
    /// Creates a new HATEOAS envelope for a resource.
    /// </summary>
    public static HateoasResponse<T> From(T resource, IEnumerable<LinkModel> links)
        => new()
        {
            Resource = resource,
            Links = links.ToList()
        };
}
