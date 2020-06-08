using FinnhubCSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinnhubCSharpClient
{
    public class FinnhubClient
    {
        private readonly HttpClient _client = new HttpClient();

        public FinnhubClient(Uri baseAddress)
        {
            _client.BaseAddress = baseAddress;
        }

        private async Task<HttpResponseMessage> Get(string request, string token)
        {
            return await _client.GetAsync($"{request}&token={token}");
        }

        public async Task<IEnumerable<StockSymbol>> StockSymbolsAsync(string exchange, string token)
        {
            var response = await Get($"stock/symbol?exchange={exchange}", token);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception();
            return await response.Content.ReadAsAsync<IEnumerable<StockSymbol>>();
        }

        public async Task<IEnumerable<News>> NewsAsync(string stock, DateTime from, DateTime to, string token)
        {
            var response = await Get($"company-news?symbol={stock}&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", token);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception();
            return await response.Content.ReadAsAsync<IEnumerable<News>>();
        }

        public async Task<PatternResponse> PatternsAsync(string stock, string resolution, string token)
        {
            var response = await Get($"scan/pattern?symbol={stock}&resolution={resolution}", token);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception();
            return await response.Content.ReadAsAsync<PatternResponse>();
        }
    }
}
