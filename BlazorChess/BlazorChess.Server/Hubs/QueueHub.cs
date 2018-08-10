using BlazorChess.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BlazorChess.Server.Hubs
{
	public class QueueHub : Hub
	{
		public async Task LeaveQueue(Player player)
		{
			await Clients.All.SendAsync("LeaveQueue", player);
		}

		public async Task JoinQueue(Player player)
		{
			await Clients.All.SendAsync("JoinQueue", player);
		}
	}
}
