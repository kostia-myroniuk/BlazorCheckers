using BlazorCheckers.GameEngine;
using Newtonsoft.Json;

namespace BlazorCheckers.Data.Items
{
    public class MultiplayerGame
    {
        [JsonProperty]
        public string Id { get; }
        [JsonProperty]
        public Lobby Lobby { get; }
        [JsonProperty]
        public Game Game { get; }

        public MultiplayerGame(Lobby lobby, Game game)
        {
            Id = Guid.NewGuid().ToString();
            Lobby = lobby;
            Game = game;
        }
    }
}
