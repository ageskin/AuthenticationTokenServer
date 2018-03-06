using Exiger.JWT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Exiger.JWT.Core.Data.EF.Repositories
{
    internal sealed class RepositoryBase
    {
        private readonly UnitOfWork _dbContext;

        public RepositoryBase(UnitOfWork dbContext)
        {
            _dbContext = Guard.AgainstNull(dbContext);
        }

        private IQueryable<TOut> BuildFindAllQuery<TIn, TOut>(Expression<Func<TIn, TOut>> projection, Expression<Func<TIn, bool>> predicate = null, IEnumerable<Expression<Func<TIn, object>>> includes = null, IEnumerable<ISortCriterion<TIn>> preSortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            Guard.AgainstNull(projection);

            IQueryable<TIn> preProjectionQuery = _dbContext.Set<TIn>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    preProjectionQuery = preProjectionQuery.Include(include);
                }
            }

            if (predicate != null)
            {
                preProjectionQuery = preProjectionQuery.Where(predicate);
            }

            if (preSortCriteria != null)
            {
                bool sorted = false;
                foreach (var criteria in preSortCriteria)
                {
                    preProjectionQuery = criteria.Sort(preProjectionQuery, sorted);
                    sorted = true;
                }
            }

            IQueryable<TOut> postProjectionQuery = preProjectionQuery.Select(projection);

            if (postSortCriteria != null)
            {
                bool sorted = false;
                foreach (var criteria in postSortCriteria)
                {
                    postProjectionQuery = criteria.Sort(postProjectionQuery, sorted);
                    sorted = true;
                }
            }

            if (!trackInContext)
            {
                postProjectionQuery = postProjectionQuery.AsNoTracking();
            }

            return postProjectionQuery;
        }

        private IEnumerable<TOut> ApplyPostQueryPredicate<TOut>(List<TOut> list, Expression<Func<TOut, bool>> postPredicate = null)
            where TOut : class
        {
            if (postPredicate != null)
            {
                var postPredicateQuery = list.AsQueryable().Where(postPredicate);

                return postPredicateQuery.ToList();
            }
            else
            {
                return list;
            }
        }

        public IEnumerable<TOut> FindAll<TIn, TOut>(Expression<Func<TIn, TOut>> projection, Expression<Func<TIn, bool>> predicate = null, IEnumerable<Expression<Func<TIn, object>>> includes = null, IEnumerable<ISortCriterion<TIn>> preSortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true, Expression<Func<TOut, bool>> postPredicate = null)
            where TIn : class
            where TOut : class
        {
            var postProjectionQuery = BuildFindAllQuery(projection, predicate, includes, preSortCriteria, postSortCriteria, trackInContext);

            var list = postProjectionQuery.ToList();

            return ApplyPostQueryPredicate(list, postPredicate);
        }

        public async Task<IEnumerable<TOut>> FindAllAsync<TIn, TOut>(Expression<Func<TIn, TOut>> projection, Expression<Func<TIn, bool>> predicate = null, IEnumerable<Expression<Func<TIn, object>>> includes = null, IEnumerable<ISortCriterion<TIn>> preSortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true, Expression<Func<TOut, bool>> postPredicate = null)
            where TIn : class
            where TOut : class
        {
            var postProjectionQuery = BuildFindAllQuery(projection, predicate, includes, preSortCriteria, postSortCriteria, trackInContext);

            var list = await postProjectionQuery.ToListAsync();

            return ApplyPostQueryPredicate(list, postPredicate);
        }

        private IQueryable<TOut> BuildFindPageQuery<TIn, TOut>(Expression<Func<TIn, TOut>> projection, int pageNumber, int pageSize, Expression<Func<TIn, bool>> predicate = null, IEnumerable<Expression<Func<TIn, object>>> includes = null, IEnumerable<ISortCriterion<TIn>> preSortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            Guard.AgainstNull(projection);

            if (pageNumber <= 0)
            {
                throw new ArgumentException("Page number must be greater than 0", "pageNumber");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("Page size must be greater than 0", "pageSize");
            }

            IQueryable<TIn> preProjectionQuery = _dbContext.Set<TIn>();

            if (predicate != null)
            {
                preProjectionQuery = preProjectionQuery.Where(predicate);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    preProjectionQuery = preProjectionQuery.Include(include);
                }
            }

            if (preSortCriteria != null)
            {
                bool sorted = false;
                foreach (var criteria in preSortCriteria)
                {
                    preProjectionQuery = criteria.Sort(preProjectionQuery, sorted);
                    sorted = true;
                }
            }

            IQueryable<TOut> postProjectionQuery = preProjectionQuery.Select(projection);

            if (postSortCriteria != null)
            {
                bool sorted = false;
                foreach (var criteria in postSortCriteria)
                {
                    postProjectionQuery = criteria.Sort(postProjectionQuery, sorted);
                    sorted = true;
                }
            }

            if (!trackInContext)
            {
                postProjectionQuery = postProjectionQuery.AsNoTracking();
            }

            return postProjectionQuery;
        }

        private IQueryable<TOut> ApplyPaging<TOut>(IQueryable<TOut> query, int pageNumber, int pageSize)
            where TOut : class
        {
            return query.Skip((pageNumber - 1) * pageSize).Take<TOut>(pageSize);
        }

        private IQueryable<TOut> BuildFindQuery<TIn, TOut>(Expression<Func<TIn, TOut>> projection, Expression<Func<TIn, bool>> predicate, IEnumerable<Expression<Func<TIn, object>>> includes = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            Guard.AgainstNull(projection);
            Guard.AgainstNull(predicate);

            IQueryable<TIn> query = _dbContext.Set<TIn>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includes != null)
            {
                foreach (Expression<Func<TIn, object>> include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (!trackInContext)
            {
                query = query.AsNoTracking();
            }

            return query.Select(projection);
        }

        public TOut Find<TIn, TOut>(Expression<Func<TIn, TOut>> projection, Expression<Func<TIn, bool>> predicate, IEnumerable<Expression<Func<TIn, object>>> includes = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            var query = BuildFindQuery(projection, predicate, includes, trackInContext);

            return query.SingleOrDefault();
        }

        public async Task<TOut> FindAsync<TIn, TOut>(Expression<Func<TIn, TOut>> projection, Expression<Func<TIn, bool>> predicate, IEnumerable<Expression<Func<TIn, object>>> includes = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            var query = BuildFindQuery(projection, predicate, includes, trackInContext);

            return await query.SingleOrDefaultAsync();
        }

        private IQueryable<TOut> BuildGroupByQuery<TIn, TGroupType, TOut>(Expression<Func<TIn, TGroupType>> grouping, Expression<Func<IGrouping<TGroupType, TIn>, TOut>> projection, Expression<Func<TIn, bool>> predicate = null, IEnumerable<Expression<Func<TIn, object>>> includes = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            IQueryable<TIn> query = _dbContext.Set<TIn>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includes != null)
            {
                foreach (Expression<Func<TIn, object>> include in includes)
                {
                    query = query.Include(include);
                }
            }

            var group = query.GroupBy(grouping);
            var results = group.Select(projection);
            if (!trackInContext)
            {
                results = results.AsNoTracking();
            }

            return results;
        }

        public IEnumerable<TOut> GroupBy<TIn, TGroupType, TOut>(Expression<Func<TIn, TGroupType>> grouping, Expression<Func<IGrouping<TGroupType, TIn>, TOut>> projection, Expression<Func<TIn, bool>> predicate = null, IEnumerable<Expression<Func<TIn, object>>> includes = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            var results = BuildGroupByQuery(grouping, projection, predicate, includes, trackInContext);
            return results.ToList();
        }

        public async Task<IEnumerable<TOut>> GroupByAsync<TIn, TGroupType, TOut>(Expression<Func<TIn, TGroupType>> grouping, Expression<Func<IGrouping<TGroupType, TIn>, TOut>> projection, Expression<Func<TIn, bool>> predicate = null, IEnumerable<Expression<Func<TIn, object>>> includes = null, bool trackInContext = true)
            where TIn : class
            where TOut : class
        {
            var results = BuildGroupByQuery(grouping, projection, predicate, includes, trackInContext);
            return await results.ToListAsync();
        }

        public void Add<T>(T entity) where T : class
        {
            Guard.AgainstNull(entity);
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void Add<T>(IEnumerable<T> entities) where T : class
        {
            Guard.AgainstNull(entities);
            _dbContext.Set<T>().AddRange(entities);
        }

        public void Attach<T>(T entity) where T : class
        {
            if (entity == null || _dbContext.Entry(entity).State != EntityState.Detached)
            {
                return;
            }
            _dbContext.Set<T>().Attach(entity);
        }

        public void Remove<T>(T entity) where T : class
        {
            if (entity != null)
            {
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbContext.Set<T>().Attach(entity);
                }

                _dbContext.Set<T>().Remove(entity);
            }
        }

        public void RemoveAll<T>(Expression<Func<T, bool>> predicate, IEnumerable<Expression<Func<T, object>>> includes = null) where T : class
        {
            IEnumerable<T> entities = this.FindAll(x => x, predicate, includes);
            _dbContext.Set<T>().RemoveRange(entities);
        }

        private DbRawSqlQuery<T> BuildRawSqlQuery<T>(string query, params object[] parameters) where T : class
        {
            if (parameters == null)
            {
                return _dbContext.Database.SqlQuery<T>(query);
            }

            return _dbContext.Database.SqlQuery<T>(query, parameters);
        }

        public List<T> ExecuteRawSql<T>(string query, params object[] parameters) where T : class
        {
            var sqlQuery = BuildRawSqlQuery<T>(query, parameters);
            return sqlQuery.ToList();
        }

        public async Task<List<T>> ExecuteRawSqlAsync<T>(string query, params object[] parameters) where T : class
        {
            var sqlQuery = BuildRawSqlQuery<T>(query, parameters);
            return await sqlQuery.ToListAsync();
        }

        private static Expression<Func<T, bool>> GetNotDeletedFilter<T>(Type t)
        {
            var parameterExpression = Expression.Parameter(t);
            var propertyExpression = Expression.Property(parameterExpression, Constants.ISoftDeletableColumnIsDeleted);
            var notExpression = Expression.Not(propertyExpression);

            return Expression.Lambda<Func<T, bool>>(notExpression, parameterExpression);
        }
    }
}
