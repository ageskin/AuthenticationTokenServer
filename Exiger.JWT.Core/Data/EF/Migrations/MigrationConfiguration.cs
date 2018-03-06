using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Utilities;
using System;
using System.Data.Entity.Migrations;
using System.Diagnostics;

namespace Exiger.JWT.Core.Data.EF.Migrations
{
    public class MigrationConfiguration : DbMigrationsConfiguration<UnitOfWork>
    {
        public bool AssumeFirstTimeSetup { get; set; }

        public MigrationConfiguration()
        {
            this.AssumeFirstTimeSetup = false;
            this.AutomaticMigrationsEnabled = false;
            this.MigrationsDirectory = "Data\\EF\\Migrations";
            this.MigrationsNamespace = typeof(MigrationConfiguration).Namespace;
            this.CommandTimeout = GetCommandTimeoutValue();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
        protected override void Seed(UnitOfWork context)
        {
            Guard.AgainstNull(context);
#if DEBUG
            SeedDeveloperDatabase(context);
#endif
        }

        protected void SeedDeveloperDatabase(UnitOfWork context)
        {
            SeedTestAccount(context);
            SeedTestUser(context);
            context.SaveChanges();
        }

        private static void SeedTestAccount(UnitOfWork context)
        {
            var testClient = context.ClientAccountRepository.GetAccountById(1);
            if (testClient == null)
            {
                var newAccount = new ClientAccount
                {
                    ClientName = "Test API Client",
                };                
                context.ClientAccountRepository.CreateAccount(newAccount);
            }
        }

        private static void SeedTestUser(UnitOfWork context)
        {
            var testUser = context.UserRepository.GetUserByName(x => x, "supervisor");
            if (testUser == null)
            {
                var clientAccount = context.ClientAccountRepository.GetAccountById(1);
                testUser = new ClientUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "supervisor",
                    Email = "supervisor@ThirdPartyAccount.com",
                    Client = clientAccount,
                    Active = true,
                };
                context.UserRepository.CreateUser(testUser, "Password123!");
            }
        }

        private void SetupAspNetDatabaseSessionState()
        {
            var info = new ProcessStartInfo
            {
                Arguments = "-S localhost -E -ssadd -sstype c -d Exiger.OMS.Auth",
                CreateNoWindow = true,
                WorkingDirectory = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319",
                FileName = "aspnet_regsql.exe",
            };
            Process.Start(info);
        }
    }
}
