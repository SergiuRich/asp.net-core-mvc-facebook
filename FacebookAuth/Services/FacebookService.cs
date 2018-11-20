using System.Threading.Tasks;
using FacebookAuth.Models;

namespace FacebookAuth.Services
{
    public class FacebookService : IFacebookService
    {
        private readonly IFacebookClient _facebookClient;

        public FacebookService(IFacebookClient facebookClient)
        {
            _facebookClient = facebookClient;
        }

        public async Task<UserAccountViewModel> GetAccountAsync(string accessToken)
        {
            var result = await _facebookClient.GetAsync<dynamic>("me",
                accessToken, "fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale,picture");

            if (result == null)
            {
                return new UserAccountViewModel();
            }

            var account = new UserAccountViewModel
            {
                Id = result.id,
                Email = result.email,
                Name = result.name,
                UserName = result.username,
                FirstName = result.first_name,
                LastName = result.last_name,
                Locale = result.locale,
                PictureUrl = result.picture.data.url

            };

            return account;
        }

        public async Task PostOnWallAsync(string accessToken, string message)
            => await _facebookClient.PostAsync("me/feed", accessToken,  new { message });
    }
}
