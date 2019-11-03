using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BAL.Model
{
    public class PartnerAttachmentModel
    {
        public int PartnerAttachID { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public int PartnerID { get; set; }

    }
}
