using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Companies.Commands.AddCompanyCommand;
using UsersService.Application.Companies.Commands.DeleteCompanyCommand;
using UsersService.Application.Companies.Commands.UpdateCompanyCommand;
using UsersService.Application.Companies.Queries.GetCompanyQuery;
using UsersService.Domain.Constants;
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
        public async Task<IActionResult> Add(AddCompanyCommand addCompanyCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(addCompanyCommand, cancellationToken));

        [HttpPut]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<IActionResult> Update(UpdateCompanyCommand updateCompanyCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateCompanyCommand, cancellationToken));

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DeleteCompanyCommand(id), cancellationToken));

        [HttpGet("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetCompanyQuery(id), cancellationToken));
    }
}
