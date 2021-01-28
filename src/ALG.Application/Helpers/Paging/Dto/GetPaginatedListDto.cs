namespace ALG.Application.Helpers.Paging.Dto
{
    public class GetPaginatedListDto
    {
        public string Filter { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
