using BlazorCheckers.GameEngine;
using Newtonsoft.Json;

namespace BlazorCheckers.Data.Items
{
    public class Lobby
    {
        [JsonProperty]
        public string Name { get; }
        [JsonProperty]
        public User Creator { get; }
        [JsonProperty]
        public DateTime CreationTime { get; }
        [JsonProperty]
        public Side FirstPlayerSide { get; private set; }
        [JsonProperty]
        public GameMode GameMode { get; }
        [JsonProperty]
        public string Id { get; }
        [JsonProperty]
        public LobbyState State { get; private set; }
        [JsonProperty]
        public List<User> Users { get; }
        [JsonProperty]
        public Dictionary<string, int> Scores { get; }
        [JsonProperty]
        public Dictionary<string, bool> RematchVotes { get; }
        [JsonProperty]
        public int CurrentGame { get; private set; }

        public Lobby(string name, User creator, DateTime creationTime, Side firstPlayerSide, GameMode gameMode, string id = "")
        {
            Name = name;
            Creator = creator;
            CreationTime = creationTime;
            FirstPlayerSide = firstPlayerSide;
            GameMode = gameMode;
            Users = new List<User>();
            Scores = new Dictionary<string, int>();
            RematchVotes = new Dictionary<string, bool>();
            Id = id == "" ? Guid.NewGuid().ToString() : id;
        }

        [JsonConstructor]
        public Lobby(string name, User creator, DateTime creationTime, Side firstPlayerSide, 
            GameMode gameMode, string id, LobbyState state, List<User> users, 
            Dictionary<string, int> scores, Dictionary<string, bool> rematchVotes, int currentGame) 
        {
            Name = name;
            Creator = creator;
            CreationTime = creationTime;
            FirstPlayerSide = firstPlayerSide;
            GameMode = gameMode;
            Id = id;
            State = state;
            Users = users;
            Scores = scores;
            RematchVotes = rematchVotes;
            CurrentGame = currentGame;
        }

        public bool HasUser(User user)
        {
            return Users.Contains(user);
        }

        public bool HasUser(string userId)
        {
            return Users.Any(u => u.Id == userId);
        }

        public bool AddUser(User user)
        {
            if (State != LobbyState.WaitingForGame || Users.Count >= 2 ||
                Users.Contains(user))
            {
                return false;
            }

            Users.Add(user);
            Scores.Add(user.Id, 0);
            RematchVotes.Add(user.Id, false);

            if (Users.Count == 2)
            {
                State = LobbyState.InGame;
            }

            return true;
        }

        public bool RemoveUser(User user)
        {
            if ((State != LobbyState.WaitingForGame && State != LobbyState.AfterGame) 
                || !HasUser(user))
            {
                return false;
            }

            Users.Remove(user);
            Scores.Remove(user.Id);
            RematchVotes.Remove(user.Id);

            return true;
        }

        public bool RemoveUser(string userId)
        {
            User? user = Users.FirstOrDefault(u => u.Id == userId);
            if (user is null)
            {
                return false;
            }
            return RemoveUser(user);
        }

        public void StartGame()
        {
            State = LobbyState.InGame;
            CurrentGame++;
            foreach (var user in Users)
            {
                RematchVotes[user.Id] = false;
            }
        }

        public void EndGame(GameState gameEndState)
        {
            if (gameEndState != GameState.InProgress)
            {
                State = LobbyState.AfterGame;

                User? user = gameEndState == GameState.LightPlayerWon ?
                    GetUserBySide(Side.Light) : GetUserBySide(Side.Dark);
                if (user is not null)
                {
                    Scores[user.Id]++;
                }
            }
        }

        public void UpdateScore(GameState gameEndState)
        {
            if (gameEndState == GameState.LightPlayerWon)
            {
                User? user = GetUserBySide(Side.Light);
                if (user is not null)
                {
                    Scores[user.Id]++;
                    CurrentGame++;
                }
            }
            else if (gameEndState == GameState.DarkPlayerWon)
            {
                User? user = GetUserBySide(Side.Dark);
                if (user is not null)
                {
                    Scores[user.Id]++;
                    CurrentGame++;
                }
            }
        }

        public void SetRematchVote(string userId)
        {
            RematchVotes[userId] = true;
        }

        public bool AllPlayersVotedForRematch()
        {
            return Users.Count == 2 && Users.All(u => RematchVotes[u.Id] == true);
        }

        public string GetNickname(Side side)
        {
            User? user = GetUserBySide(side);
            return user is not null ? user.Nickname : "";
        }

        public int GetScore(Side side)
        {
            User? user = GetUserBySide(side);
            return user is not null ? Scores[user.Id] : 0;  
        }

        public User? GetUserBySide(Side side)
        {
            int userIndex = (side == FirstPlayerSide) ? ((CurrentGame + 1) % 2) : (CurrentGame % 2);
            if (Users.Count <= userIndex)
            {
                return null;
            }
            return Users[userIndex];
        }

        public Side GetSideByUserId(string userId)
        {
            var darkSideUser = GetUserBySide(Side.Dark);
            if (darkSideUser is not null && darkSideUser.Id == userId)
            {
                return Side.Dark;
            }
            return Side.Light;
        }
    }
}