using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMSProject.Core.Domain.Entities.Content;

namespace CMSProject.Core.Domain.Entities
{
    public class User
    {
       public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public ICollection<Content> Contents { get; set; }

    }
}
