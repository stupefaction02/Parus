namespace Parus.Core.Entities
{
	public class PaginationData
    {
        public int Page { get; set; }
        public int PageCount { get; set; }

        public const int PAGE_SIZE = 12;
    }
}