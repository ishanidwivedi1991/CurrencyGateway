using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyGateway.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClient;

        public CurrencyExchangeController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("rates")]
        [ResponseCache(Duration =30)]
        public async Task<IActionResult> GetExchangeRates()
        {
            try
            {
                var client = _httpClient.CreateClient("currencyexchange");
                var response = await client.GetAsync("latest");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error fetching exchange rates.");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("ratesbycurrency")]
        [ResponseCache(Duration = 30)]
        public async Task<IActionResult> GetExchangeRates(string basecurrency)
        {
            try
            {
                var client = _httpClient.CreateClient("currencyexchange");                
                var response = await client.GetAsync(String.Concat("latest?base=",basecurrency));

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error fetching exchange rates.");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
