using System;
using System.ComponentModel.DataAnnotations;

namespace Exiger.JWT.Core.Models
{
    public class ClientIPAddresses
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ClientUser LoginUser { get; set; }

        [Required]
        public String IPAddress4 { get; set; }
    }
}