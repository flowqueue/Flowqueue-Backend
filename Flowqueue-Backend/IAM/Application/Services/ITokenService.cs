using Flowqueue_Backend.IAM.Domain.Model.Aggregates;

namespace Flowqueue_Backend.IAM.Application.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
