using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Exiger.JWT.Core.Data.EF.Repositories
{
    internal class AuditRepository : IAuditRepository
    {
        private readonly UnitOfWork _dbContext;
        private readonly RepositoryBase _repositoryBase;

        public AuditRepository(UnitOfWork context)
        {
            _dbContext = Guard.AgainstNull(context);
            _repositoryBase = new RepositoryBase(_dbContext);
        }

        public int CreateAuditActivityLog(AuditActivityLog auditLog)
        {
            _repositoryBase.Add(auditLog);
            return auditLog.Id;
        }

        public TOut GetAuditActivityLogById<TOut>(Expression<Func<AuditActivityLog, TOut>> projection, int logId, IEnumerable<Expression<Func<AuditActivityLog, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.Find<AuditActivityLog, TOut>(projection, x => x.Id == logId, includes, trackInContext);
        }

        public void CreateClientConnectionLog(ClientConnections clientConnection)
        {
            _repositoryBase.Add(clientConnection);
        }

        public IEnumerable<TOut> GetConnectionsByClient<TOut>(Expression<Func<ClientConnections, TOut>> projection, int clientId, IEnumerable<Expression<Func<ClientConnections, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.FindAll<ClientConnections, TOut>(projection, x => x.ConnectionClientAccount.id == clientId, includes, trackInContext: trackInContext);
        }

        public IEnumerable<TOut> GetConnectionsByUser<TOut>(Expression<Func<ClientConnections, TOut>> projection, string userId, IEnumerable<Expression<Func<ClientConnections, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.FindAll<ClientConnections, TOut>(projection, x => x.LoginUser.Id == userId, includes, trackInContext: trackInContext);
        }
    }
}
