using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Exiger.JWT.Core.Data
{
    public class ExpressionSortCriterion<T, TKey> : ISortCriterion<T>
    {
        public Expression<Func<T, TKey>> SortExpression { get; set; }
        
        public ListSortDirection SortDirection { get; set; }

        public ExpressionSortCriterion(Expression<Func<T, TKey>> sortExpression, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            SortExpression = sortExpression;
            SortDirection = sortDirection;
        }

        #region ISortCriterion
        public IOrderedQueryable<T> Sort(IQueryable<T> query, bool sorted)
        {
			var orderedQuery = query as IOrderedQueryable<T>;

            if (SortDirection == ListSortDirection.Ascending)
            {
                return sorted ? orderedQuery.ThenBy(SortExpression) : query.OrderBy(SortExpression);
            }
            else
            {
                return sorted ? orderedQuery.ThenByDescending(SortExpression) : query.OrderByDescending(SortExpression);
            }
        } 
        #endregion
    }
}
