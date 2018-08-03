using System.Threading.Tasks;

namespace StreamRipper.Interfaces
{
    public interface IStreamRipper
    {        
        void StartAsync();

        void Start();
    }
}