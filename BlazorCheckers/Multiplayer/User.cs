namespace BlazorCheckers.Multiplayer
{
    public class User
    {
        public string Id { get; }
        public string Nickname { get; set; }

        public User(string id, string nickname)
        {
            Id = id;
            Nickname = nickname;
        }
    }
}
