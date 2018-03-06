using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exiger.JWT.Core.Models
{
    public class ClientUser : IdentityUser<string, ApplicationIdentityUserLogin, ApplicationIdentityUserRole, ApplicationIdentityUserClaim>
    {
        [Required]
        public virtual ClientAccount Client { get; set; }
        [Required]
        public bool Active { get; set; }
        public virtual ICollection<ClientIPAddresses> UserIPAddresses { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Convention")]
    public class ApplicationIdentityUserLogin : IdentityUserLogin<string>
    {
    }

    public class ApplicationIdentityRole : IdentityRole<string, ApplicationIdentityUserRole>
    {
    }

    public class ApplicationIdentityUserRole : IdentityUserRole<string>
    {
    }

    public class ApplicationIdentityUserClaim : IdentityUserClaim<string>
    {
    }
}