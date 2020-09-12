using System.IO;
using System.Threading.Tasks;

namespace KnifeAndSpoon
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}