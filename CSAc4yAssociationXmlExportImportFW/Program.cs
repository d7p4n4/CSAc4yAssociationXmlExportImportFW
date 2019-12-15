
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CSAc4yAssociationXmlExportImportFW
{
    class Program
    {
        private static readonly log4net.ILog _naplo = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static string connectionString = ConfigurationManager.AppSettings["connectionString"];
        public static SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["conneectionString"]);
        public static SqlConnection sqlConnectionXML = new SqlConnection(ConfigurationManager.AppSettings["connectionStringXML"]);
        public static string TemplateName = ConfigurationManager.AppSettings["TemplateName"];
        public static string outPath = ConfigurationManager.AppSettings["Path"];
        public static string defaultPath = ConfigurationManager.AppSettings["DefaultPath"];
        public static string outPathProcess = defaultPath + ConfigurationManager.AppSettings["PathProcess"];
        public static string outPathSuccess = defaultPath + ConfigurationManager.AppSettings["PathSuccess"];
        public static string outPathError = defaultPath + ConfigurationManager.AppSettings["PathError"];

        static void Main(string[] args)
        {
            try
            {
                SaveToFileSysAssociationFW saveToFileSysAssociationFW = new SaveToFileSysAssociationFW(connectionString, TemplateName, outPath, outPathProcess, outPathSuccess, outPathError);

                saveToFileSysAssociationFW.WriteOutAc4yAssociationAll();
                /*
                List<SerializationObject> xmls = getXmls.GetXmlsMethod(sqlConn, sqlConnXML, TemplateName);
                foreach(var xml in xmls)
                {
                    writeOut(xml.xml, xml.fileName, outPath);
                }
                */
            }
            catch (Exception exception)
            {
                _naplo.Error(exception.StackTrace);
            }

        }

    }
}
