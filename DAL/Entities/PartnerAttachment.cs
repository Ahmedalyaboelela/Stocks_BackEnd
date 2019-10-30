using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class PartnerAttachment
    {
        [Key]
        public int PartnerAttachID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string FilePath { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string FileName { get; set; }

        public int PartnerID { get; set; }


        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}
