using Newtonsoft.Json;

namespace BlazorCheckers.Data.Items
{
    public class User
    {
        [JsonProperty]
        public string Id { get; }
        [JsonProperty]
        public string Nickname { get; set; }

        public User(string id, string nickname)
        {
            Id = id;
            Nickname = nickname;
        }
    }
}
