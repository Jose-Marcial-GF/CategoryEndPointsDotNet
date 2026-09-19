using System;
using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUSers();
    ApplicationUser? GetUSer(string id);
    bool Exists(string username);
    Task<UserLogingResponseDto> Login(UserLoginDto userLoginDto);

    Task<UserDataDto> Register (CreateUserDto createUserDto);

}
