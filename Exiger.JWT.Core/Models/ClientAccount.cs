using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exiger.JWT.Core.Models
{
    public class ClientAccount
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; }
        [Required]
        public int TokenTimeOut { get; set; } = 0;
        [Required]
        public int OpenTokensLimit { get; set; } = 0;
        [Required]
        public bool WhitelistIPs { get; set; } = false;
        public virtual ICollection<ClientUser> ClientUsers { get; set; }
    }
}