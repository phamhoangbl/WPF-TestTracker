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
namespace TestTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logger _logger;
        public bool _isRun;
        public MainWindow()
        {
            InitializeComponent();
            BindControls();

            this.DataContext = this;
            _logger = LogManager.GetCurrentClassLogger();
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
            _isRun = true;  
            try
            {
                //Create a test queue


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
            if (_isRun)
            {
                CallDmMaster();
            }
        }

        private void CallDmMaster()
        {
            _logger.Info("Begin call DM Master exe");
            //TODO: validation here
            string fileDMPatch = _filePathTextBox.Text;
            List<string> scriptNames = GetFilecriptNames();
            string[] platform = ((ComboBoxItem)this._platformCombobox.SelectedItem).Tag.ToString().Split('-');
            string verdorId = platform[0];
            string deviceId = platform[1];
            string port = ((ComboBoxItem)this._port.SelectedItem).Tag.ToString();

            // Run Multi script Setup the process with the ProcessStartInfo class.
            foreach (var script in scriptNames)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = fileDMPatch;
                startInfo.RedirectStandardOutput = true;
                startInfo.Arguments = string.Format(@"/s:{0} /v:{1} /D:{2} /P:{3} /l:/e", script, verdorId, deviceId, port);
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                try
                {
                    using (Process process = Process.Start(startInfo))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            _logger.Info(string.Format("DM Master Output: {0}", result));
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.Error("Error exception when trying to call DM Master", ex);
                }
            }

            _isRun = false;

            LoadingAdorner.IsAdornerVisible = false;
            _mainForm.IsEnabled = true;
        }

        #endregion
    }
}
