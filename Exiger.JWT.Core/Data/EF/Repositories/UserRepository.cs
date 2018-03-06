using Exiger.JWT.Core.Data.EF.Identity;
using Exiger.JWT.Core.Exceptions;
using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Exiger.JWT.Core.Data.EF.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly UnitOfWork _dbContext;
        private readonly RepositoryBase _repositoryBase;
        private readonly UserManager<ClientUser, string> _userManager;

        public UserRepository(UnitOfWork context)
        {
            _dbContext = Guard.AgainstNull(context);
            _repositoryBase = new RepositoryBase(_dbContext);
            _userManager = IdentityFactory.CreateUserManager(_dbContext);
        }

        public void ChangeUserPassword(string userId, string currentPassword, string newPassword)
        {
            IdentityResult result = this._userManager.ChangePassword(userId, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                //throw new ExigerValidationException(Resource.UnexpectedErrorCreatingUserRecord, null, result.Errors.Select(x => new ValidationResult(x, new string[] { string.Empty })));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
        public ClaimsIdentity CreateIdentity(ClientUser user, string authenticationType)
        {
            Guard.AgainstNull(user);
            var claimsIdentity = _userManager.CreateIdentity(user, authenticationType);
            return claimsIdentity;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
        public void CreateUser(ClientUser user, string password = null)
        {
            Guard.AgainstNull(user);
            var identityResult = password != null ? _userManager.Create(user, password) : _userManager.Create(user);
            if (!identityResult.Succeeded)
            {
                throw new ExigerValidationException("Unexpected Error while Creating User Record", null, identityResult.Errors.Select(x => new ValidationResult(x, new string[] { "" })));
            }
        }

        public string GetPasswordResetToken(string userId)
        {
            return this._userManager.GeneratePasswordResetToken(userId);
        }

        public TOut GetUserById<TOut>(Expression<Func<ClientUser, TOut>> projection, string userId, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.Find<ClientUser, TOut>(projection, x => x.Id == userId, includes, trackInContext);
        }

        public async Task<TOut> GetUserByIdAsync<TOut>(Expression<Func<ClientUser, TOut>> projection, string userId, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return await _repositoryBase.FindAsync<ClientUser, TOut>(projection, x => x.Id == userId, includes, trackInContext);
        }

        public TOut GetUserByName<TOut>(Expression<Func<ClientUser, TOut>> projection, string userName, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.Find<ClientUser, TOut>(projection, x => x.UserName == userName, includes, trackInContext);
        }

        public IEnumerable<TOut> GetUsers<TOut>(Expression<Func<ClientUser, TOut>> projection, Expression<Func<ClientUser, bool>> predicate = null, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, IEnumerable<ISortCriterion<ClientUser>> preSortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true) where TOut : class
        {
            return _repositoryBase.FindAll<ClientUser, TOut>(projection, predicate, includes, preSortCriteria, postSortCriteria, trackInContext);
        }

         public async Task<bool> IsPasswordValidAsync(ClientUser user, string password)
        {
            var isValid = await _userManager.CheckPasswordAsync(user, password);

            return isValid;
        }

        public bool IsUserPasswordExpired(string id)
        {
            return false;
        }

        public void ResetUserPassword(string userId, string password)
        {
            this._userManager.UserTokenProvider = new EmailTokenProvider<ClientUser, string>();
            var resetToken = this._userManager.GeneratePasswordResetToken(userId);
            IdentityResult passwordChange = this._userManager.ResetPassword(userId, resetToken, password);

            if (!passwordChange.Succeeded)
            {
                throw new ExigerValidationException("Unexpected Error Resetting User Password", null, passwordChange.Errors.Select(x => new ValidationResult(x, new string[] { string.Empty })));
            }
        }

        public void ResetUserPassword(string userId, string resetToken, string newPassword)
        {
            IdentityResult result = this._userManager.ResetPassword(userId, resetToken, newPassword);

            if (!result.Succeeded)
            {
                throw new ExigerValidationException("Unexpected Error Creating User Password", null, result.Errors.Select(x => new ValidationResult(x, new string[] { string.Empty })));
            }
        }

        public bool UserExists(string userName)
        {
            return this._dbContext.Users.Any(user => user.UserName == userName);
        }

        public async Task<IdentityResult> UserFailedLogOn(ClientUser user)
        {
            return await _userManager.AccessFailedAsync(user.Id);
        }

        public void ResetUserLockout(string userId)
        {
            this._userManager.ResetAccessFailedCount(userId);
        }

        public void RemoveUser(ClientUser user)
        {
 
            this._repositoryBase.Remove(user);
        }

        public void UnlockUser(string userId)
        {
            this._userManager.SetLockoutEndDate(userId, DateTimeOffset.MinValue);
        }

        public void AttachUsers(IEnumerable<ClientUser> users)
        {
            foreach (var user in users)
            {
                _repositoryBase.Attach(user);
            }
        }
    }
}
