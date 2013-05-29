using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Group: Menyimpan informasi sebuah grup kontak
    public class Group
    {
        [Key]
        [Required]
        public int GroupId { get; set; }

        [Required]
        public string GroupName { get; set; }
        
        // User yang memiliki group ini
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}