using MediatR;
using MessagesService.Application.Messages.Commands.EditMessage;
using MessagesService.Application.Messages.Commands.ReadMessage;
using MessagesService.Application.Messages.Commands.SendMessage;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessagesService.Presentation.Hubs
{
    public class MessagesHub : Hub
    {
        private readonly ILogger<MessagesHub> _logger;
        private readonly ISender _sender;

        public MessagesHub(ILogger<MessagesHub> logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("[SignalR] [Connection] userId = {userId}", Context.UserIdentifier);

            return base.OnConnectedAsync();
        }

        public async Task SendMessage(SaveMessageCommand command)
        {
            var message = await _sender.Send(command, Context.ConnectionAborted);

            _logger.LogInformation("[SignalR] Message saved, send to clients back message : {@Message}", message);

            await Clients
                .Users($"{message.RecipientId}", $"{message.SenderId}")
                .SendAsync("ReceiveMessage", message, Context.ConnectionAborted);
        }

        public async Task EditMessage(EditMessageCommand command)
        {
            var message = await _sender.Send(command, Context.ConnectionAborted);

            _logger.LogInformation("[SignalR] Message edited, send to clients back message : {@Message}", message);

            await Clients
                .Users($"{message.RecipientId}", $"{message.SenderId}")
                .SendAsync("EditMessage", message, Context.ConnectionAborted);
        }

        public async Task ReadMessage(ReadMessageCommand command)
        {
            var message = await _sender.Send(command, Context.ConnectionAborted);

            _logger.LogInformation("[SignalR] Message read, send to clients back message : {@Message}", message);

            await Clients
                .Users($"{message.RecipientId}", $"{message.SenderId}")
                .SendAsync("ReceiveReadMessage", message, Context.ConnectionAborted);
        }
    }
}
