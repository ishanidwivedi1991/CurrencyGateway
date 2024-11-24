namespace CurrencyGateway.Server.Models
{
    public class CurrencyExchangeRateResponse
    {
        public Dictionary<string, double> Rates { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
    }
}
