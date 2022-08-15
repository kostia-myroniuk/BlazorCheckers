using BlazorCheckers.Data.Repositories;

namespace BlazorCheckers.Data
{
    public class MultiplayerData : IMultiplayerData
    {
        public IUserRepository Users => users;
        public ILobbyRepository Lobbies => lobbies;
        public IGameRepository Games => games;

        private LocalUserRepository users = new LocalUserRepository();
        private LocalLobbyRepository lobbies = new LocalLobbyRepository();
        private LocalGameRepository games = new LocalGameRepository();
    }
}
