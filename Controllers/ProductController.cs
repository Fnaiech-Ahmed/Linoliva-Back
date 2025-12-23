using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.DTO.ProductDTOs;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public ProductController(IProductService productService, ITokenAuthenticationService tokenAuthService)
        {
            _productService = productService;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(params string[] requiredPolicies)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            foreach (var policy in requiredPolicies)
            {
                if (_tokenAuthService.IsUserAuthorized(token, policy))
                {
                    return true;
                }
            }
            return false;
        }

        [HttpPost("add-product")]
        public async Task<ActionResult<bool>> AddProduct(ProductCreateDTO productCreateDTO)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "ProductPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à ajouter des produits.");
            }

            if (productCreateDTO == null)
            {
                return BadRequest("Le produit est null.");
            }

            return await _productService.AddProduct(productCreateDTO);
        }

        [HttpGet("get-list-products")]
        public async Task<ActionResult<List<ProductDTO>>> GetProducts()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "ProductPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter la liste des produits.");
            }

            return Ok(await _productService.GetProducts());
        }

        [HttpGet("externe-get-list-products")]
        public async Task<ActionResult<List<ProductDTO>>> ExterneGetProducts()
        {
            try
            {
                return Ok(await _productService.GetProducts());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("get-product-by-id/{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "ProductPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce produit.");
            }

            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("get-product-by-barcode/{barcode}")]
        public async Task<ActionResult<ProductDTO>> GetProductByBarCode(string barcode)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "ProductPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce produit.");
            }

            var product = await _productService.GetProductByBarCode(barcode);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut("update-product/{productId}")]
        public async Task<ActionResult<bool>> UpdateProduct(int productId, ProductUpdateDTO product)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "ProductSeniorPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à mettre à jour ce produit.");
            }

            if (product == null)
            {
                return BadRequest("Le produit est null. Impossible de mettre à jour.");
            }

            return await _productService.UpdateProduct(productId, product);
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(int id)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "ProductSeniorPolicy", "AdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à supprimer ce produit.");
            }

            return await _productService.DeleteProduct(id);
        }

        [HttpPut("desactiver/{productId}")]
        public async Task<IActionResult> DesactiverProduit(int productId)
        {
            if (IsUserAuthorized("ProductSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    await _productService.DesactiverProduitAsync(productId);
                    return NoContent(); // 204 No Content si l'opération réussit
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(ex.Message); // 404 si le produit n'est pas trouvé            
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Une erreur est survenue : " + ex.Message); // 500 si une erreur se produit            
                }
            }
            else
            {
                return Unauthorized("Vous n'êtes pas autorisé à ajouter des produits.");
            }

        }
    }
}
