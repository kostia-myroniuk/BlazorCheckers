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
    }
}
