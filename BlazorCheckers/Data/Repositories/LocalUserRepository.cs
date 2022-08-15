using BlazorCheckers.Data.Items;

namespace BlazorCheckers.Data.Repositories
{
    public class LocalUserRepository : IUserRepository
    {
        public List<User> users = new List<User>();

        public void Add(User user)
        {
            if (!users.Any(u => u.Id == user.Id))
            {
                users.Add(user);
            }
        }

        public User? Get(string userId)
        {
            return users.FirstOrDefault(u => u.Id == userId);
        }

        public List<User> GetAll()
        {
            return new List<User>(users);
        }

        public void Remove(User user)
        {
            users.Remove(user);
        }

        public void Remove(string userId)
        {
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user is not null)
            {
                users.Remove(user);
            }
        }
    }
}
