using System;
using System.Collections.Specialized;
using System.Web;

namespace EFGrid.Functions;

public static class Misc
{
    public static string RemoveQueryStringByKey(string url, string key)
    {
        var uri = new Uri(url);

        // this gets all the query string key value pairs as a collection
        NameValueCollection newQueryString = HttpUtility.ParseQueryString(uri.Query);

        // this removes the key if exists
        newQueryString.Remove(key);

        // this gets the page path from root without QueryString
        string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

        return newQueryString.Count > 0
            ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
            : pagePathWithoutQueryString;
    }

    // public static string GetQueryStringByKey(string url, string key)
    // {
    //     var uri = new Uri(url);
    //
    //     // this gets all the query string key value pairs as a collection
    //     NameValueCollection newQueryString = HttpUtility.ParseQueryString(uri.Query);
    //     newQueryString
    //
    // }
}

public class CalendarMemory
{
    public DateTime Date { get; set; }
}
