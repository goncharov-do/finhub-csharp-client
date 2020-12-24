using System;

namespace FinnhubCSharpClient
{
    public class FinnhubResponse<T>
    {
        public int RemainingRequests { get; set; }
        public DateTimeOffset LimitResetTime { get; set; }
        public T Data { get; set; }

        internal static FinnhubResponse<T> InvalidApiKey => new FinnhubResponse<T> {Data = default, LimitResetTime = DateTimeOffset.UtcNow, RemainingRequests = -1};
        internal static FinnhubResponse<T> Default => new FinnhubResponse<T>();
    }
}
