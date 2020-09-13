using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public partial class ReportFileModel
    {
        public int Id { get; set; }
        public string ReportNameAr { get; set; }
        public string ReportNameEn { get; set; }
        public bool? IsDefault { get; set; }
        public int? ReportType { get; set; }
        public int? ReportTypeId { get; set; }
    }
}
