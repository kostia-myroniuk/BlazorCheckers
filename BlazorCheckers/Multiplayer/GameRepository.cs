using BlazorCheckers.GameEngine;

namespace BlazorCheckers.Multiplayer
{
    public class GameRepository
    {
        public Dictionary<Lobby, Game> LobbyGames { get; } = new Dictionary<Lobby, Game>();

        public Game Add(Lobby lobby)
        {
            Game game = new Game();
            LobbyGames.Add(lobby, game);
            return game;
        }

        public void Remove(Lobby lobby)
        {
            LobbyGames.Remove(lobby);
        }

        public Game Get(Lobby lobby)
        {
            Game game;
            LobbyGames.TryGetValue(lobby, out game);
            return game;
        }
    }
}
