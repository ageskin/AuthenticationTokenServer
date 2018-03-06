using Exiger.JWT.Core.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Exiger.JWT.Core.Extensions
{
	public static class IQueryableExtensions
	{
		private const string METHOD_NAME_ORDER_BY = "OrderBy";
		private const string METHOD_NAME_ORDER_BY_DESCENDING = "OrderByDescending";
		private const string METHOD_NAME_THEN_BY = "ThenBy";
		private const string METHOD_NAME_THEN_BY_DESCENDING = "ThenByDescending";

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string sortField, ListSortDirection sortDirection)
		{
			Guard.AgainstNull(queryable);

			var expression = CreateLambdaExpression<T>(sortField);
			var methodName = sortDirection == ListSortDirection.Ascending ? METHOD_NAME_ORDER_BY : METHOD_NAME_ORDER_BY_DESCENDING;
			var types = new Type[] { queryable.ElementType, expression.Body.Type };
			var methodCallExpression = Expression.Call(typeof(Queryable), methodName, types, queryable.Expression, expression);

			return (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(methodCallExpression);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
		public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, string sortField, ListSortDirection sortDirection)
		{
			Guard.AgainstNull(queryable);

			var expression = CreateLambdaExpression<T>(sortField);
			var methodName = sortDirection == ListSortDirection.Ascending ? METHOD_NAME_THEN_BY : METHOD_NAME_THEN_BY_DESCENDING;
			var types = new Type[] { queryable.ElementType, expression.Body.Type };
			var methodCallExpression = Expression.Call(typeof(Queryable), methodName, types, queryable.Expression, expression);

			return (IOrderedQueryable<T>)queryable.Provider.CreateQuery<T>(methodCallExpression);
		}

		private static LambdaExpression CreateLambdaExpression<T>(string propertyName)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "p");
			Expression expression = parameterExpression;
			foreach (string member in propertyName.Split('.'))
			{
				expression = Expression.PropertyOrField(expression, member);
			}

			return Expression.Lambda(expression, parameterExpression);
		}
	}
}
