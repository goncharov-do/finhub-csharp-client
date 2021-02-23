using System;

namespace FinnhubCSharpClient
{
    public interface ITokenPool
    {
        string GetToken();
        void AddToken(string token);
        void ReleaseToken(string token, DateTimeOffset resetTime, int remainingRequests);
        void ReleaseInvalidToken(string token);
    }
}