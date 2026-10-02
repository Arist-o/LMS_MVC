using System;
using System.Collections.Generic;
using System.Text;

namespace Courses.Domain.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
