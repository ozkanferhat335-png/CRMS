using System;
using System.Collections.Generic;
using System.Linq;

namespace CRMS.Common.Helpers
{
    public static class PaginationHelper
    {
        public static PaginatedResult<T> Paginate<T>(IEnumerable<T> items, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var totalCount = items.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedItems = items
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<T>
            {
                Items = pagedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}