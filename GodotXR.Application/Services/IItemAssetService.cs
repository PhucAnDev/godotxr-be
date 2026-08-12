using System.IO;
using System.Threading.Tasks;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ItemAsset;

namespace GodotXR.Application.Services
{
    public interface IItemAssetService
    {
        Task<PagedResponse<ItemAssetResponse>> GetListAsync(int pageNumber, int pageSize);
        
        Task<ItemAssetResponse?> GetByIdAsync(int id);
        
        Task<ItemAssetResponse> CreateAsync(
            string name, 
            string answerSentence, 
            Stream modelStream, 
            string modelFileName, 
            string modelContentType, 
            Stream? imageStream, 
            string? imageFileName, 
            string? imageContentType, 
            Stream? audioStream, 
            string? audioFileName, 
            string? audioContentType);

        Task<ItemAssetResponse?> UpdateAsync(
            int id,
            string name, 
            string answerSentence, 
            Stream? modelStream, 
            string? modelFileName, 
            string? modelContentType, 
            Stream? imageStream, 
            string? imageFileName, 
            string? imageContentType, 
            Stream? audioStream, 
            string? audioFileName, 
            string? audioContentType);

        Task<bool> DeleteAsync(int id);
    }
}
