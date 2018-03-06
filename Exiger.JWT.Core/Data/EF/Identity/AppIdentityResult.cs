using System.Collections.Generic;

namespace Exiger.JWT.Core.Data.EF.Identity
{
    public class AppIdentityResult
    {
        public IEnumerable<string> Errors { get; private set; }
        public bool Succeeded { get; private set; }

        public AppIdentityResult(IEnumerable<string> errors, bool succeeded)
        {
            Succeeded = succeeded;
            Errors = errors;
        }
    }
}
