using CSAc4yObjectDBCap;
using CSAc4yObjectObjectService.Association;
using CSAc4yUtilityContainer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CSAc4yAssociationXmlExportImport
{

    public class SaveToFileSys
    {

        public SqlConnection _sqlConnection;

        public string sqlConnectionString;
        public string TemplateName;
        public string outPath;
        public string defaultPath;
        public string outPathProcess;
        public string outPathSuccess;
        public string outPathError;

        public SaveToFileSys() { }

        public SaveToFileSys(string newSqlConn, string newTemp, string newOut, string newProc, string newSucc, string newErr)
        {
            sqlConnectionString = newSqlConn;
            TemplateName = newTemp;
            outPath = newOut;
            outPathProcess = newProc;
            outPathSuccess = newSucc;
            outPathError = newErr;

            _sqlConnection = new SqlConnection(sqlConnectionString);
            _sqlConnection.Open();
        }
        
        public SaveToFileSys(string newSqlConn, string newOut)
        {
            sqlConnectionString = newSqlConn;
            outPath = newOut;

            _sqlConnection = new SqlConnection(sqlConnectionString);
            _sqlConnection.Open();
        }

        public SaveToFileSys(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public void ExportAllInstances(string outputPath)
        {

            if (String.IsNullOrEmpty(outputPath))
                throw new Exception("OUTPUTPATH nem lehet üres!");

            Ac4yAssociationObjectService.ListInstanceResponse listInstanceResponse =
                new Ac4yAssociationObjectService(_sqlConnection).ListInstance(
                    new Ac4yAssociationObjectService.ListInstanceRequest() { }
                );

            if (listInstanceResponse.Result.Fail())
                throw new Exception(listInstanceResponse.Result.Message);

            foreach (var element in listInstanceResponse.Ac4yAssociationList)
            {
                string xml = serialize(element, typeof(Ac4yAssociation));

                //WriteOut(xml, element.OriginGUID + "@" + element.OriginTemplateGUID + "@Ac4yAssociation", outPath);
                WriteOut(xml, element.GUID + "@Ac4yAssociation", outputPath);
            }

        } // ExportAllInstances
        
        public void ExportTargetAc4yObjectList(List<Ac4yAssociationTarget> list, string outputPath)
        {

            foreach (var element in list)
            {

                WriteOut(
                    new Ac4yUtility().GetAsXml(element)
                    , element.GUID + "@Ac4yAssociation"
                    , outputPath
                );

            }

        } // ExportAc4yObjectList

        public void ExportTargetListByNames(string templateName, string name, string outputPath)
        {

            if (String.IsNullOrEmpty(outputPath))
                throw new Exception("outputpath nem lehet üres!");

            if (String.IsNullOrEmpty(templateName))
                throw new Exception("templateName nem lehet üres!");

            if (String.IsNullOrEmpty(name))
                throw new Exception("name nem lehet üres!");

            Ac4yAssociationObjectService.ListTargetByNamesResponse response =
                new Ac4yAssociationObjectService(_sqlConnection).ListTargetByNames(
                    new Ac4yAssociationObjectService.ListTargetByNamesRequest() {
                    TemplateName=templateName, Name=name}
                );

            if (response.Result.Fail())
                throw new Exception(response.Result.Message);

            ExportTargetAc4yObjectList(response.Ac4yAssociationTargetList, outputPath);
            /*
            foreach (var element in response.Ac4yAssociationTargetList)
            {
                string xml = serialize(element, typeof(Ac4yAssociation));

                WriteOut(xml, element.GUID + "@Ac4yAssociation", outputPath);
            }
            */
        } // ExportAllInstances

        public static void WriteOut(string text, string fileName, string outputPath)
        {

            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Debug(fileName);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            File.WriteAllText(outputPath + fileName + ".xml", text);

        } // WriteOut

        public string serialize(Object taroltEljaras, Type anyType)
        {
            XmlSerializer serializer = new XmlSerializer(anyType);
            var xml = "";

            using (var writer = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    serializer.Serialize(writer, taroltEljaras);
                    xml = writer.ToString(); // Your XML
                }
            }
            //System.IO.File.WriteAllText(path, xml);

            return xml;
        }
        /*
        public void Load()
        {
            Ac4yObjectObjectService ac4YObjectObjectService = new Ac4yObjectObjectService(sqlConn);

            try
            {
                string[] files =
                    Directory.GetFiles(outPath, "*.xml", SearchOption.TopDirectoryOnly);

                Console.WriteLine(files.Length);

                foreach (var _file in files)
                {
                    string _filename = Path.GetFileNameWithoutExtension(_file);
                    Console.WriteLine(_filename);
                    System.IO.File.Move(outPath + _filename + ".xml", outPathProcess + _filename + ".xml");


                    string xml = readIn(_filename, outPathProcess);

                    Ac4yObjectHome tabla = (Ac4yObjectHome)deser(xml, typeof(Ac4yObjectHome));

                    SetByNamesResponse response = ac4YObjectObjectService.SetByNames(
                        new SetByNamesRequest() { TemplateName = TemplateName, Name = tabla.SimpledHumanId }
                        );

                    if (response.Result.Code.Equals("1"))
                    {
                        System.IO.File.Move(outPathProcess + _filename + ".xml", outPathSuccess + _filename + ".xml");

                    }
                    else
                    {
                        System.IO.File.Move(outPathProcess + _filename + ".xml", outPathError + _filename + ".xml");

                    }
                }
            }
            catch (Exception _exception)
            {
                Console.WriteLine(_exception.Message);
            }
        }
        */
        public Object deser(string xml, Type anyType)
        {
            Object result = null;

            XmlSerializer serializer = new XmlSerializer(anyType);
            using (TextReader reader = new StringReader(xml))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public string ReadIn(string fileName, string path)
        {

            string textFile = path + fileName + ".xml";

            string text = File.ReadAllText(textFile);

            return text;

        } // ReadIn

    } // SaveToFileSysAssociation

} // CSAc4yAssociationXmlExportImport