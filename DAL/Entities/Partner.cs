using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Partner
    {
        [Key]
        public int PartnerID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string NameAR { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string NameEN { get; set; }

        public int IdentityType { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string IdentityNumber { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string IssuePlace { get; set; }

        public DateTime? IssueDate { get; set; }


        [Column(TypeName = "nvarchar(MAX)")]
        public string Address { get; set; }


        [Column(TypeName = "nvarchar(150)")]
        public string Mobile { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Phone1 { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Phone2 { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Fax { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string ConvertNumber { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Email { get; set; }

        public int AccountID { get; set; }

        public int? CountryID { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        [ForeignKey("CountryID")]
        public virtual Country Country { get; set; }



    }
}
