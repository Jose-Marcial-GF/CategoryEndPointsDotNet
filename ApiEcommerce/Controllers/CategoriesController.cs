using ApiEcommerce.Model.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            ICollection<Category> catetories = _categoryRepository.GetCategories();
            List<CategoryDto> categoryDtos = new List<CategoryDto>();
            foreach (var category in catetories){
                categoryDtos.Add(ToCategoryDto(category));
            }
            return Ok(categoryDtos);

        }


        [HttpGet("{id:int}", Name = "GetCategory")]
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
    }
}
