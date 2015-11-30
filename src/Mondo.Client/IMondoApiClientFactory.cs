using System.Threading.Tasks;

namespace Mondo.Client
{
    public interface IMondoApiClientFactory
    {
        Task<IMondoApiClient> Authenticate(string username, string password);
    }
}