using Microsoft.AspNetCore.SignalR;
using BlazorCheckers.Multiplayer;
using Newtonsoft.Json;
using BlazorCheckers.GameEngine;

namespace BlazorCheckers.Hubs
{
    public class GameHub : Hub
    {
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

            if (lobby.State == LobbyState.Game)
            {
                Game game = GameManager.GameRepository.Add(lobby);
                string serialized = JsonConvert.SerializeObject(game.Board);

                foreach (var lobbyUser in lobby.Users)
                {
                    await Clients.Client(lobbyUser.Id).SendAsync("GameStarted", 
                        serialized, game.CurrentPlayer, game.CaptureMoves, game.RegularMoves);
                }
            }
            else
            {
                string serialized = JsonConvert.SerializeObject(lobby);
                await Clients.Client(Context.ConnectionId).SendAsync("JoinedLobby", serialized);
                foreach (var otherUser in GameManager.UserRepository.Users.Where(u => u.Id != user.Id))
                {
                    await Clients.Client(otherUser.Id).SendAsync("LobbyUpdated", lobbyId, serialized);
                }
            }
        }

        public async Task LeaveLobby()
        {
            var user = GameManager.UserRepository.Get(Context.ConnectionId);
            if (user == null)
            {
                return;
            }

            var searchResult = GameManager.LobbyRepository.Find(user);
            if (searchResult == null)
            {
                return;
            }
            int lobbyId = searchResult.Item1;
            var lobby = searchResult.Item2;
            
            bool left = lobby.RemoveUser(user);
            if (!left)
            {
                return;
            }

            string serialized = JsonConvert.SerializeObject(lobby);
            await Clients.Client(Context.ConnectionId).SendAsync("LeftLobby");
            foreach (var otherUser in GameManager.UserRepository.Users.Where(u => u.Id != user.Id))
            {
                await Clients.Client(otherUser.Id).SendAsync("LobbyUpdated", lobbyId, serialized);
            }
        }

        public async Task MakeRegularMove(RegularMove move)
        {
            var lobby = GameManager.GetCurrentUserLobby(Context.ConnectionId);
            var game = GameManager.GetCurrentUserGame(Context.ConnectionId);
            if (lobby == null || game == null)
            {
                return;
            }

            game.ApplyRegularMove(move);

            string serialized = JsonConvert.SerializeObject(game.Board);
            foreach (var lobbyUser in lobby.Users)
            {
                await Clients.Client(lobbyUser.Id).SendAsync("GameUpdated",
                    serialized, game.CurrentPlayer, game.CaptureMoves, game.RegularMoves);
            }
        }

        public async Task MakeCaptureMove(RegularMove move)
        {
            var lobby = GameManager.GetCurrentUserLobby(Context.ConnectionId);
            var game = GameManager.GetCurrentUserGame(Context.ConnectionId);
            if (lobby == null || game == null)
            {
                return;
            }

            CaptureMove? captureMove = game.GetCaptureMove(move);
            if (captureMove is null)
            {
                return;
            }
            game.ApplyCaptureMove(captureMove);

            string serialized = JsonConvert.SerializeObject(game.Board);
            foreach (var lobbyUser in lobby.Users)
            {
                await Clients.Client(lobbyUser.Id).SendAsync("GameUpdated",
                    serialized, game.CurrentPlayer, game.CaptureMoves, game.RegularMoves);
            }
        }

        public async Task ChangeNickname(string newNickname)
        {
            var user = GameManager.UserRepository.Get(Context.ConnectionId);
            if (user == null)
            {
                return;
            }
            
            user.Nickname = newNickname;
            await Clients.All.SendAsync("ChangedNickname", newNickname);
        }

        public async Task EnterNickname(string nickname)
        {
            if (GameManager.UserRepository.Get(Context.ConnectionId) == null)
            {
                GameManager.UserRepository.Add(new User(Context.ConnectionId, nickname));
            }

            await Clients.Client(Context.ConnectionId).SendAsync("EnteredNickname", nickname);

            string serialized = JsonConvert.SerializeObject(GameManager.LobbyRepository.Lobbies);
            await Clients.Client(Context.ConnectionId).SendAsync("GetAllLobies", serialized);
        }
    }
}
