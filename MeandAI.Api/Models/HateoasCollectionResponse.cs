using System.Text.Json.Serialization;

namespace MeandAI.Api.Models;

/// <summary>
/// Standard envelope for collections, adding metadata and HATEOAS links.
/// </summary>
/// <typeparam name="T">Type of the resource contained in the response.</typeparam>
public class HateoasCollectionResponse<T>
{
    /// <summary>
    /// Resources returned by the endpoint.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// Hypermedia links to related resources.
    /// </summary>
    public List<LinkModel> Links { get; init; } = new();

    /// <summary>
    /// Optional pagination metadata.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationMetadata? Pagination { get; init; }

    /// <summary>
    /// Creates a new HATEOAS response for a collection.
    /// </summary>
    public static HateoasCollectionResponse<T> From(IReadOnlyCollection<T> items, IEnumerable<LinkModel> links, PaginationMetadata? pagination = null)
        => new()
        {
            Items = items,
            Links = links.ToList(),
            Pagination = pagination
        };
}
