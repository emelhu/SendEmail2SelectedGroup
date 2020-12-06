#nullable enable   

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using EPPlus.SimpleTable;
//using System.Windows.Shapes;

namespace SendEmail2SelectedGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region common variables
        public const  string appName = "SendEmail2SelectedGroup";
        public static string appDir;

        ProfilNames         profilNames;
        ProfilViewModel     profilViewModel;        
        #endregion

        static MainWindow()
        {
            appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
            Directory.CreateDirectory(appDir);
        }

        public MainWindow()
        {
            Profil.xmlDirectory = appDir;
            
            profilNames = ProfilNames.LoadFromXML();
            profilViewModel = new ProfilViewModel(profilNames);
            
            InitializeComponent();

            // DataContext = setting;           --> MainGrid_Initialized
        }

        private void MainGrid_Initialized(object sender, EventArgs e)
        {
            MainGrid.DataContext = profilViewModel;
        }

        #region events   

        #region profil
        private void SettingNewProfil_Click(object sender, RoutedEventArgs e)
        {
            var window = new NewProfilWindow(profilViewModel.profils.names);
            var result = window.ShowDialog();

            if (result ?? false)
            {
                //profilViewModel.profil = Profil.LoadFromXML(window.newName);
                profilViewModel.profilNames.Add(window.newName);
                profilViewModel.profilNameLast = window.newName;
                profilViewModel.Refresh();

                MessageBox.Show($"A kért új '{window.newName}' profil hozzáadva és kijelölve.", "Csoportos levélküldő");
            }
            else
            {
                MessageBox.Show("Ön nem adott hozzá új profilt.", "Csoportos levélküldő");
            }
        }

        private void SettingSaveProfil_Click(object sender, RoutedEventArgs e)
        {
            profilViewModel.profil.SaveAsXML(); 
            profilViewModel.profils.SaveAsXML();
            profilViewModel.modified = false;
        }
        #endregion

        #region Datafile
        private const string DataFileExtCsv    = ".csv";
        private const string DataFileExtXlsx   = ".xlsx";
        private const string DataFileExtFilter = "Excel (.xlsx)|*.xlsx|Text (.csv)|*.csv";

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            profilViewModel.dataFileStatusText = "Az adatállomány nem beolvasott!";
            profilViewModel.emailData          = null;
        }

        private async void SettingLoadXlsx_Click(object sender, RoutedEventArgs e)
        {
            var load = Task.Run(() =>
            {
                profilViewModel.emailData = null;

                try
                {
                    profilViewModel.TrimDataFileName();

                    if (File.Exists(profilViewModel.dataFile))
                    {
                        var newData = EmailDataManager.Get(profilViewModel.dataFile);

                        profilViewModel.emailData = newData;

                        return $"Beolvasva {newData.Count()} tételsor.";
                    }
                    else
                    {
                        return "Az állomány nem létezik, nem beolvasható!";
                    }
                }
                catch (Exception e)
                {
                    return "ERROR! " + e.Message;                                                                                                                   // return error text
                }
            });

            var result = await load;                                                                                                                    // var results = await Task.WhenAll(load, firstLongTask, secondLongTask, thirdLongTask);

            profilViewModel.dataFileStatusText = result ?? ""; 
        }

        private async void SettingFindXlsx_Click(object sender, RoutedEventArgs e)
        {
            var dlg              = new Microsoft.Win32.OpenFileDialog();                                                                                // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/app-development/dialog-boxes-overview?view=netframeworkdesktop-4.8&viewFallbackFrom=netdesktop-5.0
            dlg.FileName         = "*.*";  
            dlg.CheckPathExists  = true;
            dlg.InitialDirectory = appDir;
            dlg.DefaultExt       = DataFileExtXlsx;  
            dlg.Filter           = DataFileExtFilter;  

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                profilViewModel.profil.dataFile = dlg.FileName;

                var load = Task.Run(() =>
                {
                    SettingLoadXlsx_Click(SettingFindXlsx, new RoutedEventArgs());
                });

                await load;

                profilViewModel.Refresh();
            }  
        }

        private void SettingCreateSampleXlsx_Click(object sender, RoutedEventArgs e)
        {   // SettingCreateSampleXlsx button OR SettingCreateSampleCsv button clicked
            var isCSV       = ((sender as Button) == SettingCreateSampleCsv);

            var dlg         = new Microsoft.Win32.SaveFileDialog();                                                                                     // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/app-development/dialog-boxes-overview?view=netframeworkdesktop-4.8&viewFallbackFrom=netdesktop-5.0
            dlg.FileName    = $"{appName}_Sample";  
            dlg.CheckPathExists  = true;
            dlg.InitialDirectory = appDir;
            dlg.DefaultExt  = (isCSV ? DataFileExtCsv : DataFileExtXlsx);  
            dlg.Filter      = DataFileExtFilter;  

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                profilViewModel.profil.dataFile = dlg.FileName;

                if (File.Exists(dlg.FileName))
                {
                    profilViewModel.dataFileStatusText = "Az állomány már létezik.";
                }
                else
                {
                    try
                    {
                        if (isCSV)
                        {
                            var    columns = Enum.GetNames(typeof(DatafileColumnDefinition));
                            string header  = string.Join(',', columns);

                            File.WriteAllText(dlg.FileName, header);
                        }
                        else
                        {
                            using (var excelTable = new SimpleExcelTable<DatafileColumnDefinition>(dlg.FileName, null))
                            {

                            }
                        }

                        profilViewModel.dataFileStatusText = "Új üres állomány létrehozva.";
                    }
                    catch (Exception e2)
                    {
                        profilViewModel.dataFileStatusText = "CREATE ERROR! " + e2.Message;
                    }
                }

                profilViewModel.Refresh();
            }  
        }
        #endregion

        #region Tabs
        private void ProfilPrevNextButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void ProfilExitButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (e.Source is TabControl)                                                                                                                             //if this event fired from TabControl then enter
            {
                if (tabItemBeallitasok.IsSelected)
                {
                    //TODO *************************************************************************************************************************************************************************
                }
                else if (tabItemDataView.IsSelected)
                {
                    //TODO *************************************************************************************************************************************************************************
                }
                else if (tabItemSelectTarget.IsSelected)
                {
                    //TODO *************************************************************************************************************************************************************************
                }
                else if (tabItemEditBody.IsSelected)
                {
                    //TODO *************************************************************************************************************************************************************************
                }
                else if (tabItemSendEmails.IsSelected)
                {
                    //TODO *************************************************************************************************************************************************************************
                }
                else 
                {
                    throw new Exception("TabControl_SelectionChanged is fired and don't managed TabItem found!");
                }
                

                e.Handled = true;
            }
        }

        private void SettingBottom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            profilViewModel.Refresh();
        }

        #endregion

        #endregion

        #region Data Binding

        //[ValueConversion(typeof(bool), typeof(bool))]
        //public class InverseBooleanConverter: IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter,
        //        System.Globalization.CultureInfo culture)
        //    {
        //        if (targetType != typeof(bool))
        //            throw new InvalidOperationException("The target must be a boolean");

        //        return !(bool)value;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter,
        //        System.Globalization.CultureInfo culture)
        //    {
        //        throw new NotSupportedException();
        //    }
        //}

        #endregion

        
    }
}

#region WPF component links
/*
https://stackoverflow.com/questions/2853276/wpf-list-of-viewmodels-bound-to-list-of-model-objects           (ObservableCollection<Item>  --> ObservableCollection<ItemViewModel>)
*/
#endregion