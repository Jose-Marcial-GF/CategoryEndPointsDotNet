using ApiEcommerce.Model.Dtos;
using ApiEcommerce.Model;
using ApiEcommerce.Repository.IRepository;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiEcommerce.Constants;
using Asp.Versioning;

namespace ApiEcommerce.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]
    [ApiController]
    public class UserController : ControllerBase{
    public readonly IMapper _mapper;
    public readonly IUserRepository _userRepository;

        public UserController(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet(Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetUsers()
        {
            return Ok(_mapper.Map<List<UserDto>>(_userRepository.GetUSers()));  
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("CustomError", "El id tiene que existir");
                return BadRequest(ModelState);
            }
            ApplicationUser? user = _userRepository.GetUSer(id);
            if (user == null)
            {
                return NotFound("User not foud");
            }
            return Ok(_mapper.Map<UserDto>(user));
        }

        [AllowAnonymous]
        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(createUserDto.UserName))
            {
                return BadRequest("Username is required");
            }
            if (_userRepository.Exists(createUserDto.UserName))
            {
                return BadRequest("the user already exists");
            }
            UserDataDto user = await _userRepository.Register(createUserDto);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtRoute("GetUser", new {id = user.Id}, user);
        }

        [AllowAnonymous]
        [HttpPost("Login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLogingDto)
        {
            if (userLogingDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UserLogingResponseDto userLogingResponseDto = await _userRepository.Login(userLogingDto);
            if (userLogingResponseDto == null)
            {
                return Unauthorized();
            }
            return Ok(userLogingResponseDto);
        }
    }
    

}
