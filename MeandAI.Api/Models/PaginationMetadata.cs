namespace MeandAI.Api.Models;

/// <summary>
/// Describes pagination information for collection responses.
/// </summary>
/// <param name="Page">Current page that was queried.</param>
/// <param name="PageSize">Number of items per page.</param>
/// <param name="TotalCount">Total amount of records available.</param>
/// <param name="TotalPages">Total pages calculated from the dataset.</param>
public record PaginationMetadata(int Page, int PageSize, int TotalCount, int TotalPages);
