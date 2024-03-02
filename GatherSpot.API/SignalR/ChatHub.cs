using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace GatherSpot.API.SignalR
{
	public class ChatHub : Hub
	{
		private readonly IMediator _mediator;

		public ChatHub(IMediator mediator)
		{
			_mediator = mediator;
		}
		// What we call a method is important, since the client will connect to the signalR hub,
		// and invoke this method 
		public async Task SendComment(CreateCommentCommand command)
		{
			var comment = await _mediator.Send(command);
			await Clients.Group(command.ActivityId.ToString())
				.SendAsync("ReceiveComment", comment.Value);
		}

		public override async Task OnConnectedAsync()
		{
			var httpContext = Context.GetHttpContext();
			var activityId = httpContext.Request.Query["activityId"];
			await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
			var result = await _mediator.Send(new GetCommentsQuery(Guid.Parse(activityId)));
			await Clients.Caller.SendAsync("LoadComments", result.Value);
		}
	}
}
