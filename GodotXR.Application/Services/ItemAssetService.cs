using AutoMapper;
using GodotXR.Application.DTOs.Response;
using GodotXR.Application.DTOs.Response.ItemAsset;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GodotXR.Application.Services
{
    public class ItemAssetService : IItemAssetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public ItemAssetService(
            IUnitOfWork unitOfWork,
            IStorageService storageService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ItemAssetResponse>> GetListAsync(int pageNumber, int pageSize)
        {
            var repository = _unitOfWork.Repository<ItemAsset>();
            
            var pagedResult = await repository.GetPagedAsync(
                pageNumber,
                pageSize,
                predicate: x => !x.IsDeleted,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                asNoTracking: true);

            var mappedItems = pagedResult.Items.Select(x => MapToResponse(x)).ToList();

            return new PagedResponse<ItemAssetResponse>
            {
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                Items = mappedItems
            };
        }

        public async Task<ItemAssetResponse?> GetByIdAsync(int id)
        {
            var repository = _unitOfWork.Repository<ItemAsset>();
            var item = await repository.GetFirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, tracked: false);
            if (item == null) return null;

            return MapToResponse(item);
        }

        public async Task<ItemAssetResponse> CreateAsync(
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
            string? audioContentType)
        {
            var guid = Guid.NewGuid().ToString();
            
            // Upload model file
            var modelKey = $"assets/{guid}/{modelFileName}";
            await _storageService.UploadAsync(modelStream, modelKey, modelContentType, default);

            string? imageKey = null;
            if (imageStream != null && !string.IsNullOrEmpty(imageFileName) && !string.IsNullOrEmpty(imageContentType))
            {
                imageKey = $"assets/{guid}/{imageFileName}";
                await _storageService.UploadAsync(imageStream, imageKey, imageContentType, default);
            }

            string? audioKey = null;
            if (audioStream != null && !string.IsNullOrEmpty(audioFileName) && !string.IsNullOrEmpty(audioContentType))
            {
                audioKey = $"assets/{guid}/{audioFileName}";
                await _storageService.UploadAsync(audioStream, audioKey, audioContentType, default);
            }

            var itemAsset = new ItemAsset
            {
                Name = name,
                AnswerSentence = answerSentence,
                ModelUrl = modelKey,
                ImageUrl = imageKey,
                AudioUrl = audioKey
            };

            await _unitOfWork.Repository<ItemAsset>().AddAsync(itemAsset);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(itemAsset);
        }

        public async Task<ItemAssetResponse?> UpdateAsync(
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
            string? audioContentType)
        {
            var repository = _unitOfWork.Repository<ItemAsset>();
            var itemAsset = await repository.GetFirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (itemAsset == null) return null;

            itemAsset.Name = name;
            itemAsset.AnswerSentence = answerSentence;
            itemAsset.UpdatedAt = DateTime.UtcNow.AddHours(7);

            var guid = Guid.NewGuid().ToString();

            if (modelStream != null && !string.IsNullOrEmpty(modelFileName) && !string.IsNullOrEmpty(modelContentType))
            {
                // Delete old model first
                if (!string.IsNullOrEmpty(itemAsset.ModelUrl))
                {
                    try { await _storageService.DeleteAsync(itemAsset.ModelUrl, default); } catch { }
                }

                var modelKey = $"assets/{guid}/{modelFileName}";
                await _storageService.UploadAsync(modelStream, modelKey, modelContentType, default);
                itemAsset.ModelUrl = modelKey;
            }

            if (imageStream != null && !string.IsNullOrEmpty(imageFileName) && !string.IsNullOrEmpty(imageContentType))
            {
                // Delete old image first
                if (!string.IsNullOrEmpty(itemAsset.ImageUrl))
                {
                    try { await _storageService.DeleteAsync(itemAsset.ImageUrl, default); } catch { }
                }

                var imageKey = $"assets/{guid}/{imageFileName}";
                await _storageService.UploadAsync(imageStream, imageKey, imageContentType, default);
                itemAsset.ImageUrl = imageKey;
            }

            if (audioStream != null && !string.IsNullOrEmpty(audioFileName) && !string.IsNullOrEmpty(audioContentType))
            {
                // Delete old audio first
                if (!string.IsNullOrEmpty(itemAsset.AudioUrl))
                {
                    try { await _storageService.DeleteAsync(itemAsset.AudioUrl, default); } catch { }
                }

                var audioKey = $"assets/{guid}/{audioFileName}";
                await _storageService.UploadAsync(audioStream, audioKey, audioContentType, default);
                itemAsset.AudioUrl = audioKey;
            }

            repository.Update(itemAsset);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(itemAsset);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repository = _unitOfWork.Repository<ItemAsset>();
            var itemAsset = await repository.GetFirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (itemAsset == null) return false;

            // Check if item is assigned to any active LessonSlot
            var slotRepository = _unitOfWork.Repository<LessonSlot>();
            var isAssigned = await slotRepository.ExistsAsync(s => s.ItemAssetId == id && !s.IsDeleted);
            if (isAssigned)
            {
                throw new InvalidOperationException("Không thể xóa vật phẩm đang được gán vào vị trí (Slot) của bài học.");
            }

            itemAsset.IsDeleted = true;
            itemAsset.DeletedAt = DateTime.UtcNow.AddHours(7);
            
            repository.Update(itemAsset);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private ItemAssetResponse MapToResponse(ItemAsset item)
        {
            return new ItemAssetResponse
            {
                Id = item.Id,
                Name = item.Name,
                AnswerSentence = item.AnswerSentence,
                ModelUrl = $"/api/files/assets/{item.Id}/model",
                ImageUrl = !string.IsNullOrEmpty(item.ImageUrl) ? $"/api/files/assets/{item.Id}/image" : null,
                AudioUrl = !string.IsNullOrEmpty(item.AudioUrl) ? $"/api/files/assets/{item.Id}/audio" : null,
                CreatedAt = item.CreatedAt
            };
        }
    }
}
