using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CareerBuilder.Support
{
    class Support
    {
        internal static DataTable BuildData(XmlDocument doc, string ID)
        {
            using (DataTable dt = new DataTable())
            {
#region Create Table Columns
                dt.Columns.Add("ID",typeof(Int32));
#endregion
                int idOut;
                XmlNodeList entities = doc.SelectNodes("//Path");
                foreach (XmlNode entity in entities)
                {
                    DataRow dr = dt.NewRow();
                    int.TryParse(ID, out idOut);
                    dr["ID"] = idOut;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
        }
        internal static string GetNodeValue(XmlNode node)
        {
            string nodeValue = "";
            try
            {
                nodeValue = node.InnerText;
            }
            catch(NullReferenceException)
            {
                return "";
            }
            return nodeValue;
        }
    }
}
