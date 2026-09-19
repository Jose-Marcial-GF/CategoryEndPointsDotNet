using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;
using Mapster;
namespace ApiEcommerce.Mapping;

public class UserProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
        config.NewConfig<UserDto, User>();
        config.NewConfig<User, CreateUserDto>();
        config.NewConfig<CreateUserDto, User>();
        config.NewConfig<User, UserLoginDto>();
        config.NewConfig<UserLoginDto, User>();
        config.NewConfig<User, UserLogingResponseDto>();
        config.NewConfig<UserLogingResponseDto, User>();
        config.NewConfig<ApplicationUser, UserDataDto>();
        config.NewConfig<UserDataDto, ApplicationUser>();
        config.NewConfig<ApplicationUser, UserDto>();
        config.NewConfig<UserDto, ApplicationUser>();
    }
}
