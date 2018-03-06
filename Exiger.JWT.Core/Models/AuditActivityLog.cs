using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exiger.JWT.Core.Models
{
    public class AuditActivityLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string SenderUserID { get; set; }

        [Required]
        public string SenderIP { get; set; }

        [Required]
        public string SenderRequestURL { get; set; }

        public ClientUser LoginUser { get; set; }

        [MaxLength(320)]
        public string InsightEmail { get; set; }

        public string AuthenticationToken { get; set; }

        public DateTime? TokenStartDateTimeUTC { get; set; }

        public DateTime? TokenEndDateTimeUTC { get; set; }

        public string ErrorMessage { get; set; }

        [Required]
        public DateTime AuditDateTime { get; set; }
    }
}