using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThAmCo.Products.Web.Controllers;
using ThAmCo.Products.Web.Data;
using ThAmCo.Products.Web.Repositories;

namespace ThAmCo.Products.Web.Tests
{
    public class ProductsControllerTest
    {

        private Product[] GetTestProducts() => new Product[]
        {
            new Product { ProductId = 1, Type="Standard", ProductName = "Product A", Price = 12.50, Quantity = 4, Description = "The first product in our catalogue"},
            new Product { ProductId = 2, Type="Standard", ProductName = "Product B", Price = 5.20, Quantity = 25, Description = "The second product in our catalogue"},
            new Product { ProductId = 3, Type="Custom", ProductName = "Product C", Price = 20.99, Quantity = 50, Description = "The third product in our catalogue"},
            new Product { ProductId = 4, Type="Standard", ProductName = "Product D", Price = 5.80, Quantity = 100, Description = "The fourth product in our catalogue"},
            new Product { ProductId = 5, Type="Custom", ProductName = "Product E", Price = 60.00, Quantity = 5, Description = "The fifth product in our catalogue"},
            new Product { ProductId = 6, Type="Custom", ProductName = "Product F", Price = 45.75, Quantity = 10, Description = "The sixth product in our catalogue"}
        };

        [Fact]
        public async Task GetIndex_WithInvalidModelState_ShouldBadResult()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>();
            var controller = new ProductsController(mockLogger.Object, mockProducts.Object);
            controller.ModelState.AddModelError("Something", "Something");

            //Act
            var result = await controller.Index(null);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetIndex_WithNullType_ShouldViewServiceEnumerable()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>(MockBehavior.Strict);
            var expected = GetTestProducts();
            mockProducts.Setup(p => p.GetProductsAsync(null))
                        .ReturnsAsync(expected)
                        .Verifiable();
            var controller = new ProductsController(mockLogger.Object, mockProducts.Object);

            //Act
            var result = await controller.Index(null);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.ViewData.Model);
            Assert.Equal(expected.Length, model.Count());
            Assert.Equal("Index", viewResult.ViewName);
            Assert.True(viewResult.ViewData.ModelState.IsValid);

            mockProducts.Verify(p => p.GetProductsAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetIndex_WithType_ShouldViewServiceEnumerable()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>(MockBehavior.Strict);
            var expected = GetTestProducts();
            mockProducts.Setup(p => p.GetProductsAsync("test type"))
                       .ReturnsAsync(expected)
                       .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                   mockProducts.Object);

            // Act
            var result = await controller.Index("test type");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.ViewData.Model);
            Assert.Equal(expected.Length, model.Count());
            Assert.Equal("Index", viewResult.ViewName);
            Assert.True(viewResult.ViewData.ModelState.IsValid);

            mockProducts.Verify(p => p.GetProductsAsync("test type"), Times.Once);
        }

        [Fact]
        public async Task GetIndex_WhenBadServiceCall_ShouldViewEmptyEnumerable()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>(MockBehavior.Strict);
            mockProducts.Setup(p => p.GetProductsAsync(null))
                       .ThrowsAsync(new Exception())
                       .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                   mockProducts.Object);

            // Act
            var result = await controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(
                viewResult.ViewData.Model);
            Assert.Empty(model);
            mockProducts.Verify(p => p.GetProductsAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetDetails_WithInvalidModelState_ShouldBadResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>();
            var controller = new ProductsController(mockLogger.Object,
                                                   mockProducts.Object);
            controller.ModelState.AddModelError("Something", "Something");

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetDetails_WithNullId_ShouldBadResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>();
            var controller = new ProductsController(mockLogger.Object,
                                                   mockProducts.Object);

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetDetails_WhenBadServiceCall_ShouldInternalError()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>(MockBehavior.Strict);
            mockProducts.Setup(p => p.GetProductAsync(3))
                       .ThrowsAsync(new Exception())
                       .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                   mockProducts.Object);

            // Act
            var result = await controller.Details(3);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable,
                         statusCodeResult.StatusCode);
            mockProducts.Verify(p => p.GetProductAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetDetails_WithUnknownId_ShouldNotFound()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>(MockBehavior.Strict);
            mockProducts.Setup(p => p.GetProductAsync(13))
                       .ReturnsAsync((Product)null)
                       .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                   mockProducts.Object);

            // Act
            var result = await controller.Details(13);

            // Assert
            var statusCodeResult = Assert.IsType<NotFoundResult>(result);
            mockProducts.Verify(p => p.GetProductAsync(13), Times.Once);
        }

        [Fact]
        public async Task GetDetails_WithId_ShouldViewServiceObject()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ProductsController>>();
            var mockProducts = new Mock<IProductsRepository>(MockBehavior.Strict);
            var expected = GetTestProducts().First();
            mockProducts.Setup(p => p.GetProductAsync(expected.ProductId))
                       .ReturnsAsync(expected)
                       .Verifiable();
            var controller = new ProductsController(mockLogger.Object,
                                                   mockProducts.Object);

            // Act
            var result = await controller.Details(expected.ProductId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Product>(viewResult.ViewData.Model);
            Assert.Equal(expected.ProductId, model.ProductId);
            // FIXME: could assert other result property values here

            mockProducts.Verify(p => p.GetProductAsync(expected.ProductId), Times.Once);
        }
    }
}
