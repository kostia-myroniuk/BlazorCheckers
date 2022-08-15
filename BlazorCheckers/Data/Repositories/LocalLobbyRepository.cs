using BlazorCheckers.Data.Items;

namespace BlazorCheckers.Data.Repositories
{
    public class LocalLobbyRepository : ILobbyRepository
    {
        private List<Lobby> lobbies = new List<Lobby>();

        public void Add(Lobby lobby)
        {
            if (!lobbies.Any(l => l.Id == lobby.Id))
            {
                lobbies.Add(lobby);
            }
        }

        public Lobby? Get(string lobbyId)
        {
            return lobbies.FirstOrDefault(l => l.Id == lobbyId);
        }

        public Lobby? GetByUserId(string userId)
        {
            return lobbies.FirstOrDefault(l => l.HasUser(userId));
        }

        public List<Lobby> GetAll()
        {
            return new List<Lobby>(lobbies);
        }

        public void Remove(Lobby lobby)
        {
            lobbies.Remove(lobby);
        }

        public void Remove(string lobbyId)
        {
            var lobby = lobbies.FirstOrDefault(l => l.Id == lobbyId);
            if (lobby is not null)
            {
                lobbies.Remove(lobby);
            }
        }
    }
}
