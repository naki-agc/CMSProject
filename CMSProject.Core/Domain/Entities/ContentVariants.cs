using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Core.Domain.Entities
{

    public class ContentVariant
    {
        public int Id { get; set; }
        public string VariantContentName { get; set; }
        public int ContentId { get; set; }
        public Content Content { get; set; }
    }

}
