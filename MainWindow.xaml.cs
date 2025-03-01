using System;
using System.Collections.Generic;
using System.IO;
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
using IWshRuntimeLibrary;

namespace LinkRenameTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = textBox_path.Text;
            string oldText = textBox_searchFor.Text;
            string newText = textBox_replace.Text;
            bool r = (bool)checkBox_rekursiv.IsChecked;
            execute(path, oldText, newText, r);
            update_Link();
            
        }


        void execute(string LinkPathName = @"D:\100_Nextcloud\400_Programmierung\LinkRenameTool\LinkRenameTool\Link.lnk", string oldText = @"C:\Users\Pascal\Documents\Buchungsformular_schnabel.pdf", string newText = "", bool searchSubdirectory = false)
        {
            pathType check = CheckPath(LinkPathName);
            if (check == pathType.invalidPath)
            {
                // MessageBox.Show($"invalid Path {LinkPathName}");
                return;
            }
            if (check == pathType.valid_path)
            {
                List<string> paths = FindLnkFiles(LinkPathName, searchSubdirectory);
                int count = paths.Count;
                foreach (string s in paths)
                {
                    
                    changeLink(s, oldText, newText);
                }
                MessageBox.Show($"{count} Shortcuts found and changed");
                return;

            }
            if (check == pathType.shortcut)
            {
                changeLink(LinkPathName, oldText, newText);
            }

        }


        void changeLink(string LinkPathName, string oldText = ".pdf", string newText = ".txt", bool showMessage = false)
        {
            LinkPathName = RemoveParentheses(LinkPathName);
            WshShell shell = new WshShell(); //https://learn.microsoft.com/en-us/troubleshoot/windows-client/admin-development/create-desktop-shortcut-with-wsh
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(LinkPathName);
            string oldPath = link.TargetPath;
            string newPath = oldPath;
            try
            {
                newPath = oldPath.Replace(oldText, newText);
                newPath = System.Text.RegularExpressions.Regex.Replace(oldPath, oldText, newText);
            }
            catch
            {
                //oldPath = newPath;
            }
            
            link.TargetPath = newPath;
            //link.Description = "new";
            //link.WorkingDirectory =;
            link.Save();

            if (showMessage)
            {
                MessageBox.Show($"Link changed from {oldPath} to {newPath}");
            }
        }

        List<string> FindLnkFiles(string directoryPath, bool searchSubdirectories = true)
        {
            List<string> lnkFiles = new List<string>();

            try
            {
                // Determine the search option based on whether to include subdirectories
                SearchOption searchOption = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                // Search for all .lnk files in the specified directory
                string[] files = Directory.GetFiles(directoryPath, "*.lnk", searchOption);

                // Add all found .lnk files to the list
                lnkFiles.AddRange(files);
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., unauthorized access, etc.)
                MessageBox.Show($"An error occurred while searching for .lnk files: {ex.Message}");
            }

            return lnkFiles;
        }

        public string RemoveParentheses(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove leading parenthesis
            if (input.StartsWith("\""))
                input = input.Substring(1);

            // Remove trailing parenthesis
            if (input.EndsWith("\""))
                input = input.Substring(0, input.Length - 1);

            return input;
        }

        


        public pathType CheckPath(string path)
        {
            path = RemoveParentheses(path);

            bool invalidPath = true;
            try
            {
                if (System.IO.Path.IsPathRooted(path))
                {
                    invalidPath = false;
                }
            }
            catch
            {
                invalidPath = true;

            }
            if (!invalidPath)
            {
                // MessageBox.Show("The path is valid.");

                if (Directory.Exists(path))
                {
                    //MessageBox.Show("It's a directory.");
                    return pathType.valid_path;
                }
                else if (System.IO.File.Exists(path))
                {
                    if (path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                    {
                        //MessageBox.Show("It's a shortcut (link) file.");
                        return pathType.shortcut;
                    }
                    else
                    {
                        // MessageBox.Show("It's a regular file.");
                        return pathType.regular_File;
                    }
                }
                else
                {
                    //MessageBox.Show($"The path doesn't exist. {path}");
                    return pathType.invalidPath;
                }
            }
            else
            {
                // MessageBox.Show($"The path is not valid. {path}");
                return pathType.invalidPath;
            }
        }
        public enum pathType
        {
            regular_File,
            valid_path,
            invalidPath,
            shortcut
        }

        private void textBox_path_LostFocus(object sender, RoutedEventArgs e)
        {
            update_Link();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();


            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                textBox_path.Text = folderDialog.SelectedPath;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DereferenceLinks = false;
            openFileDialog.Filter = "Shortcut (*.Ink)|*.Ink|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                textBox_path.Text = openFileDialog.FileName;
        }

        private void textBox_Link_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            update_Link();
        }
        void update_Link()
        {
            string LinkPathName = textBox_path.Text;
            LinkPathName=RemoveParentheses(LinkPathName);
            pathType pathType = CheckPath(LinkPathName);
            if (pathType != pathType.shortcut)
            {
                return;
            }
            WshShell shell = new WshShell(); //https://learn.microsoft.com/en-us/troubleshoot/windows-client/admin-development/create-desktop-shortcut-with-wsh
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(LinkPathName);
            try { textBox_Link.Text = link.TargetPath; }
            catch { }

        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //update_Link();
        }

        public static readonly DependencyProperty LinkPathNameProperty = DependencyProperty.Register(nameof(LinkPathName), typeof(string), typeof(MainWindow), new PropertyMetadata("\"D:\\100_Nextcloud\\400_Programmierung\\LinkRenameTool\\LinkRenameTool\\Link.lnk\"", OnMyPropertyChanged));

        public string LinkPathName
        {
            get => (string)GetValue(LinkPathNameProperty);
            set => SetValue(LinkPathNameProperty, value);
        }

        private static void OnMyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindow control = d as MainWindow;
            if (control != null)
            {
                string newValue = (string)e.NewValue;
                string oldValue = (string)e.OldValue;

                // Your logic here
                control.HandlePropertyChanged(newValue, oldValue);
            }
        }

        private void HandlePropertyChanged(string newValue, string oldValue)
        {
            // Your custom logic here
            update_Link();
            
            
        }

    }
}
