using System.Collections.Generic;
using System.Linq;

namespace EFGrid.Functions;

public class FakeDB
{
    private readonly Dictionary<string, string> reportNameToReport = new();
    public void SaveReport(string rptName, string report)
    {
        reportNameToReport[rptName] = report;
    }
    public string[] FetchReport()
    {
        return reportNameToReport.Keys.ToArray();
    }
    public string LoadReport(string rptName)
    {
        return reportNameToReport[rptName];
    }
    public string RemoveThenReturnFirstReport(string reportName)
    {
        reportNameToReport.Remove(reportName);
        if (reportNameToReport.Count > 0)
        {
            var firstRpt = reportNameToReport.First().Value;
            return firstRpt;
        }
        return string.Empty;
    }
    public void Rename(string oldName, string newRptName)
    {
        var rpt = reportNameToReport[oldName];
        reportNameToReport.Remove(oldName);

        reportNameToReport[newRptName] = rpt;
    }
}
