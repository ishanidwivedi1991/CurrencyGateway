using CurrencyGateway.Server.Controllers;
using CurrencyGateway.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace CurrencyExchange.Test
{
    public class CurrencyExchangeApiTest
    {
        public class CurrencyExchangeControllerTests
        {
            private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
            private readonly Mock<IHttpClientFactory> _mockFactory;
            private readonly Mock<ILogger<CurrencyExchangeController>> _mockLog;
            private readonly HttpClient _httpClient;
            private readonly CurrencyExchangeController _currencyController;

            public CurrencyExchangeControllerTests()
            {
                _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                _mockFactory = new Mock<IHttpClientFactory>();
                _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
                { BaseAddress = new Uri("https://api.fxratesapi.com/latest") };
                _mockFactory.Setup(_ => _.CreateClient("currencyexchange")).Returns(_httpClient);
                _mockLog = new Mock<ILogger<CurrencyExchangeController>>();

                _currencyController = new CurrencyExchangeController(_mockFactory.Object, _mockLog.Object);
            }

            [Fact]
            public async Task GetExchangeRates_ReturnsRates_WhenApiCallIsSuccessful()
            {
                // Update the value with current rate to test this
                var apiResponse = new
                {
                    rates = new { INR = 1, USD = 0.011852641 },
                    @base = "INR",
                    date = "2024-11-24"
                };

                var jsonResponse = JsonConvert.SerializeObject(apiResponse);

                _mockHttpMessageHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(jsonResponse)
                    });

                // Get data from API
                var result = await _currencyController.GetExchangeRates();

                
                var okResult = Assert.IsType<OkObjectResult>(result);
                var responseData = JsonConvert.DeserializeObject<CurrencyExchangeRateResponse>(okResult.Value.ToString());

                // Assertions
                Assert.NotNull(responseData);
                Assert.Equal(1, responseData.Rates["INR"]);
                Assert.Equal(0.011852641, responseData.Rates["USD"]);
                Assert.Equal("INR", responseData.Base);
            }

            [Fact]
            public async Task GetExchangeRates_ReturnsInternalServerError_WhenApiCallFails()
            {
                
                _mockHttpMessageHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Content = new StringContent("Internal Server Error")
                    });

                // Get data from API
                var result = await _currencyController.GetExchangeRates();

                // Assertions
                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            }
        }
    }
}