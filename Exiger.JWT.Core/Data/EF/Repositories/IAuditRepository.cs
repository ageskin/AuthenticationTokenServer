using Exiger.JWT.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Exiger.JWT.Core.Data.EF.Repositories
{
    public interface IAuditRepository
    {
        int CreateAuditActivityLog(AuditActivityLog auditLog);

        TOut GetAuditActivityLogById<TOut>(Expression<Func<AuditActivityLog, TOut>> projection, int logId, IEnumerable<Expression<Func<AuditActivityLog, object>>> includes = null, bool trackInContext = true) where TOut : class;

        void CreateClientConnectionLog(ClientConnections clientConnection);

        IEnumerable<TOut> GetConnectionsByClient<TOut>(Expression<Func<ClientConnections, TOut>> projection, int clientId, IEnumerable<Expression<Func<ClientConnections, object>>> includes = null, bool trackInContext = true) where TOut : class;

        IEnumerable<TOut> GetConnectionsByUser<TOut>(Expression<Func<ClientConnections, TOut>> projection, string userId, IEnumerable<Expression<Func<ClientConnections, object>>> includes = null, bool trackInContext = true) where TOut : class;
    }
}
