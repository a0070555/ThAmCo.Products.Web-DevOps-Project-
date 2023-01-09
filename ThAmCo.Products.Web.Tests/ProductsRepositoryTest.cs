using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThAmCo.Products.Web.Data;
using ThAmCo.Products.Web.Repositories;

namespace ThAmCo.Products.Web.Tests
{
    public class ProductsRepositoryTest
    {
        private Mock<HttpMessageHandler> CreateHttpMock(HttpStatusCode expectedCode, string expectedJson)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = expectedCode
            };
            if (expectedJson != null)
            {
                response.Content = new StringContent(expectedJson, Encoding.UTF8, "application/json");
            }
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response)
                .Verifiable();
            return mock;       
        }

        private IProductsRepository CreateProductsRepository(HttpClient client)
        {
            var mockConfiguration = new Mock<IConfiguration>(MockBehavior.Strict);
            mockConfiguration.Setup(c => c["WebServices:Products:BaseURL"])
                             .Returns("http://example.com");
            return new ProductsRepository(client, mockConfiguration.Object);
        }

        [Fact]
        public async Task GetReviewAsync_WithValid_ShouldOkEntity()
        {
            //Arrange
            var expectedResult = new Product { ProductId = 1, ProductName = "Product A", Price = 12.50, Quantity = 4, Description = "The first product in our catalogue" };
            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("http://example.com/api/products/1");
            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsRepository(client);

            //Act
            var result = await service.GetProductAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.ProductId, result.ProductId);
            Assert.Equal(expectedResult.ProductName, result.ProductName);
            Assert.Equal(expectedResult.Price, result.Price);
            Assert.Equal(expectedResult.Quantity, result.Quantity);

            mock.Protected()
                .Verify("SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(
                    req => req.Method == HttpMethod.Get
                           && req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetReviewAsync_WithInvalid_ShouldReturnNull()
        {
            //Arrange
            var expectedUri = new Uri("http://example.com/api/products/100");
            var mock = CreateHttpMock(HttpStatusCode.NotFound, null);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsRepository(client);

            //Act
            var result = await service.GetProductAsync(100);

            //Asssert
            Assert.Null(result);
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(
                            req => req.Method == HttpMethod.Get
                                   && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                        );
        }

        [Fact]
        public async Task GetProductAsync_OnHttpBad_ShouldThrow()
        {
            //Arrange
            var expectedUri = new Uri("http://example.com/api/products/1");
            var mock = CreateHttpMock(HttpStatusCode.ServiceUnavailable, null);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsRepository(client);

            //Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => service.GetProductAsync(1));
        }

        [Fact]
        public async Task GetProductsAsync_WithNull_ShouldReturnAll()
        {
            //Arrange
            var expectedResult = new Product[]
            {
                new Product { ProductId = 1, Type="Standard", ProductName = "Product A", Price = 12.50, Quantity = 4, Description = "The first product in our catalogue"},
                new Product { ProductId = 2, Type="Standard", ProductName = "Product B", Price = 5.20, Quantity = 25, Description = "The second product in our catalogue"},
                new Product { ProductId = 3, Type="Custom", ProductName = "Product C", Price = 20.99, Quantity = 50, Description = "The third product in our catalogue"},
                new Product { ProductId = 4, Type="Standard", ProductName = "Product D", Price = 5.80, Quantity = 100, Description = "The fourth product in our catalogue"},
                new Product { ProductId = 5, Type="Custom", ProductName = "Product E", Price = 60.00, Quantity = 5, Description = "The fifth product in our catalogue"},
                new Product { ProductId = 6, Type="Custom", ProductName = "Product F", Price = 45.75, Quantity = 10, Description = "The sixth product in our catalogue"}
            };

            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var expectedUri = new Uri("http://example.com/api/products?type=Standard");
            var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
            var client = new HttpClient(mock.Object);
            var service = CreateProductsRepository(client);

            //Act
            var result = (await service.GetProductsAsync(null)).ToArray();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.Equal(expectedResult[i].ProductId, result[i].ProductId);
                Assert.Equal(expectedResult[i].ProductName, result[i].ProductName);
                Assert.Equal(expectedResult[i].Price, result[i].Price);
                Assert.Equal(expectedResult[i].Quantity, result[i].Quantity);
            }
            mock.Protected()
                .Verify("SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(
                            req => req.Method == HttpMethod.Get
                                   && req.RequestUri == expectedUri),
                        ItExpr.IsAny<CancellationToken>()
                        );
        }

        //[Fact]
        //public async Task GetProductsAsync_WithValid_ShouldReturnList()
        //{
        //    //Arrange
        //    var expectedResult = new Product[]
        //    {
        //        new Product { ProductId = 1, Type="Standard", ProductName = "Product A", Price = 12.50, Quantity = 4, Description = "The first product in our catalogue"},
        //        new Product { ProductId = 2, Type="Standard", ProductName = "Product B", Price = 5.20, Quantity = 25, Description = "The second product in our catalogue"},
        //        new Product { ProductId = 3, Type="Custom", ProductName = "Product C", Price = 20.99, Quantity = 50, Description = "The third product in our catalogue"},
        //        new Product { ProductId = 4, Type="Standard", ProductName = "Product D", Price = 5.80, Quantity = 100, Description = "The fourth product in our catalogue"},
        //        new Product { ProductId = 5, Type="Custom", ProductName = "Product E", Price = 60.00, Quantity = 5, Description = "The fifth product in our catalogue"},
        //        new Product { ProductId = 6, Type="Custom", ProductName = "Product F", Price = 45.75, Quantity = 10, Description = "The sixth product in our catalogue"}
        //    };

        //    var expectedJson = JsonConvert.SerializeObject(expectedResult);
        //    var expectedUri = new Uri("http://example.com/api/products?type=Standard");
        //    var mock = CreateHttpMock(HttpStatusCode.OK, expectedJson);
        //    var client = new HttpClient(mock.Object);
        //    var service = CreateProductsRepository(client);

        //    //Act
        //    var result = (await service.GetProductsAsync("Standard")).ToArray();

        //    //Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(expectedResult.Length, result.Length);
        //    for (int i = 0; i < result.Length; i++)
        //    {
        //        Assert.Equal(expectedResult[i].ProductId, result[i].ProductId);
        //        Assert.Equal(expectedResult[i].ProductName, result[i].ProductName);
        //        Assert.Equal(expectedResult[i].Price, result[i].Price);
        //        Assert.Equal(expectedResult[i].Quantity, result[i].Quantity);
        //    }
        //    mock.Protected()
        //        .Verify("SendAsync",
        //                Times.Once(),
        //                ItExpr.Is<HttpRequestMessage>(
        //                    req => req.Method == HttpMethod.Get
        //                           && req.RequestUri == expectedUri),
        //                ItExpr.IsAny<CancellationToken>()
        //                );
        //}
    }
}
