using TestExerciseSK.Models;

namespace TestExerciseSK.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly TestExerciseSKDbContext _context;
        public const String EMPTY_STRING = "";

        public UserRepository(TestExerciseSKDbContext context)
        {
            _context = context;
        }

        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(string id)
        {
            return _context.Users.Find(id);
        }
        
        public User Create(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
           
            return user;
        }
        public bool Update(User user)
        {
            var userToUpdate = _context.Users.Find(user.userID);

            if (userToUpdate != null)
            {
                if (user.firstName != null && !EMPTY_STRING.Equals(user.firstName))
                {
                    userToUpdate.firstName = user.firstName;
                }
                if (user.lastName != null && !EMPTY_STRING.Equals(user.lastName))
                {
                    userToUpdate.lastName = user.lastName;
                }
                if (user.email != null && !EMPTY_STRING.Equals(user.email))
                {
                    userToUpdate.email = user.email;
                }
                if (user.birthDate != null && !EMPTY_STRING.Equals(user.birthDate))
                {
                    userToUpdate.birthDate = user.birthDate;
                    userToUpdate.retirementDate = user.retirementDate;
                }
            }
            int updatedRecords = _context.SaveChanges();
            return updatedRecords > 0;
        }

        public bool Delete(string id)
        {
            var userToDelete = _context.Users.Find(id);
            if (userToDelete != null)
            {
                _context.Users.Remove(userToDelete);
                int deletedRecords = _context.SaveChanges();
                return deletedRecords > 0;
            } else 
            { 
                return false; 
            }
            
        }

    }
}
