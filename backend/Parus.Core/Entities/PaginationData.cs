namespace Parus.Core.Entities
{
	public class PaginationContext
    {
        public int Page { get; set; }
        public int PageCount { get; set; }

        public string Path { get; set; }

        public const int PAGE_SIZE = 12;
    }
}