using FinnhubCSharpClient.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinnhubCSharpClient
{
    public class FinnhubClient
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string token;

        public FinnhubClient(FinnhubConfiguration configuration)
        {
            client.BaseAddress = configuration.BaseAddress;
            token = configuration.Token;
        }

        public async Task<IEnumerable<StockSymbol>> StockSymbolsAsync(string exchange)
        {
            var response = await client.GetAsync($"stock/symbol?exchange={exchange}&token={token}");
            return await response.Content.ReadAsAsync<IEnumerable<StockSymbol>>();
        }
    }
}
