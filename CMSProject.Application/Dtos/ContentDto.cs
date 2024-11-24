using CMSProject.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Application.Dtos
{
    public class ContentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Language { get; set; }
        public string CategoryName { get; set; }
        public User User { get; set; }
        public List<ContentVariantDto> Variants { get; set; }
    }

}
