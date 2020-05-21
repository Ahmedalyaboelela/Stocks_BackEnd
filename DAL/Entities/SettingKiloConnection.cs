using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class SettingKiloConnection
    {
        [Key]
        public int SettingKiloID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string ServerName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string DatabaseName { get; set; }


        [Column(TypeName = "nvarchar(250)")]
        public string UserId { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string Password { get; set; }
       
    }
}
