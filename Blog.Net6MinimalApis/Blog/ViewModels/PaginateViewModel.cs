using Blog.ViewModels.Posts;

namespace Blog.ViewModels
{
    public class PaginateViewModel
    {

        public int Total { get; set; }
        public int NextPage { get; set; }
        public bool EndOfList { get; set; }
        public int PageSize { get; set; }
        public IList<ListPostsViewModel> Posts { get; set; }

    }
}
