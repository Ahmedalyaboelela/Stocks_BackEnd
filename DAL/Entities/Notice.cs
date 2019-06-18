﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Notice
    {
        [Key]
        public int NoticeID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }
        public DateTime? NoticeDate { get; set; }
        public bool Type { get; set; }

        public int PortfolioID { get; set; }
        public int EmployeeID { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }
        public virtual ICollection<NoticeCreditorDeptor> NoticeCreditorDeptors { get; set; }
        public virtual ICollection<Entry> Entry { get; set; }

    }
}
