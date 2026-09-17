using System;
using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetUSers();
    User? GetUSer(int id);
    bool IsUnique(string username);
    Task<UserLogingResponseDto> Login(UserLoginDto userLoginDto);

    Task<User> Register (CreateUserDto createUserDto);

}
