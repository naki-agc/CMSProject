using CMSProject.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Application.Interfaces
{
    public interface IContentService
    {
        // Temel içerik operasyonları
        Task<ContentDto> GetContentAsync(int id);
        Task<IEnumerable<ContentDto>> GetAllContentsAsync();
        Task<int> CreateContentAsync(ContentDto contentDto);
        Task<IEnumerable<ContentDto>> GetContentsByCategoryAsync(int categoryId);
        Task<IEnumerable<ContentDto>> GetContentsByLanguageAsync(string language);
        
    }
}
