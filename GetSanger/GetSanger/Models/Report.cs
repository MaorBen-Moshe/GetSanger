using System;

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

        public System.DateTime TimeReportCreated { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Report report &&
                   ReportId == report.ReportId &&
                   ReporterId == report.ReporterId &&
                   ReportedId == report.ReportedId &&
                   Reason == report.Reason &&
                   ReportMessage == report.ReportMessage &&
                   Status == report.Status &&
                   TimeReportCreated == report.TimeReportCreated;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReportId, ReporterId, ReportedId, Reason, ReportMessage, Status, TimeReportCreated);
        }
    }
}
