using LittleViet.Data.Domains;
using LittleViet.Data.ServiceHelper;
using LittleViet.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LittleViet.Api.Controllers;

    [Route("api/product")]
    [ApiController]
    public class ProductController : Controller
    {
        private IProductDomain _productDomain;
        public ProductController(IProductDomain productDomain)
        {
            _productDomain = productDomain;
        }

        [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
        [HttpPost("")]
        public IActionResult Create(CreateProductVM productVM)
        {
            try
            {
                var result = _productDomain.Create(productVM);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
        [HttpPut("id")]
        public IActionResult Update(Guid id, UpdateProductVM productVM)
        {
            try
            {
                productVM.Id = id;
                var result = _productDomain.Update(productVM);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [AuthorizeRoles(Role.ADMIN, Role.MANAGER)]
        [HttpDelete("id")]
        public IActionResult DeactiveAccount(Guid id)
        {
            try
            {
                var result = _productDomain.Deactivate(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }

        [HttpGet("landing-page")]
        public IActionResult GetProductsForLP()
        {
            try
            {
                var result = _productDomain.GetActivesForLP();
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResponseVM { Message = e.Message, Success = false });
            }
        }
    }

