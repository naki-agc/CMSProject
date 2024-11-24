using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Core.Domain.Exceptions
{
    namespace CMSProject.Core.Exceptions
    {
        public class NotFoundException : BaseException
        {
            public NotFoundException(string message) : base(message)
            {
            }

            public NotFoundException(string name, object key)
                : base($"Entity \"{name}\" ({key}) was not found.")
            {
            }
        }
    }
}
