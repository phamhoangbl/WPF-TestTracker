using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Utils;

namespace TestTracker.Core.Utils
{
    public class FileExport
    {
        private const string STR_DIR_DMTEST = @"C:\DMTest";
        private const string STR_SEPARATING = "\r\n";

        public List<TestResult> CheckFileResult(out List<TestUnitResult> testUnitResultList)
        {
            string rootPath = STR_DIR_DMTEST;
            testUnitResultList = new List<TestUnitResult>();
            List<string> listFilePath = new List<string>();
            List<string> predictString = new List<string> { "DATE:", "DATE=", 
                                                            "STARTTIME:", "STARTTIME=", 
                                                            "DEVICENAME:", "DEVICENAME=",
                                                            "HBANAME:", "HBANAME=",
                                                            "TOTALLBA:", "TOTALLBA=",
                                                            "CAPACITY:", "CAPACITY=", 
                                                            "SCRIPTSTARTTIME:", "SCRIPTSTARTTIME=", 
                                                            "SCRIPTENDTIME:", "SCRIPTENDTIME=",
                                                            "SCRIPTENDDATE:", "SCRIPTENDDATE=",
                                                            "TOTALRUNTIME:", "TOTALRUNTIME=",
                                                            "TIME:", "TIME=", //TIME IS SAME TOTAL RUNTIME, BUT SOME OF FILES RESULT USE THE NAME
                                                            "OFERRORS:", "OFERRORS=",
                                                            "OFCOMMANDS:", "OFCOMMANDS=",
                                                            "SCRIPT:", "SCRIPT=", 
                                                            "MODELNUMBER:", "MODELNUMBER=",
                                                            "SERIALNUMBER:", "SERIALNUMBER=",
                                                            "FWREVISION:", "FWREVISION=",
                                                            "FWREVISION:", "FWREVISION=",
                                                            "FIRMWAREREVISION:", "FIRMWAREREVISION=",
                                                            "TESTEDBY:", "TESTEDBY=",
                                                            };

            List<TestResult> listFileResult = new List<TestResult>();

            //Get all path of files in rootpath
            if(Directory.Exists(rootPath))
            {
                listFilePath = Directory.GetFiles(@" " + rootPath + " ", "*.*", SearchOption.AllDirectories).ToList();
            }
            else
            {
                return listFileResult;
            }
            TestResult testResult = new TestResult();

            foreach (string s in listFilePath)
            {
                //Handle log files
                List<string> listLines = new List<string>(); //use for log file
                if (s.Contains(".log"))
                {
                    listLines = File.ReadLines(s).ToList();
                    testResult = new TestResult();
                    List<string> logResult = new List<string>();
                    logResult.Add(string.Format("Path of file:{0}{1}", STR_SEPARATING, s));

                    foreach(var listline in listLines)
                    {
                        string copyListLine = listline;
                        if (predictString.Any(x => copyListLine.Replace(" ", "").ToUpper().Contains(x)))
                        {
                            logResult.Add(listline);
                        }
                    }

                    CreateFileResultTemplate(out testResult, logResult);
                    listFileResult.Add(testResult);
                   
                }

                //Handle csv files
                else if (s.Contains(".csv"))
                {
                    List<List<string>> records = new List<List<string>>();
                    using (CsvReader cReader = new CsvReader(s, Encoding.Default))
                    {
                        while (cReader.ReadNextRecord())
                        {
                            records.Add(cReader.Fields);
                        }
                    }

                    testResult = new TestResult();
                    var csvResult = GetCSVResultString(records, predictString, s);

                    CreateFileResultTemplate(out testResult, csvResult);

                    
                    //get testUnitResult 
                    var listTestUnitResults = csvResult.Where(x=>x.Contains("PASS") || x.Contains("FAIL") || x.Contains("N/A"));
                    
                    foreach(var str in listTestUnitResults)
                    {
                        var testUnitResult = new TestUnitResult();
                        string[] attrs = Regex.Split(str, STR_SEPARATING);
                        testUnitResult.TestUnitName = attrs[0].Trim();
                        testUnitResult.TestValue = attrs[1].Trim();

                        testUnitResultList.Add(testUnitResult);
                    }

                    listFileResult.Add(testResult);
                }


            }


            return listFileResult;
        }

        //input: row (records)
        //output: result string
        private static List<string> GetCSVResultString(List<List<string>> records, List<string> predictString, string fileName)
        {
            List<string> csvResult = new List<string>();
            csvResult.Add(string.Format("Path of file:{0}{1}", STR_SEPARATING, fileName));

            foreach (List<string> record in records)
            {
                int colIndex = 1;
                //check if string in records contains needed information
                foreach (string col in record)
                {
                    //get test result                    
                    if (predictString.Any(x => col.Replace(" ", "").ToUpper().Contains(x)))
                    {
                        string recordStr = String.Join(STR_SEPARATING, record);
                        csvResult.Add(recordStr);
                    }

                    //get test unit result
                    if ((col.Contains("PASS") || col.Contains("FAIL") || col.Contains("N/A")) && colIndex == 2)
                    {
                        if(colIndex == 2)
                        {
                            string testUnitResult = record[0] + STR_SEPARATING + col;
                            csvResult.Add(testUnitResult);
                        }
                    }
                    colIndex++;
                }
            }

            return csvResult;
        }

        private static void CreateFileResultTemplate(out TestResult testResult, List<string> listAttribute)
        {
            testResult = new TestResult();

            foreach (string attribute in listAttribute)
            {
                //split attributes
                string[] attrs = Regex.Split(attribute, STR_SEPARATING);

                foreach (string attr in attrs)
                {
                    string copyAttr = attribute;
                    string compAttr = attribute.Replace(" ", "").ToUpper();

                    //for test result
                    if (compAttr.Contains("Pathoffile".ToUpper()))
                    {
                        testResult.FileName = copyAttr.Replace("Path of file:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("ScriptStartTime".ToUpper()))
                    {
                        testResult.ScriptStartTime = copyAttr.Replace("Script Start Time :", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("Date".ToUpper()))
                    {
                        testResult.ScriptStartTime = copyAttr.Replace("Date:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("StartTime".ToUpper()))
                    {
                        testResult.ScriptStartTime = copyAttr.Replace("StartTime : ", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("DeviceName".ToUpper()))
                    {
                        testResult.DeviceName = copyAttr.Replace("Device name : ", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("HBANAME".ToUpper()))
                    {
                        testResult.HBAName = copyAttr.Replace("Hba Name :", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("TotalLba".ToUpper()))
                    {
                        testResult.TotalLBA = copyAttr.Replace("Total LBA = ", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("Capacity".ToUpper()))
                    {
                        testResult.Capacity = copyAttr.Replace("Total LBA = ", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("ScriptStartDate".ToUpper()))
                    {
                        testResult.ScriptStartDate = copyAttr.Replace("Script Start Date:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("ScriptEndTime".ToUpper()))
                    {
                        testResult.ScriptEndTime = copyAttr.Replace("Script End Time:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("ScriptEndDate".ToUpper()))
                    {
                        testResult.ScriptEndDate = copyAttr.Replace("Script End Date:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("totalruntime".ToUpper()))
                    {
                        testResult.TotalRuntime = copyAttr.Replace("total run time        :   ", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("time".ToUpper()))
                    {
                        testResult.TotalRuntime = copyAttr.Replace(",Time:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("oferrors".ToUpper()))
                    {
                        testResult.TotalOfErrors = copyAttr.Replace("total # of errors     :         ", "").Replace("total # of errors     :        ", "").Replace(STR_SEPARATING, "").Trim(); ;
                    }
                    else if (compAttr.Contains("ofcommands".ToUpper()))
                    {
                        testResult.TotalOfCommands = copyAttr.Replace("total # of commands   :        ", "").Replace("total run time:  ", "").Trim();
                    }
                    else if (compAttr.Contains("script".ToUpper()))
                    {
                        testResult.ScriptName = copyAttr.Replace("SCRIPT: ", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("ModelNumber".ToUpper()))
                    {
                        testResult.ModelNumber = copyAttr.Replace("Model Number:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("SerialNumber".ToUpper()))
                    {
                        testResult.SerialNumber = copyAttr.Replace("Serial Number:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("FWRevision".ToUpper()))
                    {
                        testResult.FWRevision = copyAttr.Replace("FW Revision:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("FirmwareRevision".ToUpper()))
                    {
                        testResult.FWRevision = copyAttr.Replace("Firmware revision:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                    else if (compAttr.Contains("Testedby".ToUpper()))
                    {
                        testResult.TestedBy = copyAttr.Replace("Tested By:", "").Replace(STR_SEPARATING, "").Trim();
                    }
                }
            }
        }

        private string TrimAttribute(string attr)
        {
            List<string> symbols = new List<string>() { ":", " ", "="};
            while(symbols.Contains(attr[0].ToString()))
            {
                attr = attr.Substring(1);
            }
            return attr;
        }
    }
}
