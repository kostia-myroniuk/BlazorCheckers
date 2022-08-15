using BlazorCheckers.Data.Items;

namespace BlazorCheckers.Data.Repositories
{
    public interface IUserRepository
    {
        public void Add(User user);
        public User? Get(string userId);
        public List<User> GetAll();
        public void Remove(User user);
        public void Remove(string userId);
    }
}
