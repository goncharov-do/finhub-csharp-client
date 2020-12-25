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

        public FinnhubClient(Uri baseAddress)
        {
            _client.BaseAddress = baseAddress;
        }

        private async Task<HttpResponseMessage> Get(string request, string token)
        {
            return await _client.GetAsync($"{request}&token={token}");
        }

        private static (int, DateTimeOffset) ParseHeaders(HttpResponseHeaders headers) {
            var remaining = int.Parse(headers.GetValues("X-Ratelimit-Remaining").FirstOrDefault()!);
            var secondsLimit = int.Parse(headers.GetValues("X-Ratelimit-Reset").FirstOrDefault()!);
            return (
                remaining,
                DateTimeOffset.FromUnixTimeSeconds(secondsLimit)
            );
        }

        private static async Task<T> ReadIfOk<T>(HttpResponseMessage response) {
            return response.StatusCode == HttpStatusCode.OK
                ? await response.Content.ReadAsAsync<T>()
                : default;
        }

        private static bool IsSuccess<T>(HttpResponseMessage response, out FinnhubResponse<T> result) {
            switch (response.StatusCode) {
                case HttpStatusCode.OK:
                    result = null;
                    return true;
                case HttpStatusCode.Unauthorized:
                    result = FinnhubResponse<T>.InvalidApiKey;
                    return false;
                default:
                    result = FinnhubResponse<T>.Default;
                    return false;
            }
        }

        public async Task<FinnhubResponse<IEnumerable<StockSymbol>>> StockSymbolsAsync(string exchange, string token)
        {
            var response = await Get($"stock/symbol?exchange={exchange}", token);
            if (!IsSuccess<IEnumerable<StockSymbol>>(response, out var result)) {
                return result;
            }
            var symbols = await ReadIfOk<IEnumerable<StockSymbol>>(response);
            var (remaining, limitReset) = ParseHeaders(response.Headers);
            return new FinnhubResponse<IEnumerable<StockSymbol>> {
                Data = symbols,
                RemainingRequests = remaining,
                LimitResetTime = limitReset
            };
        }

        public async Task<FinnhubResponse<IEnumerable<News>>> NewsAsync(string stock, DateTime from, DateTime to, string token)
        {
            var response = await Get($"company-news?symbol={stock}&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", token);
            if (!IsSuccess<IEnumerable<News>>(response, out var result)) {
                return result;
            }
            var news = await ReadIfOk<IEnumerable<News>>(response);
            var (remaining, limitReset) = ParseHeaders(response.Headers);
            return new FinnhubResponse<IEnumerable<News>> {
                Data = news,
                RemainingRequests = remaining,
                LimitResetTime = limitReset
            };
        }

        public async Task<FinnhubResponse<PatternResponse>> PatternsAsync(string stock, string resolution, string token)
        {
            var response = await Get($"scan/pattern?symbol={stock}&resolution={resolution}", token);
            if (!IsSuccess<PatternResponse>(response, out var result)) {
                return result;
            }
            var patterns = await ReadIfOk<PatternResponse>(response);
            var (remaining, limitReset) = ParseHeaders(response.Headers);
            return new FinnhubResponse<PatternResponse> {
                Data = patterns,
                RemainingRequests = remaining,
                LimitResetTime = limitReset
            };
        }

        public async Task<FinnhubResponse<CompanyInfo>> CompanyInfoAsync(string stock, string token) {
            var response = await Get($"stock/profile2?symbol={stock}", token);
            if (!IsSuccess<CompanyInfo>(response, out var result)) {
                return result;
            }

            var info = await ReadIfOk<CompanyInfo>(response);
            var (remaining, limitReset) = ParseHeaders(response.Headers);
            return new FinnhubResponse<CompanyInfo> {
                Data = info,
                RemainingRequests = remaining,
                LimitResetTime = limitReset
            };
        }
    }
}
