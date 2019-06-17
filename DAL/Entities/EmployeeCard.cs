using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class EmployeeCard
    {

        [Key]
        public int EmpCardId { get; set; }

        public int CardType { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string IssuePlace { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? RenewalDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Fees { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Notes { get; set; }

   
        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }

    }
}
