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
            string path =textBox_path.Text;
            string oldText =textBox_searchFor.Text;
            string newText=textBox_searchFor.Text;
            bool r = (bool) checkBox_rekursiv.IsChecked;
            execute(path, oldText, newText, r);
        }


        void execute(string LinkPathName = @"D:\100_Nextcloud\400_Programmierung\LinkRenameTool\LinkRenameTool\Link.lnk", string oldText = @"C:\Users\Pascal\Documents\Buchungsformular_schnabel.pdf", string newText = "", bool searchSubdirectory = false)
        {
            pathType check = CheckPath(LinkPathName);
            if (check == pathType.invalidPath)
            {
                MessageBox.Show("invalid Path");
                return;
            }
            if (check == pathType.valid_path)
            {
                List<string> paths = FindLnkFiles(LinkPathName, searchSubdirectory);
                foreach (string s in paths)
                {
                    changeLink(s, oldText, newText);
                }

            }
            if (check == pathType.shortcut)
            {
                changeLink(LinkPathName, oldText, newText);
            }

        }


        void changeLink(string LinkPathName, string oldText = ".pdf", string newText = ".txt", bool showMessage = false)
        {
            WshShell shell = new WshShell(); //https://learn.microsoft.com/en-us/troubleshoot/windows-client/admin-development/create-desktop-shortcut-with-wsh
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(LinkPathName);
            string oldPath = link.TargetPath;
            string newPath = oldPath.Replace(oldText, newText);
            link.TargetPath = newPath;
            link.Description = "new";
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



        public pathType CheckPath(string path)
        {
            if (System.IO.Path.IsPathRooted(path))
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
                   // MessageBox.Show("The path doesn't exist.");
                    return pathType.invalidPath;
                }
            }
            else
            {
             //   MessageBox.Show("The path is not valid.");
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

    }
}
