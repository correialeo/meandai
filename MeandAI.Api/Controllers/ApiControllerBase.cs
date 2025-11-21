using System;
using MeandAI.Api.Models;
using MeandAI.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace MeandAI.Api.Controllers;

/// <summary>
/// Base class for all API controllers, centralizing helpers shared across endpoints.
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Creates a hypermedia link pointing to a named route.
    /// </summary>
    /// <param name="rel">Relation between the current resource and the target route.</param>
    /// <param name="routeName">Route name previously configured in attribute routing.</param>
    /// <param name="routeValues">Route values to compose the URL.</param>
    /// <param name="method">HTTP verb used to follow the link.</param>
    /// <returns>A fully computed <see cref="LinkModel"/>.</returns>
    protected LinkModel CreateLink(string rel, string routeName, object? routeValues, string method)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rel);
        ArgumentException.ThrowIfNullOrWhiteSpace(routeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(method);

        RouteValueDictionary values = routeValues is null
            ? new RouteValueDictionary()
            : new RouteValueDictionary(routeValues);

        if (!values.ContainsKey("version"))
        {
            values["version"] = CurrentApiVersion;
        }

        string href = Url.Link(routeName, values) ?? string.Empty;
        return new LinkModel(rel, href, method.ToUpperInvariant());
    }

    protected string CurrentApiVersion
        => HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

    /// <summary>
    /// Builds pagination metadata from an application paged result.
    /// </summary>
    /// <param name="result">Result returned by the application layer.</param>
    /// <typeparam name="T">Type of the paged item.</typeparam>
    protected static PaginationMetadata BuildPaginationMetadata<T>(PagedResult<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        int totalPages = result.PageSize > 0
            ? (int)Math.Ceiling(result.TotalCount / (double)result.PageSize)
            : 0;

        return new PaginationMetadata(result.Page, result.PageSize, result.TotalCount, totalPages);
    }

    /// <summary>
    /// Creates a consistent not-found response containing additional details.
    /// </summary>
    /// <param name="detail">Extra information about the missing resource.</param>
    protected IActionResult NotFoundProblem(string detail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);

        return NotFound(new ProblemDetails
        {
            Title = "Resource not found",
            Detail = detail,
            Status = StatusCodes.Status404NotFound
        });
    }

    /// <summary>
    /// Creates a consistent bad-request response containing additional details.
    /// </summary>
    /// <param name="detail">Reason explaining why the request is invalid.</param>
    protected IActionResult BadRequestProblem(string detail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);

        return BadRequest(new ProblemDetails
        {
            Title = "Invalid request",
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        });
    }

    /// <summary>
    /// Creates a consistent conflict response containing additional details.
    /// </summary>
    /// <param name="detail">Reason explaining the conflict.</param>
    protected IActionResult ConflictProblem(string detail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);

        return Conflict(new ProblemDetails
        {
            Title = "Conflict detected",
            Detail = detail,
            Status = StatusCodes.Status409Conflict
        });
    }

    /// <summary>
    /// Maps <see cref="InvalidOperationException"/> coming from the domain/application layers to HTTP semantics.
    /// </summary>
    /// <param name="exception">Exception raised by the downstream layer.</param>
    protected IActionResult DomainProblem(InvalidOperationException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        string detail = exception.Message;

        if (detail.Contains("não foi encontrada", StringComparison.InvariantCultureIgnoreCase)
            || detail.Contains("não foi encontrado", StringComparison.InvariantCultureIgnoreCase)
            || detail.Contains("não encontrado", StringComparison.InvariantCultureIgnoreCase))
        {
            return NotFoundProblem(detail);
        }

        if (detail.Contains("já existe", StringComparison.InvariantCultureIgnoreCase))
        {
            return ConflictProblem(detail);
        }

        if (detail.Contains("inválido", StringComparison.InvariantCultureIgnoreCase))
        {
            return BadRequestProblem(detail);
        }

        return BadRequestProblem("Não foi possível processar a requisição enviada.");
    }
}
