using Exiger.JWT.Core.Data.EF.Repositories;
using System;

namespace Exiger.JWT.Core.Data.EF
{
    public interface IUnitOfWork : IDisposable
    {
		IUserRepository UserRepository { get; }
        IClientAccountRepository ClientAccountRepository { get; }
        IAuditRepository AuditRepository { get; }

        void Commit();
    }
}
