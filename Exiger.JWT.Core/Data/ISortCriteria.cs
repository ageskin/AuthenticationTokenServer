using System.ComponentModel;
using System.Linq;

namespace Exiger.JWT.Core.Data
{
    public interface ISortCriterion<T>
    {
        ListSortDirection SortDirection { get; set; }

        IOrderedQueryable<T> Sort(IQueryable<T> query, bool sorted);
    }
}
