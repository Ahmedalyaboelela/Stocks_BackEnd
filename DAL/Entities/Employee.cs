using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string NameAR { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string NameEN { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string ImagePath { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Nationality { get; set; }

        public int Religion { get; set; }

        public bool IsActive { get; set; }

        public bool IsInternal { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Profession { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string PassportProfession { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Email { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string Mobile { get; set; }

        public DateTime? BirthDate { get; set; }
        
        public int Age { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string BankAccNum { get; set; }

        public virtual ICollection<EmployeeCard> EmployeeCards { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<SellOrder> SellOrders { get; set; }
        public virtual ICollection<Notice> Notices { get; set; }
      
    }
}
