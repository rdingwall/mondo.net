using System.Threading.Tasks;

namespace Mondo.Client
{
    public interface IMondoApiClientFactory
    {
        /// <summary>
        /// Creates an authenticated API client.
        /// </summary>
        Task<IMondoApiClient> Authenticate(string username, string password);
    }
}