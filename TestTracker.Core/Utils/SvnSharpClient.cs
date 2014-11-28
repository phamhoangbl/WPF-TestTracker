using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;


namespace TestTracker.Core.Utils
{
    public static class SvnSharpClient
    {
        public static bool UploadFile(string svnRepo, string userNameSvn, string passwordUserSvn, string dMTestPath, string dMTestSVNPath, string firmwareRevision, string partNumber, string serialNumber, string logMessage, out string errorMessage)
        {
            errorMessage = string.Empty;
            using (SvnClient client = new SvnClient())
            {
                try
                {
                    client.Authentication.DefaultCredentials = new System.Net.NetworkCredential("Auto.Tester", "12345");
                    var uri = client.GetUriFromWorkingCopy(dMTestSVNPath);

                    //check if 
                    if (uri == null)
                    {
                        // Checkout the code to the specified directory
                        client.CheckOut(new Uri(svnRepo), dMTestSVNPath);
                    }

                    //check if have new files in path of DMTest
                    var folder = new DirectoryInfo(dMTestPath);

                    var folderName = "DMTest" + DateTime.UtcNow.ToString("MMMMddyyyy-hh-mm-ss");
                    string destinationPath = string.Format(@"{0}\{1}\{2}\{3}\", dMTestSVNPath, firmwareRevision, partNumber, serialNumber);
                    var listFile = Directory.GetFiles(@" " + dMTestPath + " ", "*.*", SearchOption.AllDirectories).ToList();

                    if (folder.Exists && listFile.Any())
                    {
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                        Directory.Move(dMTestPath, destinationPath + folderName);
                    }
                    else
                    {
                        errorMessage = string.Format("No file is exist in the directory: {0}", folder);
                        return false;
                    }

                    Collection<SvnStatusEventArgs> changedFiles = new Collection<SvnStatusEventArgs>();
                    client.GetStatus(dMTestSVNPath, out changedFiles);

                    foreach (SvnStatusEventArgs changedFile in changedFiles)
                    {
                        if (changedFile.LocalContentStatus == SvnStatus.Missing)
                        {
                            client.Delete(changedFile.Path);
                        }
                        if (changedFile.LocalContentStatus == SvnStatus.NotVersioned)
                        {
                            client.Add(changedFile.Path);
                        }
                    }

                    SvnCommitArgs ca = new SvnCommitArgs();
                    ca.LogMessage = logMessage;

                    client.Commit(dMTestSVNPath, ca);
                    client.Update(dMTestSVNPath);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException.Message;
                    return false;
                }
                return true;
            }
        }
    }
}
