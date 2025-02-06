using MediatR;
using MessagesService.Application.Chats.Queries.GetChatByApplication;
using MessagesService.Application.Chats.Queries.GetChatsPageByCompany;
using MessagesService.Application.Chats.Queries.GetPageByUser;
using MessagesService.Core.Constants;
using MessagesService.Core.Models;
using MessagesService.Presentation.Middleware.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagesService.Presentation.Controllers
{
    [ApiController]
    [Route("/chats")]
    public class ChatsController
    {
        private readonly ISender _sender;

        public ChatsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [Route("companies/{companyId}&pageIndex={pageIndex}&pageSize={pageSize}")]
        [AuthorizeRole( Roles = BusinessRules.Roles.Company )]
        public async Task<ActionResult<List<Chat>>> GetChatsPageByCompany(Guid companyId, int pageIndex, int pageSize, CancellationToken token)
        {
            return await _sender.Send(new GetChatsPageByCompanyQuery(companyId, pageIndex, pageSize), token);
        }

        [HttpGet]
        [Route("users/{userId}&pageIndex={pageIndex}&pageSize={pageSize}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Chat>>> GetChatsPageByUser(Guid userId, int pageIndex, int pageSize, CancellationToken token)
        {
            return await _sender.Send(new GetChatsPageByUserQuery(userId, pageIndex, pageSize), token);
        }

        [HttpGet]
        [Route("applications/{applicationId}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<Chat>> GetChatByApplication(Guid applicationId, CancellationToken token)
        {
            return await _sender.Send(new GetChatByApplicationQuery(applicationId), token);
        }
    }
}
