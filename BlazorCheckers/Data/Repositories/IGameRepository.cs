using BlazorCheckers.Data.Items;

namespace BlazorCheckers.Data.Repositories
{
    public interface IGameRepository
    {
        public void Add(MultiplayerGame game);
        public MultiplayerGame? Get(string gameId);
        public MultiplayerGame? GetByLobbyId(string lobby);
        public MultiplayerGame? GetByUserId(string userId);
        public void Remove(MultiplayerGame game);
        public void Remove(string gameId);
    }
}
