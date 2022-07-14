namespace BlazorCheckers.Multiplayer
{
    public class Lobby
    {
        public string Name { get; }
        public List<User> Users { get; }
        public User Creator { get; }
        public LobbyState State { get; private set; }
        public DateTime CreationTime { get; }

        public Lobby(string name, User creator, DateTime creationTime)
        {
            Users = new List<User>();
            Name = name;
            Creator = creator;
            CreationTime = creationTime;
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
    }
}