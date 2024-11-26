using CMSProject.Application.Dtos;
using CMSProject.Application.Interfaces;
using CMSProject.Core.Domain.Entities;
using CMSProject.Core.Domain.Exceptions.CMSProject.Core.Exceptions;
using CMSProject.Core.Domain.Interfaces;
using CMSProject.Infrastructure.Cache;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CMSProject.Application.Services
{
    public class ContentService : IContentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ContentService> _logger;

        public ContentService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<ContentService> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ContentDto> GetContentAsync(int id)
        {
            string cacheKey = $"content_{id}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var content = await _unitOfWork.Contents.GetByIdAsync(id);
                if (content == null)
                    throw new NotFoundException($"Content with id {id} not found");

                return content.Adapt<ContentDto>();
            });
        }

        public async Task<IEnumerable<ContentDto>> GetAllContentsAsync()
        {
            string cacheKey = "all_contents";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var contents = await _unitOfWork.Contents.GetAllAsync();
                return contents.Adapt<IEnumerable<ContentDto>>();
            });
        }

        public async Task<int> CreateContentAsync(ContentDto contentDto)
        {
            var content = contentDto.Adapt<Content>();

            // En az 2 varyant kontrolü
            if (contentDto.Variants?.Count < 2)
                throw new ValidationException("Content must have at least 2 variants");

            await _unitOfWork.Contents.AddAsync(content);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync("all_contents");

            return content.Id;
        }

        public async Task<IEnumerable<ContentDto>> GetContentsByCategoryAsync(int categoryId)
        {
            string cacheKey = $"contents_category_{categoryId}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var contents = await _unitOfWork.Contents.GetByCategoryAsync(categoryId);
                return contents.Adapt<IEnumerable<ContentDto>>();
            });
        }

        public async Task<IEnumerable<ContentDto>> GetContentsByLanguageAsync(string language)
        {
            string cacheKey = $"contents_language_{language}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var contents = await _unitOfWork.Contents.GetByLanguageAsync(language);
                return contents.Adapt<IEnumerable<ContentDto>>();
            });
        }

        public async Task<ContentVariantDto> GetContentVariantAsync(int contentId, int variantId)
        {
            string cacheKey = $"content_{contentId}_variant_{variantId}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var variant = await _unitOfWork.Contents.GetVariantAsync(contentId, variantId);
                if (variant == null)
                    throw new NotFoundException($"Variant not found");

                return variant.Adapt<ContentVariantDto>();
            });
        }

        private async Task<ContentVariant> SelectVariantForUser(Content content, int userId)
        {
            // Kullanıcıya özel varyant
            // Örnek: A/B test
            var variantIndex = userId % content.Variants.Count;
            return content.Variants.ElementAt(variantIndex);
        }
    }
}
