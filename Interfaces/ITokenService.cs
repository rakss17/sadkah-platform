using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.Backend.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        Task<RefreshToken> CreateRefreshTokenAsync(User user);
        Task<(string AccessToken, RefreshToken RefreshToken)> RefreshAsync(string token);
    }
}