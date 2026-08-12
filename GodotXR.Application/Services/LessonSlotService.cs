using AutoMapper;
using GodotXR.Application.DTOs.Response.ItemAsset;
using GodotXR.Application.DTOs.Response.LessonImage;
using GodotXR.Application.DTOs.Response.LessonSlot;
using GodotXR.Domain.Entities;
using GodotXR.Domain.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GodotXR.Application.Services
{
    public class LessonSlotService : ILessonSlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public LessonSlotService(
            IUnitOfWork unitOfWork,
            IStorageService storageService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LessonImageResponse>> GetImagesByLessonIdAsync(int lessonId)
        {
            var repo = _unitOfWork.Repository<LessonImage>();
            var images = await repo.FindAsync(x => x.LessonId == lessonId && !x.IsDeleted);

            return images.Select(x => MapImageResponse(x)).ToList();
        }

        public async Task<LessonImageResponse> AddImageAsync(
            int lessonId,
            string angleName,
            Stream imageStream,
            string fileName,
            string contentType)
        {
            // Verify Lesson exists
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
            if (lesson == null || lesson.IsDeleted)
            {
                throw new KeyNotFoundException("Không tìm thấy bài học.");
            }

            var guid = Guid.NewGuid().ToString();
            var objectKey = $"lessons/{lessonId}/images/{guid}_{fileName}";
            
            await _storageService.UploadAsync(imageStream, objectKey, contentType, default);

            var lessonImage = new LessonImage
            {
                LessonId = lessonId,
                AngleName = angleName,
                ImageUrl = objectKey
            };

            await _unitOfWork.Repository<LessonImage>().AddAsync(lessonImage);
            await _unitOfWork.SaveChangesAsync();

            return MapImageResponse(lessonImage);
        }

        public async Task<bool> DeleteImageAsync(int lessonId, int imageId)
        {
            var repo = _unitOfWork.Repository<LessonImage>();
            var lessonImage = await repo.GetFirstOrDefaultAsync(x => x.Id == imageId && x.LessonId == lessonId && !x.IsDeleted);

            if (lessonImage == null) return false;

            // Delete from storage
            if (!string.IsNullOrEmpty(lessonImage.ImageUrl))
            {
                try { await _storageService.DeleteAsync(lessonImage.ImageUrl, default); } catch { }
            }

            // Note: EF configurations will set LessonImageId to Null in LessonSlots due to OnDelete(DeleteBehavior.SetNull)
            lessonImage.IsDeleted = true;
            lessonImage.DeletedAt = DateTime.UtcNow.AddHours(7);

            repo.Update(lessonImage);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LessonSlotResponse>> GetSlotsByLessonIdAsync(int lessonId)
        {
            var repo = _unitOfWork.Repository<LessonSlot>();
            var slots = await repo.FindAsync(
                filter: s => s.LessonId == lessonId && !s.IsDeleted,
                includeProperties: "ItemAsset");

            return slots.Select(x => MapSlotResponse(x)).ToList();
        }

        public async Task<LessonSlotResponse> ConfigureSlotAsync(
            int lessonId,
            string slotIdentifier,
            string slotName,
            int? lessonImageId)
        {
            // Verify Lesson
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
            if (lesson == null || lesson.IsDeleted)
            {
                throw new KeyNotFoundException("Không tìm thấy bài học.");
            }

            // Verify Lesson Image if provided
            if (lessonImageId.HasValue)
            {
                var imageRepo = _unitOfWork.Repository<LessonImage>();
                var imageExists = await imageRepo.ExistsAsync(x => x.Id == lessonImageId.Value && x.LessonId == lessonId && !x.IsDeleted);
                if (!imageExists)
                {
                    throw new KeyNotFoundException("Không tìm thấy ảnh góc chụp của bài học này.");
                }
            }

            var slotRepo = _unitOfWork.Repository<LessonSlot>();
            var existingSlot = await slotRepo.GetFirstOrDefaultAsync(
                filter: s => s.LessonId == lessonId && s.SlotIdentifier == slotIdentifier && !s.IsDeleted,
                includeProperties: "ItemAsset");

            if (existingSlot != null)
            {
                existingSlot.SlotName = slotName;
                existingSlot.LessonImageId = lessonImageId;
                existingSlot.UpdatedAt = DateTime.UtcNow.AddHours(7);
                slotRepo.Update(existingSlot);
                await _unitOfWork.SaveChangesAsync();
                return MapSlotResponse(existingSlot);
            }

            var newSlot = new LessonSlot
            {
                LessonId = lessonId,
                SlotIdentifier = slotIdentifier,
                SlotName = slotName,
                LessonImageId = lessonImageId
            };

            await slotRepo.AddAsync(newSlot);
            await _unitOfWork.SaveChangesAsync();
            return MapSlotResponse(newSlot);
        }

        public async Task<LessonSlotResponse?> AssignItemToSlotAsync(int lessonId, int slotId, int? itemAssetId)
        {
            var slotRepo = _unitOfWork.Repository<LessonSlot>();
            var slot = await slotRepo.GetFirstOrDefaultAsync(
                filter: s => s.Id == slotId && s.LessonId == lessonId && !s.IsDeleted,
                includeProperties: "ItemAsset");

            if (slot == null) return null;

            if (itemAssetId.HasValue)
            {
                var itemRepo = _unitOfWork.Repository<ItemAsset>();
                var itemExists = await itemRepo.ExistsAsync(x => x.Id == itemAssetId.Value && !x.IsDeleted);
                if (!itemExists)
                {
                    throw new KeyNotFoundException("Không tìm thấy vật phẩm 3D.");
                }
            }

            slot.ItemAssetId = itemAssetId;
            slot.UpdatedAt = DateTime.UtcNow.AddHours(7);
            slotRepo.Update(slot);
            await _unitOfWork.SaveChangesAsync();

            // Re-fetch slot with details to be safe
            var updatedSlot = await slotRepo.GetFirstOrDefaultAsync(
                filter: s => s.Id == slotId,
                includeProperties: "ItemAsset");

            if (updatedSlot == null) return null;

            return MapSlotResponse(updatedSlot);
        }

        public async Task<IEnumerable<LessonSlotResponse>> GetClientConfigAsync(int lessonId)
        {
            var repo = _unitOfWork.Repository<LessonSlot>();
            
            // Only fetch slots that have an ItemAsset assigned (and not soft-deleted)
            var slots = await repo.FindAsync(
                filter: s => s.LessonId == lessonId && s.ItemAssetId != null && !s.IsDeleted,
                includeProperties: "ItemAsset");

            // Filter out slots where the asset itself is soft-deleted
            var activeSlots = slots.Where(s => s.ItemAsset != null && !s.ItemAsset.IsDeleted);

            return activeSlots.Select(x => MapSlotResponse(x)).ToList();
        }

        private LessonImageResponse MapImageResponse(LessonImage img)
        {
            return new LessonImageResponse
            {
                Id = img.Id,
                LessonId = img.LessonId,
                AngleName = img.AngleName,
                ImageUrl = $"/api/files/lessons/{img.LessonId}/images/{img.Id}"
            };
        }

        private LessonSlotResponse MapSlotResponse(LessonSlot slot)
        {
            return new LessonSlotResponse
            {
                Id = slot.Id,
                LessonId = slot.LessonId,
                LessonImageId = slot.LessonImageId,
                SlotIdentifier = slot.SlotIdentifier,
                SlotName = slot.SlotName,
                ItemAssetId = slot.ItemAssetId,
                ItemAsset = slot.ItemAsset != null ? new ItemAssetResponse
                {
                    Id = slot.ItemAsset.Id,
                    Name = slot.ItemAsset.Name,
                    AnswerSentence = slot.ItemAsset.AnswerSentence,
                    ModelUrl = $"/api/files/assets/{slot.ItemAsset.Id}/model",
                    ImageUrl = !string.IsNullOrEmpty(slot.ItemAsset.ImageUrl) ? $"/api/files/assets/{slot.ItemAsset.Id}/image" : null,
                    AudioUrl = !string.IsNullOrEmpty(slot.ItemAsset.AudioUrl) ? $"/api/files/assets/{slot.ItemAsset.Id}/audio" : null,
                    CreatedAt = slot.ItemAsset.CreatedAt
                } : null
            };
        }
    }
}
