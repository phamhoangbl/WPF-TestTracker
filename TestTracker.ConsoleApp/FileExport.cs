using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Utils;

namespace TestTracker.ConsoleApp
{
    public class FileExport
    {
        private const string STR_DIR_DMTEST = @"C:\DMTest";

        public List<FileResult> CheckFileResult()
        {
            //[Trung]: handle csv and log file content
            #region "get infor from log file"

            string rootPath = STR_DIR_DMTEST;
            List<string> listFilePath = new List<string>();
            List<string> predictString = new List<string> { "Date:".ToUpper(), "Date=".ToUpper(), 
                                                            "StartTime:".ToUpper(), "StartTime=".ToUpper(), 
                                                            "DeviceName:".ToUpper(), "DeviceName=".ToUpper(),
                                                            "HBANAME:".ToUpper(), "HBANAME=".ToUpper(),
                                                            "TotalLba:".ToUpper(), "TotalLba=".ToUpper(),
                                                            "Capacity:".ToUpper(), "Capacity=".ToUpper(), 
                                                            "ScriptStartTime:".ToUpper(), "ScriptStartTime=".ToUpper(), 
                                                            "ScriptEndTime:".ToUpper(), "ScriptEndTime=".ToUpper(),
                                                            "ScriptEndDate:".ToUpper(), "ScriptEndDate=".ToUpper(),
                                                            "totalruntime:".ToUpper(), "totalruntime=".ToUpper(),
                                                            "time:".ToUpper(), "time=".ToUpper(), //Time is same total runtime, but some of files result use the name
                                                            "oferrors:".ToUpper(), "oferrors=".ToUpper(),
                                                            "ofcommands:".ToUpper(), "ofcommands=".ToUpper(),
                                                            "SCRIPT:".ToUpper(), "SCRIPT=".ToUpper(), 
                                                            "ModelNumber:".ToUpper(), "ModelNumber=".ToUpper(),
                                                            "SerialNumber:".ToUpper(), "SerialNumber=".ToUpper(),
                                                            "FWRevision:".ToUpper(), "FWRevision=".ToUpper(),
                                                            "Testedby:".ToUpper(), "Testedby=".ToUpper() };
            //Get all path of files in rootpath
            listFilePath = Directory.GetFiles(@" " + rootPath + " ", "*.*", SearchOption.AllDirectories).ToList();

            //Handle log files
            List<List<string>> records = new List<List<string>>();

            List<FileResult> listFileResult = new List<FileResult>();
            FileResult fileResult = new FileResult();

            foreach (string s in listFilePath)
            {
                List<string> listLines = new List<string>(); //use for log file
                List<string> listLogResult = new List<string>();
                if (s.Contains(".log"))
                {
                    List<string> logResult = new List<string>(); //get result of log files
                    listLines = File.ReadLines(s).ToList();

                    foreach (string line in listLines)
                    {
                        bool match = false;
                        //check if string in records contains needed information 
                        foreach (string x in predictString)
                        {
                            string lineTrimSpace = line.Replace(" ", "");
                            if (lineTrimSpace.ToUpper().Contains(x))
                                match = true;
                        }
                        if (match)
                        {
                            string valueLine = line;
                            while (valueLine[0] == ',' || valueLine[0] == ' ')
                            {
                                valueLine = valueLine.Substring(1);
                            }
                            listLogResult.Add(valueLine);
                        }
                    }
                    logResult.Add("Path of file: " + s);
                    foreach (string str in listLogResult)
                    {
                        logResult.Add(str);
                    }

                    fileResult = CreateFileResultTemplate(fileResult, logResult);
                }

                //Handle csv files
                else if (s.Contains(".csv"))
                {
                    List<string> csvResult = new List<string>();
                    csvResult.Add("Path of file: " + s);
                    using (CsvReader cReader = new CsvReader(s, Encoding.Default))
                    {
                        while (cReader.ReadNextRecord())
                        {
                            records.Add(cReader.Fields);
                        }
                    }

                    fileResult = new FileResult();
                    foreach (List<string> listStr in records)
                    {
                        bool match = false;
                        string resultString = null;

                        //check if string in records contains needed information 
                        foreach (string str in listStr)
                        {
                            foreach (string x in predictString)
                            {
                                string copyStr = str;
                                string lineTrimSpace = copyStr.Replace(" ", "");
                                if (lineTrimSpace.ToUpper().Contains(x))
                                    match = true;

                            }
                            if (match)
                            {
                                //restore full record and apply it to resultString
                                resultString += str;
                            }
                        }
                        if (match)
                        {
                            string valueLine = resultString;
                            while (valueLine[0] == ',' || valueLine[0] == ' ')
                            {
                                valueLine = valueLine.Substring(1);
                            }
                            csvResult.Add(valueLine);
                        }
                    }

                    fileResult = CreateFileResultTemplate(fileResult, csvResult);
                }


                listFileResult.Add(fileResult);
            }


            return listFileResult;

            #endregion

        }

        private static FileResult CreateFileResultTemplate(FileResult fileResult, List<string> listAttribute)
        {
            foreach (string attribute in listAttribute)
            {
                string copyAttr = attribute.Replace(" ", "").ToUpper();

                if (copyAttr.Contains("Pathoffile".ToUpper()))
                {
                    fileResult.FileName = attribute.Replace("Path of file:  ", "");
                }
                if (copyAttr.Contains("Date".ToUpper()))
                {
                    fileResult.StartDate = attribute.Replace(",Date : ", "").Replace("Date : ", "").Replace("Date: ", "").Replace("date ", "").Replace("date", "")
                        .Replace(",DATE :", "").Replace("DATE :", "").Replace("DATE:", "").Replace("DATE ", "").Replace("DATE", "");
                }
                else if (copyAttr.Contains("StartTime".ToUpper()))
                {
                    fileResult.StartTime = attribute.Replace(",StartTime : ", "").Replace("StartTime : ", "").Replace("StartTime: ", "").Replace("StartTime ", "").Replace("StartTime", "");
                }
                else if (copyAttr.Contains("DeviceName".ToUpper()))
                {
                    fileResult.DeviceName = attribute.Replace(",device name : ", "").Replace(",device name :", "").Replace("device name :", "").Replace("device name:", "").Replace("device name ", "").Replace("device name", "")
                        .Replace(",DEVICE NAME : ", "").Replace("DEVICE NAME : ", "").Replace("DEVICE NAME: ", "").Replace("DEVICE NAME ", "").Replace("DEVICE NAME", "");
                }
                else if (copyAttr.Contains("HBANAME".ToUpper()))
                {
                    fileResult.HBAName = attribute.Replace(",Hba Name :", "").Replace("Hba Name :", "").Replace("Hba Name:", "").Replace("Hba Name ", "").Replace("Hba Name", "")
                        .Replace(",HBA NAME : ", "").Replace("HBA NAME : ", "").Replace("HBA NAME: ", "").Replace("HBA NAME ", "").Replace("HBA NAME", "");
                }
                else if (copyAttr.Contains("TotalLba".ToUpper()))
                {
                    fileResult.TotalLBA = attribute.Replace(",TotalLba:,", "").Replace("TotalLba:,", "").Replace("TotalLba:,", "")
                        .Replace(",TotalLba :,", "").Replace("TotalLba :,", "").Replace("TotalLba :,", "").Replace("TotalLba ", "").Replace("TotalLba", "")
                        .Replace(",TotalLba :", "").Replace("TotalLba :", "").Replace("TotalLba :", "").Replace("TotalLba ", "").Replace("TotalLba", "")
                        .Replace(",Total LBA :", "").Replace("Total LBA :", "").Replace("Total LBA:", "").Replace("Total LBA ", "").Replace("Total LBA", "")
                        .Replace(",TOTAL LBA :", "").Replace("TOTAL LBA :", "").Replace("TOTAL LBA:", "").Replace("TOTAL LBA ", "").Replace("TOTAL LBA", "");
                }
                else if (copyAttr.Contains("Capacity".ToUpper()))
                {
                    fileResult.Capacity = attribute.Replace(",Capacity :,", "").Replace("Capacity :,", "").Replace("Capacity:,", "")
                        .Replace(",Capacity :", "").Replace("Capacity :", "").Replace("Capacity:", "").Replace("Capacity ", "").Replace("Capacity", "")
                        .Replace(",CAPACITY :", "").Replace("CAPACITY :", "").Replace("CAPACITY:", "").Replace("CAPACITY ", "").Replace("CAPACITY", "");
                }
                else if (copyAttr.Contains("ScriptStartTime".ToUpper()))
                {
                    fileResult.ScriptStartTime = attribute.Replace(",ScriptStartTime :", "").Replace("Script Start Time :", "").Replace("Script Start Time:", "").Replace("Script Start Time ", "").Replace("Script Start Time", "")
                        .Replace(",SCRIPT START TIME :", "").Replace("SCRIPT START TIME :", "").Replace("SCRIPT START TIME:", "").Replace("SCRIPT START TIME ", "").Replace("SCRIPT START TIME", "");
                }
                else if (copyAttr.Contains("ScriptEndTime".ToUpper()))
                {
                    fileResult.ScriptEndTime = attribute.Replace(",Script End Time :", "").Replace("Script End Time :", "").Replace("Script End Time:", "").Replace("Script End Time ", "").Replace("Script End Time", "")
                        .Replace(",SCRIPT END TIME :", "").Replace("SCRIPT END TIME :", "").Replace("SCRIPT END TIME:", "").Replace("SCRIPT END TIME ", "").Replace("SCRIPT END TIME", "");
                }
                else if (copyAttr.Contains("ScriptEndDate".ToUpper()))
                {
                    fileResult.ScriptEndDate = attribute.Replace(",Script End Date :", "").Replace("Script End Date :", "").Replace("Script End Date:", "").Replace("Script End Date ", "").Replace("Script End Date", "")
                        .Replace(",SCRIPT END DATE :", "").Replace("SCRIPT END DATE :", "").Replace("SCRIPT END DATE:", "").Replace("SCRIPT END DATE ", "").Replace("SCRIPT END DATE", "");
                }
                else if (copyAttr.Contains("totalruntime".ToUpper()))
                {
                    fileResult.TotalRuntime = attribute.Replace(",Total Runtime :", "").Replace("Total Runtime :", "").Replace("Total Runtime:", "").Replace("Total Runtime ", "").Replace("Total Runtime", "")
                        .Replace(",TOTAL RUNTIME :", "").Replace("TOTAL RUNTIME :", "").Replace("TOTAL RUNTIME:", "").Replace("TOTAL RUNTIME ", "").Replace("TOTAL RUNTIME", "");
                }
                else if (copyAttr.Contains("time".ToUpper()))
                {
                    fileResult.Time = attribute.Replace(",Time : ", "").Replace("Time : ", "").Replace("Time: ", "").Replace("Time ", "").Replace("Time", "")
                        .Replace(",TIME :", "").Replace("TIME :", "").Replace("TIME:", "").Replace("TIME ", "").Replace("TIME", "");
                }
                else if (copyAttr.Contains("oferrors".ToUpper()))
                {
                    fileResult.TotalOfErrors = attribute.Replace("total # of errors", "");
                }
                else if (copyAttr.Contains("ofcommands".ToUpper()))
                {
                    fileResult.TotalOfCommands = attribute.Replace("total # of errors", "");
                }
                else if (copyAttr.Contains("script".ToUpper()))
                {
                    fileResult.ScriptName = attribute.Replace(",Script :", "").Replace("Script :", "").Replace("Script:", "").Replace("Script ", "").Replace("Script", "")
                        .Replace(",script :", "").Replace("script :", "").Replace("script:", "").Replace("script ", "").Replace("script", "")
                        .Replace(",SCRIPT :", "").Replace("SCRIPT :", "").Replace("SCRIPT:", "").Replace("SCRIPT ", "").Replace("SCRIPT", "");
                }
                else if (copyAttr.Contains("ModelNumber".ToUpper()))
                {
                    fileResult.ModelNumber = attribute.Replace(",Model Number :", "").Replace("Model Number :", "").Replace("Model Number:", "").Replace("Model Number ", "").Replace("Model Number", "")
                        .Replace(",model number :", "").Replace("model number :", "").Replace("model number:", "").Replace("model number ", "").Replace("model number", "")
                        .Replace(",MODEL NUMBER :", "").Replace("MODEL NUMBER :", "").Replace("MODEL NUMBER:", "").Replace("MODEL NUMBER ", "").Replace("MODEL NUMBER", "");
                }
                else if (copyAttr.Contains("SerialNumber".ToUpper()))
                {
                    fileResult.SerialNumber = attribute.Replace(",Serial Number :", "").Replace("Serial Number :", "").Replace("Serial Number:", "").Replace("Serial Number ", "").Replace("Serial Number", "")
                        .Replace(",serial number :", "").Replace("serial number :", "").Replace("serial number:", "").Replace("serial number ", "").Replace("serial number", "")
                        .Replace(",SERIAL NUMBER :", "").Replace("SERIAL NUMBER :", "").Replace("SERIAL NUMBER:", "").Replace("SERIAL NUMBER ", "").Replace("SERIAL NUMBER", "");
                }
                else if (copyAttr.Contains("FWRevision".ToUpper()))
                {
                    fileResult.FWRevision = attribute.Replace(",FW Revision :", "").Replace("FW Revision :", "").Replace("FW Revision:", "").Replace("FW Revision ", "").Replace("FW Revision", "");
                }
                else if (copyAttr.Contains("Testedby".ToUpper()))
                {
                    fileResult.TestedBy = attribute.Replace(",Tested By :", "").Replace("Tested By :", "").Replace("Tested By:", "").Replace("Tested By ", "").Replace("Tested By", "")
                        .Replace(",Tested by :", "").Replace("Tested by :", "").Replace("Tested by:", "").Replace("Tested by ", "").Replace("Tested by", "");
                }
            }
            return fileResult;
        }
    }
}
