using MeandAI.Api;
using MeandAI.Api.Models;
using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeandAI.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/skills")]
[Authorize]
public class SkillsController : ApiControllerBase
{
    private readonly ISkillsService _skillsService;

    public SkillsController(ISkillsService skillsService)
    {
        _skillsService = skillsService;
    }

    /// <summary>
    /// Registers a new skill in the catalog.
    /// </summary>
    /// <param name="request">Skill payload containing taxonomy data.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpPost(Name = RouteNames.Skills.Create)]
    [ProducesResponseType(typeof(HateoasResponse<SkillDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSkillRequest request, CancellationToken cancellationToken)
    {
        SkillDto skill = await _skillsService.CreateAsync(request, cancellationToken);
        List<LinkModel> links = BuildSkillLinks(skill.Id);
        return CreatedAtRoute(RouteNames.Skills.GetById, new { version = CurrentApiVersion, id = skill.Id }, HateoasResponse<SkillDto>.From(skill, links));
    }

    /// <summary>
    /// Returns the complete catalog of skills.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet(Name = RouteNames.Skills.GetAll)]
    [ProducesResponseType(typeof(HateoasCollectionResponse<SkillDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SkillDto> skills = await _skillsService.GetAllAsync(cancellationToken);
        List<LinkModel> links = new List<LinkModel>
        {
            CreateLink("self", RouteNames.Skills.GetAll, null, HttpMethods.Get),
            CreateLink("create", RouteNames.Skills.Create, null, HttpMethods.Post)
        };

        return Ok(HateoasCollectionResponse<SkillDto>.From(skills, links));
    }

    /// <summary>
    /// Retrieves details for a specific skill.
    /// </summary>
    /// <param name="id">Skill identifier.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet("{id:guid}", Name = RouteNames.Skills.GetById)]
    [ProducesResponseType(typeof(HateoasResponse<SkillDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        SkillDto? skill = await _skillsService.GetByIdAsync(id, cancellationToken);
        if (skill is null)
        {
            return NotFound();
        }

        List<LinkModel> links = BuildSkillLinks(skill.Id);
        return Ok(HateoasResponse<SkillDto>.From(skill, links));
    }

    /// <summary>
    /// Lists skills filtered by category.
    /// </summary>
    /// <param name="category">Category name.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet("category/{category}", Name = RouteNames.Skills.GetByCategory)]
    [ProducesResponseType(typeof(HateoasCollectionResponse<SkillDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCategoryAsync(string category, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SkillDto> skills = await _skillsService.GetByCategoryAsync(category, cancellationToken);
        List<LinkModel> links = new List<LinkModel>
        {
            CreateLink("self", RouteNames.Skills.GetByCategory, new { version = CurrentApiVersion, category }, HttpMethods.Get),
            CreateLink("catalog", RouteNames.Skills.GetAll, null, HttpMethods.Get),
            CreateLink("create", RouteNames.Skills.Create, null, HttpMethods.Post)
        };

        return Ok(HateoasCollectionResponse<SkillDto>.From(skills, links));
    }

    /// <summary>
    /// Updates a skill definition.
    /// </summary>
    /// <param name="id">Skill identifier.</param>
    /// <param name="request">Fields allowed to change.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpPut("{id:guid}", Name = RouteNames.Skills.Update)]
    [ProducesResponseType(typeof(HateoasResponse<SkillDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateSkillRequest request, CancellationToken cancellationToken)
    {
        SkillDto? skill = await _skillsService.UpdateAsync(id, request, cancellationToken);
        if (skill is null)
        {
            return NotFound();
        }

        List<LinkModel> links = BuildSkillLinks(skill.Id);
        return Ok(HateoasResponse<SkillDto>.From(skill, links));
    }

    /// <summary>
    /// Removes a skill from the catalog.
    /// </summary>
    /// <param name="id">Skill identifier.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpDelete("{id:guid}", Name = RouteNames.Skills.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await _skillsService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private List<LinkModel> BuildSkillLinks(Guid id)
        => new()
        {
            CreateLink("self", RouteNames.Skills.GetById, new { version = CurrentApiVersion, id }, HttpMethods.Get),
            CreateLink("update", RouteNames.Skills.Update, new { version = CurrentApiVersion, id }, HttpMethods.Put),
            CreateLink("delete", RouteNames.Skills.Delete, new { version = CurrentApiVersion, id }, HttpMethods.Delete)
        };
}
