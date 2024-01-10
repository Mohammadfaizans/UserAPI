using Microsoft.EntityFrameworkCore;

namespace UserAPI.Repositories
{
    public class UsersRepository : IRepository<User>
    {
        private AppDbContext _Dbcontext;

        public UsersRepository(AppDbContext appDbContext)
        {
                _Dbcontext = appDbContext;
        }
        public bool Add(User entity)
        {
            _Dbcontext.Add(entity);
            int recordsAffected = _Dbcontext.SaveChanges();
            if (recordsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool Delete(object key)
        {
            throw new NotImplementedException();
        }
      
        public IEnumerable<User> Get()
        {
            var users = _Dbcontext.Users.ToList();
            return users;
        }

        public User Get(Guid key)
        {
            var user = _Dbcontext.Users.Find(key);
            return user;
        }

        public bool Update(User entity)
        {
            var user = _Dbcontext.Users.AsNoTracking().FirstOrDefault(c => c.Id == entity.Id);//Find(entity.Id);
            //if (user == null)
            //{
              //  throw new OvrsException("Customer not found");
            //}
            _Dbcontext.Users.Update(entity);
            int recordsAffected = _Dbcontext.SaveChanges();
            if (recordsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
