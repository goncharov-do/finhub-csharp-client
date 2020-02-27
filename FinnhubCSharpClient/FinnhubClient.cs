using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinnhubCSharpClient
{
    public static class FinnhubClient
    {
        private static HttpClient client = new HttpClient();

        static FinnhubClient()
        {
            client.BaseAddress = new Uri("https://finnhub.io/api/v1/");
        }

        public static async Task<IEnumerable<StockSymbol>> StockSymbolsAsync(string exchange)
        {
            var response = await client.GetAsync($"stock/symbol?exchange={exchange}&token=bpbsv8vrh5r9k08nbdag");
            return await response.Content.ReadAsAsync<IEnumerable<StockSymbol>>();
        }
    }
}
