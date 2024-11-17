using Reddit.Models;

namespace Reddit.Repositories
{
    public interface ICommunityRepository
    {
        public Task<PagedList<Community>> GetCommunities(int pageNumber, int pageSize, bool? isAscending = null, string? sortKey = null, string? searchKey = null);
    }
}
