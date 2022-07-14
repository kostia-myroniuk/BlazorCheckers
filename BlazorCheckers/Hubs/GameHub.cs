using Microsoft.AspNetCore.SignalR;
using BlazorCheckers.Multiplayer;

namespace BlazorCheckers.Hubs
{
    public class GameHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (GameManager.UserRepository.Get(Context.ConnectionId) == null)
            {
                GameManager.UserRepository.Add(new User(Context.ConnectionId, "No name"));
            }
            System.Diagnostics.Debug.WriteLine($"{Context.ConnectionId} connected");
            
            await base.OnConnectedAsync();

            await Clients.Client(Context.ConnectionId).SendAsync("GetAllLobies", GameManager.LobbyRepository.Lobbies);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            GameManager.UserRepository.Remove(Context.ConnectionId);
            System.Diagnostics.Debug.WriteLine($"{Context.ConnectionId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetAllLobbies()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("GetAllLobies", GameManager.LobbyRepository.Lobbies);
        }

        public async Task AddLobby(string lobbyName)
        {
            var creator = GameManager.UserRepository.Get(Context.ConnectionId);
            if (creator == null)
            {
                return;
            }

            Lobby newLobby = new Lobby(lobbyName, creator, DateTime.Now);
            GameManager.LobbyRepository.Add(newLobby);
            await Clients.All.SendAsync("LobbyAdded", newLobby);
        }

        public async Task JoinLobby(int lobbyId)
        {
            var lobby = GameManager.LobbyRepository.Get(lobbyId);
            var user = GameManager.UserRepository.Get(Context.ConnectionId);
            if (lobby == null || user == null)
            {
                return;
            }
            bool joined = lobby.AddUser(user);
            if (!joined)
            {
                return;
            }

            foreach (var lobbyUser in lobby.Users)
            {
                System.Diagnostics.Debug.WriteLine($"{lobbyUser.Id}, {lobbyUser.Nickname}");
            }

            await Clients.All.SendAsync("JoinedLobby", lobby);
        }

        public async Task ChangeNickname(string newNickname)
        {
            var user = GameManager.UserRepository.Get(Context.ConnectionId);
            if (user == null)
            {
                return;
            }
            
            user.Nickname = newNickname;
            await Clients.All.SendAsync("NicknameChanged", newNickname);
        }
    }
}
