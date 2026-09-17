using System;

using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;
using AutoMapper;
namespace ApiEcommerce.Mapping;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserLogingResponseDto>().ReverseMap();
    }
}
