using Blazor.Extensions;
using BlazorChess.Shared;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Client.Pages
{
	public class QueueComponent : BlazorComponent
	{
		private HubConnection _connection;
		public Player Me { get; set; }
		public string PlayerName { get; set; }
		public IList<Player> Players { get; set; } = new List<Player>();
		
		protected override async Task OnInitAsync()
		{
			_connection = new HubConnectionBuilder()
				.WithUrl("/queuehub",
				opt =>
				{
					opt.LogLevel = SignalRLogLevel.Trace;
					opt.Transport = HttpTransportType.WebSockets;
				})
				.Build();

			_connection.On<Player>("JoinQueue", HandleJoinQueue);
			_connection.On<Player>("LeaveQueue", HandleLeaveQueue);
			_connection.OnClose(exc =>
			{
				// _logger.LogError(exc, "Connection was closed!");
				return Task.CompletedTask;
			});
			await _connection.StartAsync();
		}

		private Task HandleLeaveQueue(object player)
		{
			Players.Remove(Players.Single(p => p.Id == (player as Player).Id));
			StateHasChanged();
			return Task.CompletedTask;
		}

		private Task HandleJoinQueue(object player)
		{
			Players.Add(player as Player);
			StateHasChanged();
			return Task.CompletedTask;
		}

		public async Task JoinQueueAsync()
		{
			Me = new Player
			{
				Id = Guid.NewGuid(),
				Name = PlayerName
			};
			await _connection.InvokeAsync("JoinQueue", Me);
		}

		public async Task LeaveQueueAsync()
		{
			var me = Me;
			Me = null;
			await _connection.InvokeAsync("LeaveQueue", me);
		}
	}
}
