using BlazorCheckers.Data.Items;

namespace BlazorCheckers.Data.Repositories
{
    public interface ILobbyRepository
    {
        public void Add(Lobby lobby);
        public Lobby? Get(string lobbyId);
        public Lobby? GetByUserId(string userId);
        public List<Lobby> GetAll();
        public void Remove(Lobby lobby);
        public void Remove(string lobbyId);
    }
}
