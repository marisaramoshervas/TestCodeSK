using TestExerciseSK.Models;

namespace TestExerciseSK.DAL
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        User GetUserById(string id);
        User Create(User user);
        bool Update(User user);
        bool Delete(string id);


    }
}
