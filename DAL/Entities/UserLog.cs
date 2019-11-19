using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class UserLog
    {
        [Key]
        public int UserLogID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string PageName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string OperationName { get; set; }

        public bool MobileView { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }


    }
}
