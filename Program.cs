using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Xml;
using System.Web.Script.Serialization;
using System.Web;
using System.Reflection;
using System.Net.Http;
using System.Xml.Linq;
using System.Linq;
using CareerBuilder.Support;

namespace CareerBuilder
{
    class DownloadDetails
    {
        public string jobStatus { get; set; }
        public string succeededlink { get; set; }
        public string failedlink { get; set; }
    }
    
    class getReturn
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
    }

    class Program
    {
        static string CreateJob()
        {
            StringBuilder postData = new StringBuilder();
            postData.Append("grant_type=" + HttpUtility.UrlEncode("password") + "&");
            postData.Append("username=" + HttpUtility.UrlEncode("USERNAME") + "&");
            postData.Append("password=" + HttpUtility.UrlEncode("PASSWOD"));
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postData.ToString());
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create("BASE SITE URL");
            }
            catch (UriFormatException)
            {
                request = null;
                Console.WriteLine("failed");
            }
            if (request == null) { throw new ApplicationException("invalid URL"); }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            request.UserAgent = "Mozilla/5.0 (IE 11.0; Windows NT 6.3; Trident/7.0; .NET4.0E; .NET4.0C; rv:11.0)";
            Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Flush();
            postStream.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var sampleObject = new JavaScriptSerializer().Deserialize<getReturn>(responseString);
                    return sampleObject.access_token;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        private static string GetObjectFromApiXML(string url, string headerVal)
        {
            var accept = "application/xml";
            var methodAction = "GET";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = methodAction;
            request.Accept = accept;
            request.Headers.Add("headerName", headerVal);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    return responseString;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        static void Main(string[] args)
        {
            double iter = 0;
            DataTable loadData = new DataTable();
            DataTable sampleData = new DataTable();
            #region Create Table Columns
            sampleData.Columns.Add("RowID", typeof(Int32));
            #endregion
            foreach (DataRow row in loadData.Rows)
            {
                iter++;
                string key = CreateJob();
                int rowNum;
                int.TryParse(row[0].ToString(), out rowNum);
                var searchString = "URL/?parameters=";
                try
                {
                    Console.WriteLine("Row Number: " + iter + " ID: " + row[0].ToString());
                    var searchResult = GetObjectFromApiXML(searchString, key);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(searchResult.ToString());
                    sampleData.Merge(Support.BuildData(doc, row[0].ToString()));
                    var rowFilePath = @"FilePath\" + row[0].ToString() + ".xml";
                    System.IO.StreamWriter rowFile = new System.IO.StreamWriter(rowFilePath);
                    string rowResult = XElement.Parse(searchResult).ToString();
                    rowFile.WriteLine(rowResult);
                    rowFile.Close();
                    XmlNodeList elems = doc.SelectNodes("//Path");
                    foreach (XmlNode elem in elems)
                    {
                        try
                        {
                            var elemID = elem.SelectSingleNode("ID").InnerText;
                            var elemResult = GetObjectFromApiXML("https://URL" + elemID + "path", key);
                            XmlDocument elemDoc = new XmlDocument();
                            elemDoc.LoadXml(elemResult.ToString());
                            sampleData.Merge(Support.BuildData(doc, elemID, row[0].ToString()));
                            var elemFilePath = @"FilePath\" + elemID + ".xml";
                            System.IO.StreamWriter elemFile = new System.IO.StreamWriter(elemFilePath);
                            elemResult = XElement.Parse(elemResult).ToString();
                            elemFile.WriteLine(elemResult);
                            elemFile.Close();
                        }
                        catch (Exception e)
                        {
                            //errorData.Rows.Add(rowNum, elem.SelectSingleNode("ID").InnerText, e.Message);
                            Console.WriteLine("Error: " + row[0].ToString());
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    //errorData.Rows.Add(rowNum, "NULL", e.Message);
                    Console.WriteLine("Error: " + row[0].ToString());
                    Console.WriteLine(searchString);
                    Console.WriteLine(e.Message);
                }
            }
            sampleData.Dispose();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
