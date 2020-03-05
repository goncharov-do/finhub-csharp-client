using FinnhubCSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinnhubCSharpClient
{
    public class FinnhubClient
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string token;

        public FinnhubClient(Uri baseAddress, string token)
        {
            client.BaseAddress = baseAddress;
            this.token = token;
        }

        public async Task<IEnumerable<StockSymbol>> StockSymbolsAsync(string exchange)
        {
            var response = await client.GetAsync($"stock/symbol?exchange={exchange}&token={token}");
            return await response.Content.ReadAsAsync<IEnumerable<StockSymbol>>();
        }
    }
}
