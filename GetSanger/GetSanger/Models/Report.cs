using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models
{
    public enum ReportOption { Abuse, Harassment, Unprofessional, Ads, Other }; 

    public enum Status { Received, Handled } // for admin mode only

    public class Report
    {
        public string ReportId { get; set; }

        public string ReporterId { get; set; } // writer of report

        public string ReportedId { get; set; } // who reported on

        public ReportOption Reason { get; set; }

        public string ReportMessage { get; set; }

        public Status Status { get; set; }
    }
}
