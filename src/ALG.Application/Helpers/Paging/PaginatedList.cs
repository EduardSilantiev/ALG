using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALG.Application.Helpers.Paging
{
    /// <summary>
    /// A generic class that provides in memory and in database paging functionality for a collection of any type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; private set; }

        /// <summary>
        /// Performs in memory  paging for a collection of any type
        /// Use the class if a collection size is not calcullated
        /// </summary>
        /// <param name="source">A collection of any type</param>
        /// <param name="currentPage">Current page for paging implementation. currentPage = 1 by default</param>
        /// <param name="pageSize">Page size for paging implementation. pageSize = default value from appsettings;
        ///     if pageSize = -1,  pageSize = the size of a whole collection
        ///     </param>
        public PaginatedList(ICollection<T> source, int currentPage, int pageSize)
        {
            TotalItems = source.Count();

            CurrentPage = currentPage == 0 ? 1 : currentPage;
            PageSize = pageSize == 0 ? TotalItems : pageSize;

            if (pageSize == -1)
                TotalPages = TotalItems == 0 ? 0 : 1;
            else
                TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            this.AddRange(source.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
        }

        /// <summary>
        /// Performs in memory  paging for a collection of any type
        /// Use the class if a collection size 'totalCount' is already calcullated
        /// </summary>
        /// <param name="source">A collection of any type</param>
        /// <param name="currentPage">Current page for paging implementation. currentPage = 1 by default</param>
        /// <param name="pageSize">Page size for paging implementation. pageSize = default value from appsettings;
        ///     if pageSize = -1,  pageSize = the size of a whole collection
        ///     </param>
        /// <param name="totalCount">A collection size</param>
        private PaginatedList(ICollection<T> source, int currentPage, int pageSize, int totalCount) : base(source)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalCount;

            if (pageSize == -1)
                TotalPages = TotalItems == 0 ? 0 : 1;
            else
                TotalPages = PageSize == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
        }

        /// <summary>
        /// Performs in database paging for a collection of any type
        /// </summary>
        /// <param name="source">A query to a database with filterring (if needed)</param>
        /// <param name="currentPage">Current page for paging implementation. currentPage = 1 by default</param>
        /// <param name="pageSize">Page size for paging implementation. pageSize = default value from appsettings;
        ///     if pageSize = -1,  pageSize = the size of a whole collection
        ///     </param>
        /// <returns>A paginated list of any type</returns>
        public static async Task<PaginatedList<T>> FromIQueryable(IQueryable<T> source, int currentPage, int pageSize)
        {
            int totalItems = source.Count();
            currentPage = currentPage == 0 ? 1 : currentPage;
            pageSize = pageSize == -1 ? totalItems : pageSize;

            int totalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

            if (currentPage > totalPages)
                //return empty list
                return await Task.Run(() => new PaginatedList<T>(new List<T>(), currentPage, pageSize, totalItems));

            if (currentPage > 1)
                source = source.Skip((currentPage - 1) * pageSize);

            source = source.Take(pageSize);

            List<T> sourceList = source.ToList();
            return await Task.Run(() => new PaginatedList<T>(sourceList, currentPage, pageSize, totalItems));
        }
    }
}
