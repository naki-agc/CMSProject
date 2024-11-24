using CMSProject.Core.Domain.Entities;
using CMSProject.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Infrastructure.Persistence.Repositories
{
    public class ContentRepository : Repository<Content>, IContentRepository
    {
        private readonly AppDbContext _context;

        public ContentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Content>> GetByLanguageAsync(string language)
        {
            return await _context.Contents
                .Include(c => c.Category)
                .Include(c => c.Variants)
                .Where(c => c.Language == language)
                .ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Contents
                .Include(c => c.Category)
                .Include(c => c.Variants)
                .Where(c => c.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<ContentVariant> GetVariantAsync(int contentId, int variantId)
        {
            return await _context.ContentVariants
                .FirstOrDefaultAsync(v => v.ContentId == contentId && v.Id == variantId);
        }

        public override async Task<Content> GetByIdAsync(int id)
        {
            return await _context.Contents
                .Include(c => c.Category)
                .Include(c => c.Variants)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
