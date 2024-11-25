using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyGateway.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<CurrencyExchangeController> _logger;

        public CurrencyExchangeController(IHttpClientFactory httpClient,
            ILogger<CurrencyExchangeController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet("rates")]
        [ResponseCache(Duration =20)]
        public async Task<IActionResult> GetExchangeRates()
        {
            try
            {
                _logger.LogInformation("Creating HttpClient.");
                var client = _httpClient.CreateClient("currencyexchange");

                _logger.LogInformation("Sending request to API");
                var response = await client.GetAsync("latest");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Response error code: {0}", (int)response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Error fetching exchange rates.");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError( "There is an error: {0}", ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("ratesbycurrency")]
        [ResponseCache(Duration = 20)]
        public async Task<IActionResult> GetExchangeRates(string basecurrency)
        {
            try
            {
                _logger.LogInformation("Creating HttpClient.");
                var client = _httpClient.CreateClient("currencyexchange");

                _logger.LogInformation("Sending request to API");
                var response = await client.GetAsync(String.Concat("latest?base=",basecurrency));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Response error code: {0}", (int)response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Error fetching exchange rates.");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError("There is an error: {0}", ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
