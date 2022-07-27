using BlazorCheckers.GameEngine;

namespace BlazorCheckers.Multiplayer
{
    public static class GameManager
    {
        public static LobbyRepository LobbyRepository { get; } = new LobbyRepository();
        public static UserRepository UserRepository { get; } = new UserRepository();
        public static GameRepository GameRepository { get; } = new GameRepository();

        public static Lobby? GetCurrentUserLobby(string userId)
        {
            var user = UserRepository.Get(userId);
            if (user == null)
            {
                return null;
            }

            var searchResult = LobbyRepository.Find(user);
            if (searchResult == null)
            {
                return null;
            }
            return searchResult.Item2;
        }

        public static Game? GetCurrentUserGame(string userId)
        {
            var user = UserRepository.Get(userId);
            if (user == null)
            {
                return null;
            }

            var searchResult = LobbyRepository.Find(user);
            if (searchResult == null)
            {
                return null;
            }
            var lobby = searchResult.Item2;

            return GameRepository.Get(lobby);
        }
    }
}
