using CMSProject.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Core.Domain.Interfaces
{
    public interface IContentRepository : IRepository<Content>
    {
        Task<IEnumerable<Content>> GetByLanguageAsync(string language);
        Task<IEnumerable<Content>> GetByCategoryAsync(int categoryId);
        Task<ContentVariant> GetVariantAsync(int contentId, int variantId);
    }
}
