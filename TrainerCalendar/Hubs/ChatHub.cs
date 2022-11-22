using Microsoft.AspNetCore.SignalR;
using TrainerCalendar.Models.Dto;

namespace TrainerCalendar.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(UserConnectionDto connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.Room);
            await Clients.Group(connection.Room).SendAsync("ReceiveMessage", "Sent By Bot", connection.Username + " has joined " + connection.Room);
        }
    }
}
