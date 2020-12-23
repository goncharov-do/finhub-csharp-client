using System;
using System.Collections.Generic;
using System.Text;

namespace FinnhubCSharpClient
{
    public class FinnhubResponse<T>
    {
        public int RemainingRequests { get; set; }
        public DateTimeOffset LimitResetTime { get; set; }
        public T Data { get; set; }
    }
}
