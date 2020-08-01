using SoundByte.Core.Models.Media;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    public interface IHistoryService
    {
        Task AddToHistoryAsync(Media media);
    }
}
