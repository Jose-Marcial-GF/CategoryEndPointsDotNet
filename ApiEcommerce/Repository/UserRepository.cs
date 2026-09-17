using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Migrations;
using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository.IRepository;

public class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext  _db;
    private string? secretKey;
    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }

    public User? GetUSer(int id)
    {
        return _db.Users.FirstOrDefault(user => user.Id == id);
    }

    public ICollection<User> GetUSers()
    {
        return _db.Users.OrderBy(user => user.Name).ToList();
    }

    public bool Exists(string username)
    {
        return _db.Users.Any(user =>user.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLogingResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.UserName))
        {
            return new UserLogingResponseDto()
            {
                Token = "",
                User = null,
                 Message = "El username es requerido"
            };
        }
        User? user = await _db.Users.FirstOrDefaultAsync(user => user.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim());
        if (user == null)
        {
            return new UserLogingResponseDto()
            {
                Token = "",
                User = null,
                 Message = "Usuario no encontrado"
            };
        }
        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            return new UserLogingResponseDto()
            {
                Token = "",
                User = null,
                Message = "Credenciales incorrectas"
            };
        }
        JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Secret Key is not configurated");
        }
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        var tockenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),   
                new Claim("username", user.UserName.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            }),
            Expires =  DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = TokenHandler.CreateToken(tockenDescriptor);
        return new UserLogingResponseDto()
        {
            Token = TokenHandler.WriteToken(token),
            User = new UserRegisterDto()
            {
                UserName = user.UserName,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password
            },
            Message = "usuario logeado correctamente"
        };
    }

    public async Task<User> Register(CreateUserDto createUserDto)
    {
        string encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        User user = new User()
        {
            UserName = createUserDto.UserName ?? "No UserName",
            Name = createUserDto.Name,
            Role = createUserDto.Role,
            Password = encriptedPassword
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;

    }
}
