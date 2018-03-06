using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Exiger.JWT.Core.Data.EF.Repositories
{
    internal class ClientAccountRepository : IClientAccountRepository
    {
        private readonly UnitOfWork _dbContext;
        private readonly RepositoryBase _repositoryBase;

        public ClientAccountRepository(UnitOfWork context)
        {
            _dbContext = Guard.AgainstNull(context);
            _repositoryBase = new RepositoryBase(_dbContext);
        }

        public void CreateAccount(ClientAccount account)
        {
            _repositoryBase.Add(account);
        }

        public ClientAccount GetAccountById(int accountId, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true)
        {
            return GetAccountById(a => a, accountId, includes: includes, trackInContext: trackInContext);
        }

        public TOut GetAccountById<TOut>(Expression<Func<ClientAccount, TOut>> projection, int accountId, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.Find(projection, x => x.id == accountId, includes, trackInContext: trackInContext);
        }

        public IEnumerable<TOut> GetAccountsByIds<TOut>(Expression<Func<ClientAccount, TOut>> projection, IEnumerable<int> accountIds, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.FindAll(projection, x => accountIds.Contains(x.id), includes, trackInContext: trackInContext);
        }

        public IEnumerable<TOut> GetAccountsByUserId<TOut>(Expression<Func<ClientAccount, TOut>> projection, string userId, Expression<Func<ClientAccount, bool>> predicate = null, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            if (predicate == null)
            {
                return _repositoryBase.FindAll(projection, x => x.ClientUsers.Any(y => y.Id == userId), includes, trackInContext: trackInContext);
            }
            else
            {
                predicate = predicate.And(x => x.ClientUsers.Any(y => y.Id == userId));
                return _repositoryBase.FindAll(projection, predicate, includes, trackInContext: trackInContext);
            }
        }

        public TOut GetAccountByName<TOut>(Expression<Func<ClientAccount, TOut>> projection, string clientName, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.Find<ClientAccount, TOut>(projection, x => x.ClientName == clientName, includes, trackInContext);
        }

        public IEnumerable<TOut> GetAllAccounts<TOut>(Expression<Func<ClientAccount, TOut>> projection, Expression<Func<ClientAccount, bool>> predicate = null, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, IEnumerable<ISortCriterion<ClientAccount>> preSortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.FindAll(projection, predicate, includes, preSortCriteria, postSortCriteria, trackInContext);
        }
    }
}
