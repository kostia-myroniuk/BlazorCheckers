using Microsoft.AspNetCore.SignalR;
using BlazorCheckers.Data;
using Newtonsoft.Json;
using BlazorCheckers.GameEngine;
using BlazorCheckers.Data.Items;

namespace BlazorCheckers.Hubs
{
    public class MultiplayerHub : Hub
    {
        private IMultiplayerData data;

        public MultiplayerHub(IMultiplayerData data)
        {
            this.data = data;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userId = Context.ConnectionId;
            data.Users.Remove(userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SignIn(string nickname)
        {
            string userId = Context.ConnectionId;
            User? user = data.Users.Get(userId);

            if (user is null)
            {
                user = new User(userId, nickname);
            }
            else
            {
                user.Nickname = nickname;
            }
            data.Users.Add(user);

            string serializedLobbies = JsonConvert.SerializeObject(data.Lobbies.GetAll());
            await Clients.Client(userId).SendAsync("SignedIn", nickname, serializedLobbies);
        }

        public async Task AddLobby(string name, LobbySideOption sideOption, GameMode gameMode)
        {
            string userId = Context.ConnectionId;
            var user = data.Users.Get(userId);
            if (user is null)
            {
                return;
            }

            Lobby lobby = new Lobby(name, user, DateTime.Now, GetFirstSide(sideOption), gameMode);
            data.Lobbies.Add(lobby);

            string lobbySerialized = JsonConvert.SerializeObject(lobby);
            await Clients.All.SendAsync("LobbyAdded", lobbySerialized);
        }

        private Side GetFirstSide(LobbySideOption lobbyOption)
        {
            if (lobbyOption == LobbySideOption.Random)
            {
                Random random = new Random();
                return (Side)random.Next(0, 2);
            }

            return (lobbyOption == LobbySideOption.Light) ? Side.Light : Side.Dark;
        }

        public async Task JoinLobby(string lobbyId)
        {
            string userId = Context.ConnectionId;
            User? user = data.Users.Get(userId);
            Lobby? lobby = data.Lobbies.Get(lobbyId);

            if (user is null || lobby is null)
            {
                return;
            }

            bool joinedLobby = lobby.AddUser(user);
            if (!joinedLobby)
            {
                return;
            }

            string serializedLobby = JsonConvert.SerializeObject(lobby);

            if (lobby.State == LobbyState.InGame)
            {
                await CreateGame(lobby);
                foreach (var appUser in data.Users.GetAll().Where(u => !lobby.HasUser(u.Id)))
                {
                    await Clients.Client(appUser.Id).SendAsync("LobbyUpdated", serializedLobby);
                }
            }
            else
            {
                foreach (var appUser in data.Users.GetAll())
                {
                    string method = appUser.Id == userId ? "JoinedLobby" : "LobbyUpdated";
                    await Clients.Client(appUser.Id).SendAsync(method, serializedLobby);
                }
            }
        }

        public async Task LeaveLobby()
        {
            string userId = Context.ConnectionId;
            User? user = data.Users.Get(userId);
            Lobby? lobby = data.Lobbies.GetByUserId(userId);
            if (user == null || lobby == null)
            {
                return;
            }

            bool leftLobby = lobby.RemoveUser(user);
            if (!leftLobby)
            {
                return;
            }

            string serializedLobby = JsonConvert.SerializeObject(lobby);
            foreach (var appUser in data.Users.GetAll())
            {
                string method = appUser.Id == userId ? "LeftLobby" : "LobbyUpdated";
                await Clients.Client(appUser.Id).SendAsync(method, serializedLobby);
            }
        }

        public async Task ApplyRegularMove(string moveSerialized)
        {
            await ApplyMove(moveSerialized, false);
        }

        public async Task ApplyCaptureMove(string moveSerialized)
        {
            await ApplyMove(moveSerialized, true);
        }

        public async Task ApplyMove(string moveSerialized, bool isCapture)
        {
            string userId = Context.ConnectionId;
            var move = JsonConvert.DeserializeObject<Move>(moveSerialized);
            MultiplayerGame? mpGame = data.Games.GetByUserId(userId);

            if (mpGame is null || move is null)
            {
                return;
            }

            GameState previousState = mpGame.Game.CurrentState;

            if (isCapture)
            {
                mpGame.Game.ApplyCaptureMove(move);
            }
            else
            {
                mpGame.Game.ApplyRegularMove(move);
            }

            if (mpGame.Game.CurrentState != GameState.InProgress &&
                previousState == GameState.InProgress)
            {
                await EndGame(mpGame);
            }
            else
            {
                string serializedGame = JsonConvert.SerializeObject(mpGame.Game);
                foreach (var lobbyUser in mpGame.Lobby.Users)
                {
                    await Clients.Client(lobbyUser.Id).SendAsync("GameUpdated", serializedGame);
                }
            }
        }

        public async Task GiveUp()
        {
            string userId = Context.ConnectionId;
            MultiplayerGame? mpGame = data.Games.GetByUserId(userId);
            if (mpGame is null)
            {
                return;
            }

            Side playerSide = mpGame.Lobby.GetSideByUserId(userId);
            mpGame.Game.EndGameEarly(playerSide);
            await EndGame(mpGame);
        }

        public async Task AskForRematch()
        {
            string userId = Context.ConnectionId;
            MultiplayerGame? mpGame = data.Games.GetByUserId(userId);
            if (mpGame is null)
            {
                return;
            }

            mpGame.Lobby.SetRematchVote(userId);
            if (mpGame.Lobby.AllPlayersVotedForRematch())
            {
                data.Games.Remove(mpGame);
                var lobby = mpGame.Lobby;
                await CreateGame(lobby);
            }
        }

        public async Task LeaveGame()
        {
            string userId = Context.ConnectionId;
            MultiplayerGame? mpGame = data.Games.GetByUserId(userId);
            if (mpGame is null)
            {
                return;
            }

            mpGame.Lobby.RemoveUser(userId);

            data.Games.Remove(mpGame);
            data.Lobbies.Remove(mpGame.Lobby.Id);

            string serializedLobby = JsonConvert.SerializeObject(mpGame.Lobby);
            await Clients.All.SendAsync("LobbyRemoved", serializedLobby);
        }

        private async Task CreateGame(Lobby lobby)
        {
            lobby.StartGame();
            var game = new Game(lobby.GameMode);
            var newMpGame = new MultiplayerGame(lobby, game);
            data.Games.Add(newMpGame);

            string serializedGame = JsonConvert.SerializeObject(game);
            string serializedLobby = JsonConvert.SerializeObject(lobby);

            foreach (var lobbyUser in lobby.Users)
            {
                Side playerSide = lobby.GetSideByUserId(lobbyUser.Id);

                await Clients.Client(lobbyUser.Id).SendAsync("GameStarted",
                    serializedGame, serializedLobby, playerSide);
            }
        }

        private async Task EndGame(MultiplayerGame mpGame)
        {
            mpGame.Lobby.EndGame(mpGame.Game.CurrentState);

            string serializedGame = JsonConvert.SerializeObject(mpGame.Game);
            string serializedLobby = JsonConvert.SerializeObject(mpGame.Lobby);

            foreach (var lobbyUser in mpGame.Lobby.Users)
            {
                await Clients.Client(lobbyUser.Id).SendAsync("GameEnded", serializedGame, serializedLobby);
            }
        }
    }
}
