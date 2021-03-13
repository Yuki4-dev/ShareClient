using System.Threading.Tasks;

namespace ShareClient.Component
{
    public interface IShareClientReciver : IShareClient
    {
        public Task ReciveAsync();
    }
}
