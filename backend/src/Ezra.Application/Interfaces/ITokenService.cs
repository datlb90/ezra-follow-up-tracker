using Ezra.Domain.Entities;

namespace Ezra.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
