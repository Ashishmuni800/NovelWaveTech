using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }

    public class PaginatedResult<T>
    {
        public IEnumerable<T> items { get; set; } = Enumerable.Empty<T>();
        public bool hasMore { get; set; }
    }

}
