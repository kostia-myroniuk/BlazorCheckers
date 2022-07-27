using BlazorCheckers.GameEngine;

namespace BlazorCheckers.Multiplayer
{
    public class MultiplayerGame
    {
        public Lobby Lobby { get; }
        public Game Game { get; }
        public bool FirstPlayerOnLightSide { get; }
        public User LightPlayer { get; }
        public User DarkPlayer { get; }

        public MultiplayerGame(Lobby lobby, bool firstPlayerOnLightSide)
        {
            Lobby = lobby;
            FirstPlayerOnLightSide = firstPlayerOnLightSide;
            Game = new Game();
        }
    }
}
