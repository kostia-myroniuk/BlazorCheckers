using BlazorCheckers.Data.Items;

namespace BlazorCheckers.Data.Repositories
{
    public class LocalGameRepository : IGameRepository
    {
        private List<MultiplayerGame> games = new List<MultiplayerGame>();

        public void Add(MultiplayerGame game)
        {
            if (!games.Any(g => g.Id == game.Id))
            {
                games.Add(game);
            }
        }

        public MultiplayerGame? Get(string gameId)
        {
            return games.FirstOrDefault(g => g.Id == gameId);
        }

        public MultiplayerGame? GetByLobbyId(string lobbyId)
        {
            return games.FirstOrDefault(g => g.Lobby.Id == lobbyId);
        }

        public MultiplayerGame? GetByUserId(string userId)
        {
            return games.FirstOrDefault(g => g.Lobby.HasUser(userId));
        }

        public void Remove(MultiplayerGame game)
        {
            games.Remove(game);
        }

        public void Remove(string gameId)
        {
            var game = games.FirstOrDefault(g => g.Id == gameId);
            if (game != null)
            {
                games.Remove(game);
            }
        }
    }
}
