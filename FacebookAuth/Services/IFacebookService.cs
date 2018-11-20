using System.Threading.Tasks;
using FacebookAuth.Models;

namespace FacebookAuth.Services
{
    public interface IFacebookService
    {
        Task<UserAccountViewModel> GetAccountAsync(string accessToken);
        Task PostOnWallAsync(string accessToken, string message);
    }
}
