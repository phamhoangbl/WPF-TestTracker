using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using NLog;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Data.Repository;
using TestTracker.Core.Utils;

namespace TestTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestQueue _testQueueRunning = null;
        private TestStuff _testStuffRunning = null;
        private Logger _logger;
        private bool _isClickRunTest;
        private bool _isRunDMMaster;
        private TimeSpan _timeSpan;
        public MainWindow()
        {
            InitializeComponent();
            BindControls();

            this.DataContext = this;
            _logger = LogManager.GetCurrentClassLogger();
            TimeSpan span = TimeSpan.Zero;
        }

        private void ChangeFilePatch_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Applications (*.exe)|*.exe";

            // Display OpenFileDialog by calling ShowDialog method
            DialogResult result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result.ToString() == "OK")
            {
                // Open document
                string filename = dlg.FileName;
                _filePathTextBox.Text = filename;
            }
        }

        private void ChangeUlinkFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ... Get control that raised this event.
            var textBox = sender as System.Windows.Controls.TextBox;
            BindControls(textBox.Text);
        }

        private void ChangeUlinkFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select ULINK folder in your compture";
                dlg.SelectedPath = "~/Desktop";
                dlg.ShowNewFolderButton = true;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    _folderPathTextBox.Text = dlg.SelectedPath;
                    BindControls(_folderPathTextBox.Text);
                }
            }
        }

        private void ChechkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckAllOrNot(true);
        }

        private void UnCheckedAll_Click(object sender, RoutedEventArgs e)
        {
            CheckAllOrNot(false);
        }

        private void ToogleExpand_Click(object sender, RoutedEventArgs e)
        {
            ToogleExapnd();
        }

        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            _isClickRunTest = true;  
            try
            {
                SetUpTimer();
            }
            catch
            {
                // Log error.
            }

            LoadingAdorner.IsAdornerVisible = true;
            _mainForm.IsEnabled = false;
        }

        #region Private Metods

        private void BindControls(string ulinkFolderStr = null)
        {
            BindListFileScriptCheckbox(ulinkFolderStr);
        }

        private void BindListFileScriptCheckbox(string ulinkFolderStr)
        {
            if ( _scriptFileTreeView == null || _scriptFileTreeView.Items == null)
            {
                return;
            }

            _scriptFileTreeView.Items.Clear();
            string testScriptFolderDir;
            //Use Folder default is Desktop
            if (String.IsNullOrEmpty(ulinkFolderStr))
            {
                string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                testScriptFolderDir = pathDesktop + "\\ULINK_Protocol_v3.4_Virtium";
            }
            //Use custom folder
            else
            {
                testScriptFolderDir = ulinkFolderStr;
            }
            if(!Directory.Exists(testScriptFolderDir)) 
            {
                _scriptFileTreeView.Visibility = Visibility.Hidden;
            }
            else
            {
                _scriptFileTreeView.Visibility = Visibility.Visible;
                string[] testScriptFolderPaths = Directory.GetDirectories(testScriptFolderDir, "*", System.IO.SearchOption.AllDirectories);

                //Get file scripts in the parent directory
                var parentScripts = Directory.GetFiles(testScriptFolderDir, "*.*", System.IO.SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".srt")).ToList();

                CreateScriptNodes(GetFileName(testScriptFolderDir), testScriptFolderDir, parentScripts);

                //Get file scripts in the child directory

                foreach(var testScriptFolderPath in testScriptFolderPaths)
                {
                    string testScriptFolder = GetFileName(testScriptFolderPath);

                    //get all file .srt
                    var childScripts = Directory.GetFiles(testScriptFolderPath, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".srt")).ToList();

                    CreateScriptNodes(testScriptFolder, testScriptFolderPath, childScripts);
                }

                //if treeview have no any file .srt, make an alert to user
                if (_scriptFileTreeView.Items.Count == 0)
                {
                    System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                    label.Content = "There is no file script";
                    _scriptFileTreeView.Items.Add(label);
                }
            }
        }

        private void CreateScriptNodes(string testScriptFolder, string testScriptFolderPath, List<string> childScripts)
        {
            TreeViewItem treeNode = new TreeViewItem();
            treeNode.Margin = new Thickness(10, 10, 10, 10);
            treeNode.Header = testScriptFolder;
            treeNode.ToolTip = testScriptFolderPath;

            if (testScriptFolder == "Main")
            {
                treeNode.IsExpanded = true;
            }

            //Add list checkbox to treeNode
            foreach (var file in childScripts)
            {
                var checkbox = new System.Windows.Controls.CheckBox();
                checkbox.Content = GetFileName(file);
                checkbox.ToolTip = file;
                checkbox.Margin = new Thickness(3, 3, 3, 3);

                string name = @"_scriptCheckbox" + checkbox.Content.ToString().Replace(".srt", "");
                checkbox.Name = name;

                treeNode.Items.Add(checkbox);
            }
            if (treeNode.Items.Count != 0)
            {
                _scriptFileTreeView.Items.Add(treeNode);
            }
        }

        private void CheckAllOrNot(bool isCheckAll)
        {
            //Find add script checkbox
            var treeView = (System.Windows.Controls.TreeView)this.FindName("_scriptFileTreeView");
            foreach (var item in treeView.Items)
            {
                var treeViewItem = item as System.Windows.Controls.TreeViewItem;
                foreach (var checkbox in treeViewItem.Items)
                {
                    if (checkbox.GetType() == typeof(System.Windows.Controls.CheckBox))
                    {
                        var scriptCheckbox = checkbox as System.Windows.Controls.CheckBox;
                        scriptCheckbox.IsChecked = isCheckAll;
                    }
                }
            }

            if(isCheckAll)
            {
                _checkAllButton.IsEnabled = false;
                _unCheckedAllButton.IsEnabled = true;
            }
            else
            {
                _checkAllButton.IsEnabled = true;
                _unCheckedAllButton.IsEnabled = false;
            }
        }

        private void ToogleExapnd()
        {
            bool isExpanded = true;
            if (_toogleExpandButton.Content.ToString() == "Collapse")
            {
                isExpanded = false;
                _toogleExpandButton.Content = "Expand";
            }
            else
            {
                _toogleExpandButton.Content = "Collapse";
            }

            var treeView = (System.Windows.Controls.TreeView)this.FindName("_scriptFileTreeView");
            foreach (var item in treeView.Items)
            {
                var treeViewItem = item as System.Windows.Controls.TreeViewItem;
                treeViewItem.IsExpanded = isExpanded;
            }
        }

        private string GetFileName(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf("\\", filePath.Length - 1) + 1);
        }

        private List<string> GetFilecriptNames()
        {
            List<string> names = new List<string>();
            var treeView = (System.Windows.Controls.TreeView)this.FindName("_scriptFileTreeView");
            foreach (var item in treeView.Items)
            {
                var treeViewItem = item as System.Windows.Controls.TreeViewItem;
                foreach (var checkbox in treeViewItem.Items)
                {
                    if (checkbox.GetType() == typeof(System.Windows.Controls.CheckBox))
                    {
                        var scriptCheckbox = checkbox as System.Windows.Controls.CheckBox;
                        if (scriptCheckbox.IsChecked != null)
                        {
                            if ((bool)scriptCheckbox.IsChecked)
                            {
                                names.Add(scriptCheckbox.ToolTip.ToString());
                            }
                        }
                    }
                }
            }
            return names;
        }

        private void SetUpTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_isClickRunTest)
            {
                CreateTestQueue();
            }
            if (_isRunDMMaster)
            {
                CallDmMaster();
            }
        }

        private void CreateTestQueue()
        {
            //TODO: validation here
            List<string> scriptNames = GetFilecriptNames();
            string[] platform = ((ComboBoxItem)this._platformCombobox.SelectedItem).Tag.ToString().Split('-');
            string verdorId = platform[0];
            string deviceId = platform[1];
            string port = ((ComboBoxItem)this._port.SelectedItem).Tag.ToString();
            string computerName = System.Environment.MachineName;
            //Process Creating a test queue
            foreach (var scriptName in scriptNames)
            {
                _logger.Info(String.Format("Creating Test Queue: scriptName: {0}, verdorId: {1}, deviceId: {2}, port {3}", scriptName, verdorId, deviceId, port));
                try
                {
                    int testStuffId;
                    var testStuffRepository = new TestStuffRepository();
                    TestStuff hasAlready = testStuffRepository.Select(deviceId, verdorId, port, computerName);
                    if (hasAlready == null)
                    {
                        //create a test stuff
                        TestStuff testStuff = new TestStuff();
                        testStuff.DeviceId = deviceId;
                        testStuff.VerdorId = verdorId;
                        testStuff.Port = port;
                        testStuff.ComputerName = computerName;
                        testStuffRepository.Insert(testStuff, out testStuffId);
                    }
                    else
                    {
                        testStuffId = hasAlready.TestStuffId;
                    }
                    _logger.Info(string.Format("Created Test Stuff with Id {0}", testStuffId));

                    //create test queue
                    var testQueueRepository = new TestQueueRepository();
                    TestQueue testQueue = new TestQueue();
                    testQueue.ScriptName = scriptName;
                    testQueue.StartedTime = DateTime.UtcNow;
                    testQueue.FinishedTime = null;
                    testQueue.TestStuffId = testStuffId;
                    testQueue.TestResultId = null;
                    //if there is queue has running, then skip pending, if not then set status = running
                    var hasRunning = testQueueRepository.HasRunning();
                    if(hasRunning)
                    {
                        testQueue.TestStatusId = (int)EnumTestStatus.Pending;
                    }
                    else
                    {
                        testQueue.TestStatusId = (int)EnumTestStatus.Running;
                    }
                    testQueueRepository.Insert(testQueue);
                }
                catch(Exception ex)
                {
                    _logger.Error("Exception when creating a test queue", ex);
                }
            }
            _testQueueDataGrid.DataBind();
            _isClickRunTest = false;
            LoadingAdorner.IsAdornerVisible = false;
            _mainForm.IsEnabled = true;
            _isRunDMMaster = true;
        }

        private void CallDmMaster()
        {
            _logger.Info("Begin call DM Master exe");
            try
            {
                var testQueueRepository = new TestQueueRepository();
                _testQueueRunning = testQueueRepository.SelectQueueRunning();

                _logger.Info(String.Format("Retrieve Test Queue with ID: {0}", _testQueueRunning.TestQueueId));

                if (!File.Exists(_testQueueRunning.ScriptName))
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("File {0} is not exist", _testQueueRunning.ScriptName));
                    _isRunDMMaster = false;
                    return;
                }
                if (!File.Exists(_filePathTextBox.Text))
                {
                    System.Windows.Forms.MessageBox.Show(string.Format("File {0} is not exist", _filePathTextBox.Text));
                    _isRunDMMaster = false;
                    return;
                }

                var testStuffRepository = new TestStuffRepository();
                _testStuffRunning = testStuffRepository.SelectByID(_testQueueRunning.TestStuffId);
                _logger.Info(String.Format("Retrieve Test Stuff with ID: {0}", _testStuffRunning.TestStuffId));

                //Run Script to defect device
                //RunScriptDefectDevice();
                //Run Test Script User chosen
                RunTestScript();
            }
            catch (Exception ex)
            {
                _logger.Error("error exception when trying to call dm master", ex);
                System.Windows.Forms.MessageBox.Show("Error exception when trying to execute the file path, please check your network");
                _isRunDMMaster = false;
            }
        }

        private void RunTestScript()
        {
            // Run Multi script Setup the process with the ProcessStartInfo class.
            // Retrieve Queues have status = Running
            string fileDMPatch = _filePathTextBox.Text;

            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
            startinfo.FileName = fileDMPatch;
            startinfo.RedirectStandardOutput = true;

            startinfo.Arguments = string.Format(@"/s:{0} /v:{1} /d:{2} /p:{3} /l:/e", _testQueueRunning.ScriptName, _testStuffRunning.VerdorId, _testStuffRunning.DeviceId, _testStuffRunning.Port);

            //start the process with the info we specified.
            //call waitforexit and then the using statement will close.

            using (Process process = Process.Start(startinfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    _logger.Info(string.Format("dm master output: {0}", result));

                    //when done process, check file export: excels and logs

                }
            }

            _isRunDMMaster = false;
            
        }

        private void RunScriptDefectDevice()
        {
            string fileDMPatch = _filePathTextBox.Text;
            string fileScriptPath = @"\UlinkScripts\IdentifyDevice-V1.Log";
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
            startinfo.FileName = fileDMPatch;
            startinfo.RedirectStandardOutput = true;

            startinfo.Arguments = string.Format(@"/s:{0} /v:{1} /d:{2} /p:{3} /l:/e", fileScriptPath, _testStuffRunning.VerdorId, _testStuffRunning.DeviceId, _testStuffRunning.Port);

            using (Process process = Process.Start(startinfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    _logger.Info(string.Format("dm master output: {0}", result));

                    //when done process, defect device info from file log

                }
            }
        }

        #endregion
    }
}
