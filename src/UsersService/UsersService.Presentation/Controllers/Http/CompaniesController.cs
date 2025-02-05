using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Companies.Commands.AddCompany;
using UsersService.Application.Companies.Commands.DeleteCompany;
using UsersService.Application.Companies.Commands.UpdateCompany;
using UsersService.Application.Companies.Queries.GetCompany;
using UsersService.Application.Companies.Queries.GetCompanyByUser;
using UsersService.Domain.Constants;
using UsersService.Domain.Models;
using UsersService.Presentation.Middleware.Authentication;

namespace UsersService.Presentation.Controllers.Http
{
    [ApiController]
    [Route("/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompaniesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Add([FromForm] AddCompanyCommand addCompanyCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(addCompanyCommand, cancellationToken));
        }

        [HttpPut]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<Guid>> Update([FromForm] UpdateCompanyCommand updateCompanyCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateCompanyCommand, cancellationToken));
        }

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<Guid>> Delete(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new DeleteCompanyCommand(id), cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> Get(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetCompanyQuery(id), cancellationToken));
        }

        [HttpGet("users/{userId}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<Company>> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetCompanyByUserQuery(userId), cancellationToken));
        }
    }
}
