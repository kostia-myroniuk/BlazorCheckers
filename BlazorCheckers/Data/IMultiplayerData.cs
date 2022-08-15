using BlazorCheckers.Data.Repositories;

namespace BlazorCheckers.Data
{
    public interface IMultiplayerData
    {
        public IUserRepository Users { get; }
        public ILobbyRepository Lobbies { get; }
        public IGameRepository Games { get; }
    }
}
