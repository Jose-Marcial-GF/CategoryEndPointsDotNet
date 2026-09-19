using System.Reflection.Metadata.Ecma335;
using ApiEcommerce.Constants;
using ApiEcommerce.Model.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("this method is deprecated")]
        public IActionResult GetCategories()
        {
            return Ok(_mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories()));

        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ResponseCache(CacheProfileName=CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public  IActionResult GetCategory(int id)
        {
            Category? category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound($"No se ha encontrado categoría con id: {id}");
            }
            return Ok(ToCategoryDto(category));
        }

        private CategoryDto ToCategoryDto(Category category)
        {
            return _mapper.Map<CategoryDto>(category);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if( createCategoryDto == null)
            {
                ModelState.AddModelError("CustomError", "La categoría no existe");
                return BadRequest(ModelState);
            }
            if (_categoryRepository.Exists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoría ya existe");
                return BadRequest(ModelState);
            }
            Category category = _mapper.Map<Category>(createCategoryDto);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal {category}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new {id = category.Id}, category);
        }


        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto UpdateCategoryDto)
        {
            if(!_categoryRepository.Exists(id))
            {
                ModelState.AddModelError("CustomError", "La categoría no existe");
            }
            if( UpdateCategoryDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoryRepository.Exists(UpdateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "La categoría ya existe");
                return BadRequest(ModelState);
            }
            Category category = _mapper.Map<Category>(UpdateCategoryDto);
            category.Id = id;
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal {category}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  IActionResult DeleteCategory(int id)
        {
            if(!_categoryRepository.Exists(id))
            {
                ModelState.AddModelError("CustomError", "La categoría no existe");
                return StatusCode(404, ModelState);
            }
            Category? category = _categoryRepository.GetCategory(id);
            if(category == null)
            {
                ModelState.AddModelError("CustomError", "La categoría no existe");
                return StatusCode(404, ModelState); 
            }
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar {category}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
