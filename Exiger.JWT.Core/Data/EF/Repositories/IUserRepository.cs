using Exiger.JWT.Core.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Exiger.JWT.Core.Data.EF.Repositories
{
    public interface IUserRepository
    {
        bool UserExists(string userName);

        ClaimsIdentity CreateIdentity(ClientUser user, string authenticationType);

        IEnumerable<TOut> GetUsers<TOut>(Expression<Func<ClientUser, TOut>> projection, Expression<Func<ClientUser, bool>> predicate = null, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, IEnumerable<ISortCriterion<ClientUser>> presortCriteria = null, IEnumerable<ISortCriterion<TOut>> postSortCriteria = null, bool trackInContext = true) where TOut : class;

        string GetPasswordResetToken(string userId);

        Task<bool> IsPasswordValidAsync(ClientUser user, string password);

        Task<IdentityResult> UserFailedLogOn(ClientUser user);

        TOut GetUserById<TOut>(Expression<Func<ClientUser, TOut>> projection, string userId, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, bool trackInContext = true) where TOut : class;

        Task<TOut> GetUserByIdAsync<TOut>(Expression<Func<ClientUser, TOut>> projection, string userId, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, bool trackInContext = true) where TOut : class;

        TOut GetUserByName<TOut>(Expression<Func<ClientUser, TOut>> projection, string userName, IEnumerable<Expression<Func<ClientUser, object>>> includes = null, bool trackInContext = true) where TOut : class;
     
        void ChangeUserPassword(string userId, string currentPassword, string newPassword);

        void CreateUser(ClientUser user, string password = null);

        void ResetUserPassword(string userId, string password);

        void ResetUserPassword(string userId, string resetToken, string newPassword);

        void ResetUserLockout(string userId);

        void RemoveUser(ClientUser user);

        void UnlockUser(string userId);

        void AttachUsers(IEnumerable<ClientUser> users);
    }
}
