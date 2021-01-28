namespace ALG.Application.Helpers.Paging.Dto
{
    public class PagingDto
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; private set; }
        public int TotalPages { get; set; }
        public bool IsFirst => CurrentPage == 1;
        public bool IsLast => ((CurrentPage == TotalPages) & (TotalPages > 0)) || (TotalPages == 0);

        public PagingDto(int currentPage, int pageSize, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}
