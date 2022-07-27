namespace BlazorCheckers.Multiplayer
{
    public class LobbyRepository
    {
        public List<Lobby> Lobbies { get; } = new List<Lobby>();

        public void Add(Lobby lobby)
        {
            Lobbies.Add(lobby);
        }

        public Lobby Get(int lobbyId)
        {
            return Lobbies[lobbyId];
        }

        public void Remove(Lobby lobby)
        {
            Lobbies.Remove(lobby);
        }

        public Tuple<int, Lobby>? Find(User user)
        {
            for (int i = 0; i < Lobbies.Count; i++)
            {
                if (Lobbies[i].HasUser(user))
                {
                    return new Tuple<int, Lobby>(i, Lobbies[i]);
                }
            }
            return null;
        }
    }
}
