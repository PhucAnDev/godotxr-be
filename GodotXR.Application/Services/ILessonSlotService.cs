using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GodotXR.Application.DTOs.Response.LessonImage;
using GodotXR.Application.DTOs.Response.LessonSlot;

namespace GodotXR.Application.Services
{
    public interface ILessonSlotService
    {
        Task<IEnumerable<LessonImageResponse>> GetImagesByLessonIdAsync(int lessonId);
        
        Task<LessonImageResponse> AddImageAsync(
            int lessonId, 
            string angleName, 
            Stream imageStream, 
            string fileName, 
            string contentType);
            
        Task<bool> DeleteImageAsync(int lessonId, int imageId);
        
        Task<IEnumerable<LessonSlotResponse>> GetSlotsByLessonIdAsync(int lessonId);
        
        Task<LessonSlotResponse> ConfigureSlotAsync(
            int lessonId, 
            string slotIdentifier, 
            string slotName, 
            int? lessonImageId);
            
        Task<LessonSlotResponse?> AssignItemToSlotAsync(int lessonId, int slotId, int? itemAssetId);
        
        Task<IEnumerable<LessonSlotResponse>> GetClientConfigAsync(int lessonId);
    }
}
