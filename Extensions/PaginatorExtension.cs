using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Promotion.Extensions
{
    public class PaginatorExtension<T> : List<T>
    {
        public int pageIndex { get; private set; }
        public int totalPages { get; private set; }

        public PaginatorExtension(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.pageIndex = pageIndex;
            this.totalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (pageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (pageIndex < totalPages);
            }
        }

        public static PaginatorExtension<T> CreateAsync(List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatorExtension<T>(items, count, pageIndex, pageSize);
        }
    }
}