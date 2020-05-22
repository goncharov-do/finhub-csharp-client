using FinnhubCSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinnhubCSharpClient
{
    public class FinnhubClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _token;

        public FinnhubClient(Uri baseAddress, string token)
        {
            _client.BaseAddress = baseAddress;
            _token = token;
        }

        private async Task<HttpResponseMessage> Get(string request)
        {
            return await _client.GetAsync($"{request}&token={_token}");
        }

        private bool CanRead(HttpResponseMessage response)
        {
            return response.Content.Headers.ContentType.MediaType == "application/json";
        }

        public async Task<IEnumerable<StockSymbol>> StockSymbolsAsync(string exchange)
        {
            var response = await Get($"stock/symbol?exchange={exchange}");
            return CanRead(response) ? await response.Content.ReadAsAsync<IEnumerable<StockSymbol>>() : null;
        }

        public async Task<IEnumerable<News>> NewsAsync(string stock, DateTime from, DateTime to)
        {
            var response = await Get($"company-news?symbol={stock}&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
            return CanRead(response) ? await response.Content.ReadAsAsync<IEnumerable<News>>() : null;
        }

        public async Task<PatternResponse> PatternsAsync(string stock, string resolution)
        {
            var response = await Get($"scan/pattern?symbol={stock}&resolution={resolution}");
            return CanRead(response) ? await response.Content.ReadAsAsync<PatternResponse>() : null;
        }
    }
}
