using WeightTrackerUI.HelperInterfaces;

namespace WeightTrackerUI.Services
{
    public interface ITokenService
    {
        string GenerateToken(int farmerId);
        int? GetId(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly Dictionary<string, int> _farmerTokens = new();

        public string GenerateToken(int farmerId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _farmerTokens[token] = farmerId;
            return token;
        }

        public int? GetId(string token)
        {
            return _farmerTokens.TryGetValue(token, out var id) ? id : null;
        }
    }

}
