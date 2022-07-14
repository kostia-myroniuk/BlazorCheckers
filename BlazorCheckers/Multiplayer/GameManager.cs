namespace BlazorCheckers.Multiplayer
{
    public static class GameManager
    {
        public static LobbyRepository LobbyRepository { get; } = new LobbyRepository();
        public static UserRepository UserRepository { get; } = new UserRepository();
    }
}
