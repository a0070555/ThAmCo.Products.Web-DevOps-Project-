using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Products.Web.Data;
using ThAmCo.Products.Web.Repositories;

namespace ThAmCo.Products.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IProductsRepository _productsRepository;
        private readonly ProductsContext _context;

        public ProductsController(ILogger<ProductsController> logger, IProductsRepository productsRepository)
        {
            _logger = logger;
            _productsRepository = productsRepository;
        }


        // GET: ProductsController
        public async Task<IActionResult> Index([FromQuery] string? type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<Product> products = null;
            try
            {
                products = await _productsRepository.GetProductsAsync(type);
            }
            catch
            {
                _logger.LogWarning("Exception occurred using Products service.");
                products = Array.Empty<Product>();
            }
            return View(products.ToList());
        }

        // GET: ProductsController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                var product = await _productsRepository.GetProductAsync(id.Value);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            catch
            {
                _logger.LogWarning("Exception occurred using Products service.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdProduct = await _productsRepository.CreateProductAsync(product);
                return RedirectToAction(nameof(Details), new { id = createdProduct.ProductId });
            }
            catch
            {
                _logger.LogWarning("Exception occurred using Products service.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }


        // GET: ProductsController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                var product = await _productsRepository.GetProductAsync(id.Value);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            catch
            {
                _logger.LogWarning("Exception occurred using Products service.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Type,ProductName,Quantity,Price,Description")] Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            try
            {
                await _productsRepository.UpdateProductAsync(id, product);
            }
            catch
            {
                _logger.LogWarning("Exception occurred using Products service.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            return RedirectToAction(nameof(Index));
        }


        // DELETE: ProductsController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                var product = await _productsRepository.GetProductAsync(id.Value);
                if (product == null)
                {
                    return NotFound();
                }

                await _productsRepository.DeleteProductAsync(id.Value);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _logger.LogWarning("Exception occurred using Products service.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

    }
}
