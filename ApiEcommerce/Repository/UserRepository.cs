using System;
using ApiEcommerce.Migrations;
using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext  _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public User? GetUSer(int id)
    {
        return _db.Users.FirstOrDefault(user => user.Id == id);
    }

    public ICollection<User> GetUSers()
    {
        return _db.Users.OrderBy(user => user.Name).ToList();
    }

    public bool IsUnique(string username)
    {
        return _db.Users.Any(user =>Normalize(user.UserName) == Normalize(username));
    }

    private string Normalize(string str) => str.ToLower().Trim();

    public Task<UserLogingResponseDto> Login(UserLoginDto userLoginDto)
    {
        throw new NotImplementedException();
    }

    public Task<User> Register(CreateUserDto createUserDto)
    {
        throw new NotImplementedException();
    }
}
