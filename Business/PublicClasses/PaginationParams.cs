using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.PublicClasses
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _PageSize = 10;

        public int pageSize
        {
            get => _PageSize;
            set => _PageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
