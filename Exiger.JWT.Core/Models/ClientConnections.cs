using System;
using System.ComponentModel.DataAnnotations;

namespace Exiger.JWT.Core.Models
{
    public class ClientConnections
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ClientUser LoginUser { get; set; }

        [Required]
        public ClientAccount ConnectionClientAccount { get; set; }

        [Required]
        public AuditActivityLog ConnectionAuditLog { get; set; }

        [Required]
        public DateTime TokenStartDateTimeUTC { get; set; }

        [Required]
        public DateTime TokenEndDateTimeUTC { get; set; }
    }
}