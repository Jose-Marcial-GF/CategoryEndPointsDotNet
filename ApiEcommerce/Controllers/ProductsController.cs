using System.Reflection.Metadata.Ecma335;
using ApiEcommerce.Constants;
using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;
using ApiEcommerce.Model.Dtos.Responses;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IProductRepository _productRepository;
        public readonly ICategoryRepository _categoryRepository;
        public readonly IMapper _mapper;
        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }



        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProducts()
        {
            return Ok(_productRepository.GetProducts().Select(product => _mapper.Map<ProductDto>(product)).ToList());
        }

        

        [AllowAnonymous]
        [HttpGet("{productId:int}", Name="GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProduct(int productId)
        {
           var product = _productRepository.GetProduct(productId);
           if (product == null)
           {
                return NotFound($"El producto con id {productId} no existe");
           }
            return Ok(_mapper.Map<ProductDto>(product));
        }


        
        [AllowAnonymous]
        [HttpGet("paged", Name="GetProductsInPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProductsInPage([FromQuery] int pageNumber=1, [FromQuery] int pageSize=5)
        {
            if(pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("wrong paged parameters");
            }
            int total_products = _productRepository.GetTotalProducts();
            int Totalpages = (int)Math.Ceiling(((double)total_products/pageSize));
            if (pageNumber > Totalpages)
            {
                return NotFound("page out of bounds");
            }
            return Ok(new PaginationResponse<ProductDto>{
               PageNumber = pageNumber,
               PageSize = pageSize,
               TotalPages = Totalpages,
               Items = _mapper.Map<List<ProductDto>>(_productRepository.GetProductsInPages(pageNumber, pageSize))
            });

        }

        [AllowAnonymous]
        [HttpGet("SearchProductByCategoy/{categroyId:int}", Name="GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetProductsForCategory(int categroyId)
        {
            if (categroyId <= 0 )
            {
                return NotFound($"La categoría tiene que ser >= 0 existe");
            }
            return Ok(_mapper.Map<List<ProductDto>>(_productRepository.GetProductsForCategory(categroyId)));
        }


        [AllowAnonymous]
        [HttpGet("SearchProductByNameDescription/{searchTerm}", Name="GetProductsByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult SearchProductByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return NotFound($"El nombre o description no puede estár vacío");
            }
            return Ok(_mapper.Map<List<ProductDto>>(_productRepository.SearchProducts(searchTerm)));
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if( createProductDto == null )
            {
                ModelState.AddModelError("CustomError", "El producto no existe");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.Exists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", "La categoría no existe");
                return BadRequest(ModelState);
            }
            if (_productRepository.Exists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "El Producto ya existe");
                return BadRequest(ModelState);
            }
            Product product = _mapper.Map<Product>(createProductDto);

            if(createProductDto.Image != null)
            {
                UploadProductImage(createProductDto, product);
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal {product}");
                return StatusCode(500, ModelState);
            }
            ProductDto productDto = _mapper.Map<ProductDto>(_productRepository.GetProduct(product.Id));
            return CreatedAtRoute("GetProduct", new {productId = product.Id}, productDto);
        }


        [HttpDelete("{productId:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  IActionResult DeleteProduct(int productId)
        {
            if( !_productRepository.Exists(productId))
            {
                ModelState.AddModelError("CustomError", "El producto no existe");
                return BadRequest(ModelState);
            }
            Product? product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                ModelState.AddModelError("CustomError", "El producto no existe");
                return BadRequest(ModelState);
            }
            if(!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el producto");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


        [HttpPatch("buyProduct/{name}/{amount:int}", Name="Buy Product")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult BuyProduct(string name, int amount)
        {
            if(string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("CustomError", "El nombre no puede estar vacío");
                return BadRequest(ModelState);
            }
            if(amount <= 0)
            {
                ModelState.AddModelError("CustomError", "La cantidad tiene que ser >= 0");
                return BadRequest(ModelState);
            }
            if(!_productRepository.BuyProduct(name, amount))
            {
                ModelState.AddModelError("CustomError", $"No se pudo realizar la compra");
                return BadRequest(ModelState);
            }
            string amountPlural = amount == 1 ? "unidad": "unidades";
            return Ok($"Se han comprado {amount} {amountPlural} '{name}'");
        }

        [HttpPut("{productId:int}", Name="UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  IActionResult UpdateProduct(int productId, [FromForm] UpdateProductDto updateProductDto)
        {
            if( updateProductDto == null )
            {
                ModelState.AddModelError("CustomError", "El producto no existe");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.Exists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", "La categoría no existe");
                return BadRequest(ModelState);
            }
            if (!_productRepository.Exists(productId))
            {
                ModelState.AddModelError("CustomError", "El Producto no existe");
                return BadRequest(ModelState);
            }
            Product product = _mapper.Map<Product>(updateProductDto);
            product.Id = productId;

            if(updateProductDto.Image != null)
            {
                UploadProductImage(updateProductDto, product);
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }


            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal {product}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        private void UploadProductImage(dynamic productDto, Product product)
        {
            string filename = product.Id + Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
            var filePath = Path.Combine(imagesFolder, filename);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
            using var fileStream = new FileStream(filePath, FileMode.Create);
            productDto.Image.CopyTo(fileStream);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            product.ImageUrl = $"{baseUrl}/ProductsImages/{filename}";
            product.ImageUrlLocal = filePath;
        }
    }

}
