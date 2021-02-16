using FinnhubCSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FinnhubCSharpClient
{
    public class FinnhubClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly ITokenPool _tokenPool;

        public FinnhubClient(Uri baseAddress, ITokenPool tokenPool) {
            _tokenPool = tokenPool;
            _client.BaseAddress = baseAddress;
        }

        private async Task<T> Get<T>(string request) {
            var token = _tokenPool.GetToken();
            var response = await _client.GetAsync($"{request}&token={token}");
            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                _tokenPool.ReleaseInvalidToken(token);
                return default;
            }
            var (remaining, secondsLimit) = ParseHeaders(response.Headers);
            _tokenPool.ReleaseToken(token, secondsLimit, remaining);
            if (response.StatusCode == HttpStatusCode.OK) {
                return await response.Content.ReadAsAsync<T>();
            }

            return default;
        }

        private static (int, DateTimeOffset) ParseHeaders(HttpHeaders headers) {
            var remaining = int.Parse(headers.GetValues("X-Ratelimit-Remaining").FirstOrDefault()!);
            var secondsLimit = int.Parse(headers.GetValues("X-Ratelimit-Reset").FirstOrDefault()!);
            return (
                remaining,
                DateTimeOffset.FromUnixTimeSeconds(secondsLimit)
            );
        }

        public async Task<bool> TestToken(string token) {
            var response = await _client.GetAsync($"quote?Symbol=AAPL&token={token}");
            return response.StatusCode != HttpStatusCode.Unauthorized;
        }

        public async Task<IEnumerable<StockSymbol>> StockSymbolsAsync(string exchange) {
            return await Get<IEnumerable<StockSymbol>>($"stock/symbol?exchange={exchange}");
        }

        public async Task<IEnumerable<News>> NewsAsync(string stock, DateTime from, DateTime to) {
            return await Get<IEnumerable<News>>($"company-news?symbol={stock}&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
        }

        public async Task<PatternResponse> PatternsAsync(string stock, string resolution) {
            return await Get<PatternResponse>($"scan/pattern?symbol={stock}&resolution={resolution}");
        }

        public async Task<CompanyInfo> CompanyInfoAsync(string stock) {
            return await Get<CompanyInfo>($"stock/profile2?symbol={stock}");
        }
    }
}
