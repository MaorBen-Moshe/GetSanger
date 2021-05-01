using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models
{
    public enum ReportOption { Abuse, Harassment, Unprofessional, Ads, Other }; // need to fully implement

    public class Report
    {
        public string ReportId { get; set; }

        public string ReporterId { get; set; } // writer of report

        public string ReportedId { get; set; } // who reported on

        public ReportOption Reason { get; set; }
    }
}
