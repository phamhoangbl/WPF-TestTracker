using NLog;
using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;


namespace TestTracker.ConsoleApp
{
    public static class SvnSharpClient
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static bool UploadFile(string svnRepo, string userNameSvn, string passwordUserSvn, string dMTestPath, string dMTestSVNPath, string firmwareRevision, string partNumber, string serialNumber, string logMessage, out string testResultLocation, out string errorMessage)
        {
            errorMessage = string.Empty;
            testResultLocation = string.Empty;
            using (SvnClient client = new SvnClient())
            {
                try
                {
                    client.Authentication.DefaultCredentials = new System.Net.NetworkCredential(userNameSvn, passwordUserSvn);
                    var uri = client.GetUriFromWorkingCopy(dMTestSVNPath);
                    //check if 
                    if (uri == null)
                    {
                        // Checkout the code to the specified directory 
                        client.CheckOut(new Uri(svnRepo), dMTestSVNPath);

                        _logger.Info("New svn directory, check out succcessfully.");
                    }

                    //check if have new files in path of DMTest
                    var folder = new DirectoryInfo(dMTestPath);

                    var folderName = "DMTest" + DateTime.UtcNow.ToString("MMMMddyyyy-hh-mm-ss");
                    string destinationPath = string.Format(@"{0}\{1}\{2}\{3}\", dMTestSVNPath, firmwareRevision, partNumber, serialNumber);
                    testResultLocation = destinationPath + folderName;
                    var listFile = Directory.GetFiles(@" " + dMTestPath + " ", "*.*", SearchOption.AllDirectories).ToList();

                    if (folder.Exists && listFile.Any())
                    {
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                            _logger.Info("Created local svn directory successfully");
                        }
                        Directory.Move(dMTestPath, testResultLocation);
                        _logger.Info(string.Format("Moved file successully: {0}", destinationPath));
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
                    _logger.Info("Commited file svn successfully");

                    client.Update(dMTestSVNPath);
                    _logger.Info("Update file svn successfully");
                }
                catch (Exception ex)
                {
                    client.CleanUp(dMTestSVNPath);
                    _logger.Info("Cleaned up file svn successfully");
                    errorMessage = ex.InnerException.Message;
                    return false;
                }
                return true;
            }
        }
    }
}
