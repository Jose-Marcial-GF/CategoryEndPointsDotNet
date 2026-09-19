using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Migrations;
using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository.IRepository;

public class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext  _db;
    private string? secretKey;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _mapper = mapper;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public ApplicationUser? GetUSer(string id)
    {
        return _db.ApplicationUsers.FirstOrDefault(user => user.Id == id);
    }

    public ICollection<ApplicationUser> GetUSers()
    {
        return _db.ApplicationUsers.OrderBy(user => user.Name).ToList();
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
        ApplicationUser? user = await _db.ApplicationUsers.FirstOrDefaultAsync(user => user.UserName != null && user.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim());
        if (user == null)
        {
            return new UserLogingResponseDto()
            {
                Token = "",
                User = null,
                 Message = "Usuario no encontrado"
            };
        }
        if (userLoginDto.Password== null)
        {
            return new UserLogingResponseDto()
            {
                Token = "",
                User = null,
                 Message = "password requerido"
            };
        }

        bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!isValid)
        {
            return new UserLogingResponseDto()
            {
              Token = "",
              User = null,
              Message = "Credenciales Incorrectas"  
            };
        }

        JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Secret Key is not configurated");
        }
        IList<string> roleList = await _userManager.GetRolesAsync(user);
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        var tockenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),   
                new Claim("username", user.UserName!.ToString()),
                new Claim(ClaimTypes.Role, roleList.FirstOrDefault(string.Empty)),
            }),
            Expires =  DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = TokenHandler.CreateToken(tockenDescriptor);
        return new UserLogingResponseDto()
        {
            Token = TokenHandler.WriteToken(token),
            User = _mapper.Map<UserDataDto>(user),
            Message = "usuario logeado correctamente"
        };
    }

    public async Task<UserDataDto> Register(CreateUserDto createUserDto)
    {
        
        if (string.IsNullOrEmpty(createUserDto.UserName))
        {
            throw new ArgumentNullException("UserName is required");
        }
        if (createUserDto.Password== null)
        {
            throw new ArgumentNullException("Password is required");
        }

        var user = new ApplicationUser()
        {
            UserName = createUserDto.UserName,
            Email = createUserDto.UserName,
            NormalizedEmail = createUserDto.UserName.ToUpper(),
            Name = createUserDto.UserName
        };
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (result.Succeeded)
        {
            var userRole = createUserDto.Role ?? "User";
            var roleExists = await _roleManager.RoleExistsAsync(userRole);
            if (!roleExists)
            {
                var identityRole = new IdentityRole(userRole);
                await _roleManager.CreateAsync(identityRole);
            }
            await _userManager.AddToRoleAsync(user, userRole);
            var createdUser = _db.ApplicationUsers.FirstOrDefault(user => user.UserName == createUserDto.UserName);
            return _mapper.Map<UserDataDto>(createdUser);
        }
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new ApplicationException($"Error while creating the user; {errors}");
    }
}
