using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Utilities;
using Exiger.JWT.Core.Data.EF.Migrations;
using Exiger.JWT.Core.Data.EF.Repositories;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exiger.JWT.Core.Exceptions;
using System.Data.Entity.Validation;

namespace Exiger.JWT.Core.Data.EF
{
    public class UnitOfWork : IdentityDbContext<ClientUser, ApplicationIdentityRole, string, ApplicationIdentityUserLogin, ApplicationIdentityUserRole, ApplicationIdentityUserClaim>, IUnitOfWork
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Only callable from database seeing")]
        public UnitOfWork() : base(UnitOfWork.GetConnectionString())
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = true;

            (this as IObjectContextAdapter).ObjectContext.ObjectMaterialized += OnObjectMaterialized;

            Database.CommandTimeout = GetCommandTimeoutValue();
#if DEBUG
            Database.SetInitializer<UnitOfWork>(new MigrateDatabaseToLatestVersion<UnitOfWork, MigrationConfiguration>());
#else
			Database.SetInitializer<UnitOfWork>(null);
#endif
        }

        private static int GetCommandTimeoutValue()
        {
            try
            {
                return ConfigurationHelper.GetInt32Setting(Constants.COMMAND_TIMEOUT);
            }
            catch
            {
                return Constants.COMMAND_TIMEOUT_DEFAULT;
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationHelper.GetConnectionString(Constants.EXIGER_DB_CONNECTION_STRING);
        }

        #region DbSets
            public DbSet<AuditActivityLog> AuditAction { get; set; }
            public DbSet<ClientAccount> APIClientAccount { get; set; }
            public DbSet<ClientConnections> APIConnections { get; set; }
            public DbSet<ClientIPAddresses> APIClientIPAddresses { get; set; }     
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required for configuration")]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Guard.AgainstNull(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
            modelBuilder.Entity<ClientAccount>().HasKey(t => t.id);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var savedEntitiesCount = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return savedEntitiesCount;
            }
            catch (DbUpdateConcurrencyException concurrencyEx)
            {
                var errorMessage = "Database concurrency exception occurred while saving records.";
                throw new ExigerDbConcurrencyException(errorMessage, concurrencyEx);
            }
            catch (DbEntityValidationException validationEx)
            {
                var errorMessage = "Database validation errors occurred while saving records.";
                Exception ex = new ExigerValidationException(errorMessage, validationEx);

                var validationErrors = new Dictionary<string, string>();
                foreach (var err in validationEx.EntityValidationErrors.SelectMany(x => x.ValidationErrors))
                {
                    if (validationErrors.ContainsKey(err.PropertyName))
                    {
                        validationErrors[err.PropertyName] += ".  " + err.ErrorMessage;
                    }
                    else
                    {
                        validationErrors.Add(err.PropertyName, err.ErrorMessage);
                    }
                }

                foreach (var err in validationErrors)
                {
                    ex.Data.Add(err.Key, err.Value);
                }

                throw ex;
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            return this.SaveChangesAsync(CancellationToken.None);
        }

        public override int SaveChanges()
        {
            using (Task<int> saveChangesTask = this.SaveChangesAsync())
            {
                return saveChangesTask.Result;
            }
        }

        private void OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity;
            InterpretDateTimesAsUtc(entity);
        }

        private void InterpretDateTimesAsUtc(object entity)
        {
            var properties = entity.GetType().GetProperties().Where(x => x.PropertyType == typeof(DateTime?) || x.PropertyType == typeof(DateTime));
            foreach (var propInfo in properties)
            {
                var dt = (propInfo.PropertyType == typeof(DateTime?)) ? (DateTime?)propInfo.GetValue(entity) : (DateTime)propInfo.GetValue(entity);
                if (dt != null)
                {
                    propInfo.SetValue(entity, DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc));
                }
            }
        }

        #region IUnitOfWork

        private IUserRepository _userRepository;
        public IUserRepository UserRepository
        {
            get
            {
                if (this._userRepository == null)
                {
                    this._userRepository = new UserRepository(this);
                }

                return this._userRepository;
            }
        }

        private IClientAccountRepository _clientAccountRepository;
        public IClientAccountRepository ClientAccountRepository
        {
            get
            {
                if (this._clientAccountRepository == null)
                {
                    this._clientAccountRepository = new ClientAccountRepository(this);
                }

                return this._clientAccountRepository;
            }
        }

        private IAuditRepository _auditRepository;
        public IAuditRepository AuditRepository
        {
            get
            {
                if (this._auditRepository == null)
                {
                    this._auditRepository = new AuditRepository(this);
                }

                return this._auditRepository;
            }
        }

        public void Commit()
        {
            if (this.ChangeTracker.HasChanges())
            {
                this.SaveChanges();
            }
        }

        public async Task CommitAsync()
        {
            if (this.ChangeTracker.HasChanges())
            {
                await this.SaveChangesAsync();
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
