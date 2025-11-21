using MeandAI.Api;
using MeandAI.Api.Models;
using MeandAI.Application.DTOs.LearningPaths;
using MeandAI.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeandAI.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/learning-paths")]
[Authorize]
public class LearningPathsController : ApiControllerBase
{
    private readonly ILearningPathsService _learningPathsService;

    public LearningPathsController(ILearningPathsService learningPathsService)
    {
        _learningPathsService = learningPathsService;
    }

    /// <summary>
    /// Creates a new learning path definition.
    /// </summary>
    /// <param name="request">Learning path payload containing steps and metadata.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpPost(Name = RouteNames.LearningPaths.Create)]
    [ProducesResponseType(typeof(HateoasResponse<LearningPathDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLearningPathRequest request, CancellationToken cancellationToken)
    {
        LearningPathDto learningPath = await _learningPathsService.CreateAsync(request, cancellationToken);
        List<LinkModel> links = BuildLearningPathLinks(learningPath.Id);
        return CreatedAtRoute(RouteNames.LearningPaths.GetById, new { version = CurrentApiVersion, id = learningPath.Id }, HateoasResponse<LearningPathDto>.From(learningPath, links));
    }

    /// <summary>
    /// Lists all learning paths.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet(Name = RouteNames.LearningPaths.GetAll)]
    [ProducesResponseType(typeof(HateoasCollectionResponse<LearningPathDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<LearningPathDto> learningPaths = await _learningPathsService.GetAllAsync(cancellationToken);
        List<LinkModel> links = new List<LinkModel>
        {
            CreateLink("self", RouteNames.LearningPaths.GetAll, null, HttpMethods.Get),
            CreateLink("create", RouteNames.LearningPaths.Create, null, HttpMethods.Post)
        };

        return Ok(HateoasCollectionResponse<LearningPathDto>.From(learningPaths, links));
    }

    /// <summary>
    /// Gets a specific learning path by identifier.
    /// </summary>
    /// <param name="id">Learning path identifier.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet("{id:guid}", Name = RouteNames.LearningPaths.GetById)]
    [ProducesResponseType(typeof(HateoasResponse<LearningPathDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        LearningPathDto? learningPath = await _learningPathsService.GetByIdAsync(id, cancellationToken);
        if (learningPath is null)
        {
            return NotFound();
        }

        List<LinkModel> links = BuildLearningPathLinks(learningPath.Id);
        return Ok(HateoasResponse<LearningPathDto>.From(learningPath, links));
    }

    /// <summary>
    /// Lists learning paths targeting a specific area.
    /// </summary>
    /// <param name="targetArea">Desired or target area.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet("target-area/{targetArea}", Name = RouteNames.LearningPaths.GetByTargetArea)]
    [ProducesResponseType(typeof(HateoasCollectionResponse<LearningPathDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTargetAreaAsync(string targetArea, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<LearningPathDto> learningPaths = await _learningPathsService.GetForTargetAreaAsync(targetArea, cancellationToken);
        List<LinkModel> links = new List<LinkModel>
        {
            CreateLink("self", RouteNames.LearningPaths.GetByTargetArea, new { version = CurrentApiVersion, targetArea }, HttpMethods.Get),
            CreateLink("catalog", RouteNames.LearningPaths.GetAll, null, HttpMethods.Get),
            CreateLink("create", RouteNames.LearningPaths.Create, null, HttpMethods.Post)
        };

        return Ok(HateoasCollectionResponse<LearningPathDto>.From(learningPaths, links));
    }

    /// <summary>
    /// Updates a learning path definition.
    /// </summary>
    /// <param name="id">Learning path identifier.</param>
    /// <param name="request">Fields that can be edited.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpPut("{id:guid}", Name = RouteNames.LearningPaths.Update)]
    [ProducesResponseType(typeof(HateoasResponse<LearningPathDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateLearningPathRequest request, CancellationToken cancellationToken)
    {
        LearningPathDto? learningPath = await _learningPathsService.UpdateAsync(id, request, cancellationToken);
        if (learningPath is null)
        {
            return NotFound();
        }

        List<LinkModel> links = BuildLearningPathLinks(learningPath.Id);
        return Ok(HateoasResponse<LearningPathDto>.From(learningPath, links));
    }

    /// <summary>
    /// Deletes a learning path definition.
    /// </summary>
    /// <param name="id">Learning path identifier.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpDelete("{id:guid}", Name = RouteNames.LearningPaths.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await _learningPathsService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Adds a new step to the learning path.
    /// </summary>
    /// <param name="id">Learning path identifier.</param>
    /// <param name="request">Step details.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpPost("{id:guid}/steps", Name = RouteNames.LearningPaths.AddStep)]
    [ProducesResponseType(typeof(HateoasResponse<LearningPathStepDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddStepAsync(Guid id, [FromBody] AddStepToPathRequest request, CancellationToken cancellationToken)
    {
        LearningPathStepDto? step = await _learningPathsService.AddStepAsync(id, request, cancellationToken);
        if (step is null)
        {
            return NotFound();
        }

        List<LinkModel> links = new List<LinkModel>
        {
            CreateLink("learning-path", RouteNames.LearningPaths.GetById, new { version = CurrentApiVersion, id }, HttpMethods.Get),
            CreateLink("update-path", RouteNames.LearningPaths.Update, new { version = CurrentApiVersion, id }, HttpMethods.Put)
        };

        return CreatedAtRoute(RouteNames.LearningPaths.GetById, new { version = CurrentApiVersion, id }, HateoasResponse<LearningPathStepDto>.From(step, links));
    }

    private List<LinkModel> BuildLearningPathLinks(Guid id)
        => new()
        {
            CreateLink("self", RouteNames.LearningPaths.GetById, new { version = CurrentApiVersion, id }, HttpMethods.Get),
            CreateLink("update", RouteNames.LearningPaths.Update, new { version = CurrentApiVersion, id }, HttpMethods.Put),
            CreateLink("delete", RouteNames.LearningPaths.Delete, new { version = CurrentApiVersion, id }, HttpMethods.Delete),
            CreateLink("add-step", RouteNames.LearningPaths.AddStep, new { version = CurrentApiVersion, id }, HttpMethods.Post)
        };
}
