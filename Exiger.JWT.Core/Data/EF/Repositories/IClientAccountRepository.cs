using Exiger.JWT.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Exiger.JWT.Core.Data.EF.Repositories
{
    public interface IClientAccountRepository
    {
        void CreateAccount(ClientAccount account);

        ClientAccount GetAccountById(int accountId, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true);

        TOut GetAccountById<TOut>(Expression<Func<ClientAccount, TOut>> projection, int accountId, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class;

        IEnumerable<TOut> GetAccountsByIds<TOut>(Expression<Func<ClientAccount, TOut>> projection, IEnumerable<int> accountIds, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class;

        IEnumerable<TOut> GetAccountsByUserId<TOut>(Expression<Func<ClientAccount, TOut>> projection, string userId, Expression<Func<ClientAccount, bool>> predicate = null, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class;

        TOut GetAccountByName<TOut>(Expression<Func<ClientAccount, TOut>> projection, string clientName, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, bool trackInContext = true) where TOut : class;


        IEnumerable<TOut> GetAllAccounts<TOut>(Expression<Func<ClientAccount, TOut>> projection, Expression<Func<ClientAccount, bool>> predicate = null, IEnumerable<Expression<Func<ClientAccount, object>>> includes = null, IEnumerable<ISortCriterion<ClientAccount>> presortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true) where TOut : class;
    }
}
