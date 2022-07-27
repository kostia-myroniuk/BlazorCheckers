using BlazorCheckers.GameEngine;
using Newtonsoft.Json;

namespace BlazorCheckers.Multiplayer
{
    public class Lobby
    {
        public string Name { get; }
        public User Creator { get; }
        public DateTime CreationTime { get; }
        [JsonProperty]
        public LobbyState State { get; private set; }
        public List<User> Users { get; }

        public Lobby(string name, User creator, DateTime creationTime)
        {
            Name = name;
            Creator = creator;
            CreationTime = creationTime;
            Users = new List<User>();
        }

        public bool HasUser(User user)
        {
            return Users.Contains(user);
        }

        public bool AddUser(User user)
        {
            if (HasUser(user) || Users.Count > 1)
            {
                return false;
            }

            Users.Add(user);
            if (Users.Count == 2)
            {
                State = LobbyState.Game;
            }
            return true;
        }

        public bool RemoveUser(User user)
        {
            if (State != LobbyState.WaitingForGame || !HasUser(user))
            {
                return false;
            }

            Users.Remove(user);
            return true;
        }

        public User GetUser(Side side)
        {
            return Users[(int)side];
        }
    }
}