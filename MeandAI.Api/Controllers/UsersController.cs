using MeandAI.Api;
using MeandAI.Api.Models;
using MeandAI.Application.Common;
using MeandAI.Application.DTOs.Auth;
using MeandAI.Application.DTOs.Skills;
using MeandAI.Application.DTOs.Users;
using MeandAI.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeandAI.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ApiControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>
    /// Creates a new user profile.
    /// </summary>
    /// <param name="request">Payload containing personal and professional info.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    /// <returns>The created user enriched with relevant HATEOAS links.</returns>
    [HttpPost(Name = RouteNames.Users.Create)]
    [ProducesResponseType(typeof(HateoasResponse<UserDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            UserDto user = await _usersService.CreateAsync(request, cancellationToken);
            List<LinkModel> links = BuildUserLinks(user.Id);
            return CreatedAtRoute(RouteNames.Users.GetById, new { version = CurrentApiVersion, id = user.Id }, HateoasResponse<UserDto>.From(user, links));
        }
        catch (InvalidOperationException ex)
        {
            return DomainProblem(ex);
        }
    }

    /// <summary>
    /// Retrieves a user by its unique identifier.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet("{id:guid}", Name = RouteNames.Users.GetById)]
    [Authorize]
    [ProducesResponseType(typeof(HateoasResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        UserDto? user = await _usersService.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFoundProblem($"Usuário {id} não foi encontrado.");
        }

        List<LinkModel> links = BuildUserLinks(user.Id);
        return Ok(HateoasResponse<UserDto>.From(user, links));
    }

    /// <summary>
    /// Returns paged users filtered by page number and size.
    /// </summary>
    /// <param name="page">Page number starting at 1.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet(Name = RouteNames.Users.GetPaged)]
    [ProducesResponseType(typeof(HateoasCollectionResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequestProblem("Os parâmetros page e pageSize devem ser maiores que zero.");
        }

        PagedResult<UserDto> result = await _usersService.GetPagedAsync(page, pageSize, cancellationToken);
        PaginationMetadata pagination = BuildPaginationMetadata(result);
        List<LinkModel> links = new List<LinkModel>
        {
            CreateLink("self", RouteNames.Users.GetPaged, new { version = CurrentApiVersion, page, pageSize }, HttpMethods.Get),
            CreateLink("create", RouteNames.Users.Create, null, HttpMethods.Post)
        };

        if (page > 1)
        {
            links.Add(CreateLink("previous", RouteNames.Users.GetPaged, new { version = CurrentApiVersion, page = page - 1, pageSize }, HttpMethods.Get));
        }

        int totalPages = pagination.TotalPages;
        if (page < totalPages)
        {
            links.Add(CreateLink("next", RouteNames.Users.GetPaged, new { version = CurrentApiVersion, page = page + 1, pageSize }, HttpMethods.Get));
        }

        return Ok(HateoasCollectionResponse<UserDto>.From(result.Items, links, pagination));
    }

    /// <summary>
    /// Updates the user profile fields.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="request">Payload with the mutable fields.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpPut("{id:guid}", Name = RouteNames.Users.Update)]
    [Authorize]
    [ProducesResponseType(typeof(HateoasResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfileAsync(Guid id, [FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        UserDto? user = await _usersService.UpdateProfileAsync(id, request, cancellationToken);
        if (user is null)
        {
            return NotFoundProblem($"Usuário {id} não foi encontrado.");
        }

        List<LinkModel> links = BuildUserLinks(user.Id);
        return Ok(HateoasResponse<UserDto>.From(user, links));
    }

    /// <summary>
    /// Deletes the user profile and all associated progress.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpDelete("{id:guid}", Name = RouteNames.Users.Delete)]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await _usersService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFoundProblem($"Usuário {id} não foi encontrado.");
        }

        return NoContent();
    }

    /// <summary>
    /// Adds a skill to the user profile.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="request">Skill identifier and proficiency level.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpPost("{id:guid}/skills", Name = RouteNames.Users.AddSkill)]
    [Authorize]
    [ProducesResponseType(typeof(HateoasResponse<UserSkillDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddSkillAsync(Guid id, [FromBody] AddUserSkillRequest request, CancellationToken cancellationToken)
    {
        try
        {
            UserSkillDto? userSkill = await _usersService.AddSkillAsync(id, request, cancellationToken);
            if (userSkill is null)
            {
                return NotFoundProblem($"Usuário {id} não foi encontrado.");
            }

            List<LinkModel> links = new List<LinkModel>
            {
                CreateLink("self", RouteNames.Users.GetSkills, new { id }, HttpMethods.Get),
                CreateLink("user", RouteNames.Users.GetById, new { id }, HttpMethods.Get),
            };

            return CreatedAtRoute(RouteNames.Users.GetSkills, new { version = CurrentApiVersion, id }, HateoasResponse<UserSkillDto>.From(userSkill, links));
        }
        catch (InvalidOperationException ex)
        {
            return DomainProblem(ex);
        }
    }

    /// <summary>
    /// Lists all skills assigned to the user.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="cancellationToken">Cancellation token propagated down the stack.</param>
    [HttpGet("{id:guid}/skills", Name = RouteNames.Users.GetSkills)]
    [ProducesResponseType(typeof(HateoasCollectionResponse<UserSkillDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillsAsync(Guid id, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<UserSkillDto> skills = await _usersService.GetSkillsAsync(id, cancellationToken);
        List<LinkModel> links = new List<LinkModel>
        {
            CreateLink("self", RouteNames.Users.GetSkills, new { id }, HttpMethods.Get),
            CreateLink("user", RouteNames.Users.GetById, new { id }, HttpMethods.Get),
            CreateLink("add", RouteNames.Users.AddSkill, new { id }, HttpMethods.Post)
        };

        return Ok(HateoasCollectionResponse<UserSkillDto>.From(skills, links));
    }

    private List<LinkModel> BuildUserLinks(Guid id)
        => new()
        {
            CreateLink("self", RouteNames.Users.GetById, new { id }, HttpMethods.Get),
            CreateLink("update", RouteNames.Users.Update, new { id }, HttpMethods.Put),
            CreateLink("delete", RouteNames.Users.Delete, new { id }, HttpMethods.Delete),
            CreateLink("skills", RouteNames.Users.GetSkills, new { id }, HttpMethods.Get)
        };
}
