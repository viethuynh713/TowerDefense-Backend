using Microsoft.AspNetCore.SignalR;

public class RealtimeHub : Hub
{
    private static int ViewCount { get; set; } = 0;

    public Task IncrementServerView()
    {
        ViewCount++;

        return Clients.All.SendAsync("incrementView", ViewCount);
    }
    //private readonly string _botUser;
    //private readonly IDictionary<string, UserDataMatching> _connections;

    //public RealtimeHub(IDictionary<string, UserDataMatching> connections)
    //{
    //    _botUser = "MyChat Bot";
    //    _connections = connections;
    //}

    //public override Task OnDisconnectedAsync(Exception exception) // disconnect
    //{
    //    if (_connections.TryGetValue(Context.ConnectionId, out UserDataMatching userDataMatching))
    //    {
    //        _connections.Remove(Context.ConnectionId);
    //        Clients.Group(userDataMatching.roomCode).SendAsync("ReceiveMessage", _botUser, $"{userDataMatching.userId} has left");
    //        SendUsersConnected(userDataMatching.roomCode);
    //    }

    //    return base.OnDisconnectedAsync(exception);
    //}

    //public async Task JoinRoom(UserDataMatching userDataMatching)  // On MM success
    //{
    //    await Groups.AddToGroupAsync(Context.ConnectionId, userDataMatching.roomCode);

    //    _connections[Context.ConnectionId] = userDataMatching;

    //    await Clients.Group(userDataMatching.roomCode).SendAsync("ReceiveMessage", _botUser, $"{userDataMatching.userId} has joined {userDataMatching.roomCode}");

    //    await SendUsersConnected(userDataMatching.roomCode);
    //}

    //public async Task SendMessage(string message) // transfer method
    //{
    //    if (_connections.TryGetValue(Context.ConnectionId, out UserDataMatching userDataMatching))
    //    {
    //        await Clients.Group(userDataMatching.roomCode).SendAsync("ReceiveMessage", userDataMatching.userId, message);
    //    }
    //}

    //public Task SendUsersConnected(string room) // notify another MM success
    //{
    //    var users = _connections.Values
    //        .Where(c => c.roomCode == room)
    //        .Select(c => c.userId);

    //    return Clients.Group(room).SendAsync("UsersInRoom", users);
    //}

}
