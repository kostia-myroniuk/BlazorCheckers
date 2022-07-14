namespace BlazorCheckers.Multiplayer
{
    public class UserRepository
    {
        public List<User> Users { get; } = new List<User>();

        public void Add(User user)
        {
            if (!Users.Any(u => u.Id == user.Id))
            {
                Users.Add(user);
            }
        }

        public User? Get(string userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            return user;
        }

        public void Remove(User user)
        {
            Users.Remove(user);
        }

        public void Remove(string userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user is not null)
            {
                Users.Remove(user);
            }
        }
    }
}
