using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.Spreadsheet;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExcelRunner.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainView : RibbonWindow
    {
        public Ribbon sfribbon;

        public bool IsOperating { get; private set; }

        public MainView()
        {
            InitializeComponent();
        }

        private void ribbon_Loaded(object sender, RoutedEventArgs e)
        {
            sfribbon = GridUtil.GetVisualChild<Ribbon>(sender as SfSpreadsheetRibbon);
            var backstage = sfribbon.BackStage;
            foreach (var item in backstage.Items)
            {
                if (item is BackStageCommandButton)
                {
                    var button = item as BackStageCommandButton;
                    var header = button.Header;
                    if (button.Header.Contains("Open"))
                    {
                        button.Click += FileOpen;
                    }
                    else if (button.Header.Contains("Save As"))
                    {
                 
                        button.Click += SaveAs; ;
                    }
                    else if (button.Header.Contains("Save"))
                    {
                        button.Click += Save; ;
                    }
                }
            }
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            if (!IsOperating)
            {
                IsOperating = true;
                this.spreadsheet.Save();
                sfribbon.HideBackStage();
                IsOperating = false;
            }
        }

        public void SaveAs(object sender, RoutedEventArgs e)
        {
            if (!IsOperating)
            {
                IsOperating = true;
                this.spreadsheet.SaveAs();
                sfribbon.HideBackStage();
                IsOperating = false;
            }
        }

        public void FileOpen(object sender, RoutedEventArgs e)
        {
            if (!IsOperating)
            {
                IsOperating = true;
                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension
                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                // Display OpenFileDialog by calling ShowDialog method
                Nullable<bool> result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                    spreadsheet.Open(filename);
                }
                sfribbon.HideBackStage();
                IsOperating = false;
            }
        }

    }
}
