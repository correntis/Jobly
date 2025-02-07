using Microsoft.AspNetCore.Http;

namespace MessagesService.Presentation.Middleware.Authorization
{
    public class AuthorizationMiddleware : IMiddleware
    {
        private readonly Abstractions.IAuthorizationHandler _authorizationHandler;

        public AuthorizationMiddleware(Abstractions.IAuthorizationHandler authorizationHandler)
        {
            _authorizationHandler = authorizationHandler;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint is not null)
            {
                var authorizeAttributes = endpoint.Metadata.GetOrderedMetadata<AuthorizeRoleAttribute>();

                if(authorizeAttributes is not null && authorizeAttributes.Count != 0)
                {
                    var roles = authorizeAttributes.SelectMany(attr => attr.Roles.Split(','));

                    var isAuthorized = await _authorizationHandler.HandleAsync(context, roles);

                    if(!isAuthorized)
                    {
                        return;
                    }
                }
            }

            await next(context);
        }
    }
}
