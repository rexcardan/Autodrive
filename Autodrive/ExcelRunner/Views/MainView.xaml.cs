using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.Spreadsheet;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ExcelRunner.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainView : RibbonWindow
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void ribbon_Loaded(object sender, RoutedEventArgs e)
        {
            var sfribbon = GridUtil.GetVisualChild<Ribbon>(sender as SfSpreadsheetRibbon);
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
            if (sfribbon != null)
            {
                var items = sfribbon.Items;

                RibbonTab rb = new RibbonTab();
                rb.Caption = "OTHER";
                RibbonButton Button1 = new RibbonButton();
                Button1.Label = "PRINT";
                //Button1.SmallIcon = new BitmapImage(new Uri("/../Icon/Icons_Print.png", UriKind.Relative));
                //Button1.Click += Button1_Click;
                //RibbonButton Button2 = new RibbonButton();
                //Button2.Label = "PRINT PREVIEW";
                //Button2.SmallIcon = new BitmapImage(new Uri("/../Icon/Icons_Print.png", UriKind.Relative));
                //Button2.Click += Button2_Click;
                //var customRibbonBar = new RibbonBar();
                //customRibbonBar.Header = "Printing Options";
                //customRibbonBar.Items.Add(Button1);
                //customRibbonBar.Items.Add(Button2);
                //customRibbonBar.IsLauncherButtonVisible = false;
                //rb.Items.Add(customRibbonBar);
                //sfribbon.Items.Add(rb);
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            this.spreadsheet.Save();
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            this.spreadsheet.SaveAs();
        }

        private void FileOpen(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                this.spreadsheet.Open(filename);
            }

        }
    }
}
