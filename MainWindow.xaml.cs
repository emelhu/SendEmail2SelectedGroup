#nullable enable   

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        MainWindowViewModel viewModel;        
        //Image               envelopeImage = new Image("pack://application:,,,/SendEmail2SelectedGroup;component/Images/email.png");        

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
            viewModel = new MainWindowViewModel(profilNames);
            
            InitializeComponent();

            viewModel.modified = false; 
        }

        private void MainGrid_Initialized(object sender, EventArgs e)
        {
               
        }

        private async void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainGrid.DataContext = viewModel;       

            await Task.Delay(1000);
            await LoadXlsxHelper(viewModel.dataFile);
 
            SetTabitemsEnabled();

            viewModel.modified = false; 
        }

        #region events   

        #region profil
        private void SettingNewProfil_Click(object sender, RoutedEventArgs e)
        {
            var window = new NewProfilWindow(viewModel.profils.names);
            var result = window.ShowDialog();

            if (result ?? false)
            {
                //profilViewModel.profil = Profil.LoadFromXML(window.newName);
                viewModel.profilNames.Add(window.newName);
                viewModel.profilNameLast = window.newName;
                viewModel.modified       = true;
                viewModel.Refresh();

                MessageBox.Show($"A kért új '{window.newName}' profil hozzáadva és kijelölve.", "Csoportos levélküldő");
            }
            else
            {
                MessageBox.Show("Ön nem adott hozzá új profilt.", "Csoportos levélküldő");
            }
        }

        private void SettingSaveProfil_Click(object sender, RoutedEventArgs e)
        {
            viewModel.profil.SaveAsXML(); 
            viewModel.profils.SaveAsXML();
            viewModel.modified = false;
        }
        #endregion

        #region Datafile
        private const string DataFileExtCsv     = ".csv";
        private const string DataFileExtXlsx    = ".xlsx";
        private const string DataFileExtFilter  = "Excel (.xlsx)|*.xlsx|Text (.csv)|*.csv";

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.dataFileStatusText  = "Az adatállomány nem beolvasott!";
            viewModel.emailData           = null;
            viewModel.emailRawData        = null;   
            viewModel.modified            = true;
            e.Handled = false;

            SetTabitemsEnabled();
        }

        private async void SettingLoadXlsx_Click(object sender, RoutedEventArgs e)
        {
            await LoadXlsxHelper(viewModel.dataFile);                                                                                                                    
            
            SetTabitemsEnabled();
        }

        private async Task LoadXlsxHelper(string filename)
        {
            var load = Task.Run(() =>
            {
                viewModel.emailData       = null;
                viewModel.emailRawData    = null;    

                try
                {
                    viewModel.TrimDataFileName();

                    if (File.Exists(viewModel.dataFile))
                    {
                        var newData = EmailDataManager.Get(viewModel.dataFile);

                        viewModel.emailData       = newData;
                        viewModel.emailRawData    = newData;    

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

            var result = await load;                                                                                                                                // var results = await Task.WhenAll(load, firstLongTask, secondLongTask, thirdLongTask);

            viewModel.dataFileStatusText = result ?? ""; 
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
                viewModel.profil.dataFile = dlg.FileName;

                var load = Task.Run(() =>
                {
                    SettingLoadXlsx_Click(SettingFindXlsx, new RoutedEventArgs());
                });

                await load;

                viewModel.Refresh();
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
                if (File.Exists(dlg.FileName))
                {
                    File.Copy(dlg.FileName, $"{dlg.FileName}~{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.save");
                    File.Delete(dlg.FileName);
                }

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

                    viewModel.dataFileStatusText = "Új üres állomány létrehozva.";
                    viewModel.profil.dataFile    = dlg.FileName;                                                                                      // trigger content read
                }
                catch (Exception e2)
                {
                    viewModel.dataFileStatusText = "CREATE ERROR! " + e2.Message;
                }
                  
                viewModel.Refresh();
            }  
        }
        #endregion

        #region close window

        private void ProfilExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (CloseEnabled())
            {
                Application.Current.Shutdown();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = ! CloseEnabled();
        }

        private bool    lastCloseEnabledResult = false; 
        private long    lastCloseEnabledCall   = 0; 

        private bool CloseEnabled()
        {
            if ((Environment.TickCount64 - lastCloseEnabledCall) < 100)
            {   // less then 0.1s after last call
                return lastCloseEnabledResult;                                                                                                          // Patkolás, de ez van.
            }

            string warning = viewModel.modified ? "A profil adatai MÓDOSULTAK," + Environment.NewLine + "ha mentés nélkül lép ki, akkor ELVESZNEK!" + Environment.NewLine + Environment.NewLine : "";
            string msgtext = $"{warning}Valóban kilép az alkalmazásból?"; 
            string caption = "Kilépés az alkalmazásból"; 

            MessageBoxResult result = MessageBox.Show(msgtext, caption, MessageBoxButton.YesNo); 
			
            lastCloseEnabledCall = Environment.TickCount64;

            return (result == MessageBoxResult.Yes);
        } 

        #endregion

        #region Tabs       
        
        private void ProfilPrevNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == profilNextButton)
            {
                if (viewModel.emailData == null)
                {
                    tabItemBeallitasok.IsSelected = true;
                }
                else if (mainTabControl.SelectedIndex < (mainTabControl.Items.Count - 1))
                {
                    mainTabControl.SelectedIndex++;                    
                }
            }
            else
            {
                if (mainTabControl.SelectedIndex > 0)
                {
                    mainTabControl.SelectedIndex--;
                }
            }

            viewModel.Refresh();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (e.Source is TabControl)                                                                                                                             //if this event fired from TabControl then enter
            {
                if (viewModel.emailData == null)
                {
                    tabItemBeallitasok.IsSelected = true;
                }

                if (tabItemBeallitasok.IsSelected)
                {
                    //TODO *************************************************************************************************************************************************************************
                }
                else if (tabItemRawDataView.IsSelected)
                {
                    //TODO *************************************************************************************************************************************************************************
                }
                else if (tabItemSelectTarget.IsSelected)
                {
                    viewModel.selectedGroupName = string.Empty;                                                                                             // clear for view all rows because EmailRawData is common for both datagrid                                                          
                    viewModel.FillEmailRawData();
                }
                else if (tabItemEditMail.IsSelected)
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

        private void SetTabitemsEnabled()
        {
            for (int loop = 0; loop < mainTabControl.Items.Count; loop++)
            {
                var tab = mainTabControl.Items[loop] as TabItem;

                if (tab != null)
                {
                    var enable = true;

                    if (viewModel.emailData == null)
                    {
                        if (loop > 0)
                        {
                            enable = false;
                        }
                        else
                        {
                            //TODO
                        }
                    }
                                
                    tab.IsEnabled = enable;
                }
            }
        }

        private void SettingBottom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.Refresh();
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

        #region RawDataView filter & sort
       
        private void BtnRawOrderClear_Click(object sender, RoutedEventArgs e)
        {
            viewModel.selectedOrderName = string.Empty;                                                                               // trigger in viewmodel
        }

        private void BtnRawFilterClear_Click(object sender, RoutedEventArgs e)
        {
            viewModel.selectedGroupName  = string.Empty;                                                                              // trigger in viewmodel
        }

        #endregion

        #region Selection DataView  

        private void BtnSelectFilterAddRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnSelectFilterClear)
            {
                viewModel.selectedAddRemoveGroups = string.Empty;

                if (viewModel.emailData != null)
                {
                    foreach (var item in viewModel.emailData)
                    {
                        item.groupSelected = false;
                    }
                }

                return;
            }

            //

            if (! string.IsNullOrWhiteSpace(viewModel.selectedGroupName))
            { 
                bool add    = (sender == btnSelectFilterAdd);
                var  lists  = GetSelectedAddRemoveGroupsList();

                lists.removeList.Remove(viewModel.selectedGroupName);
                lists.addList.Remove(viewModel.selectedGroupName);

                if (add)
                {
                    lists.addList.Add(viewModel.selectedGroupName);
                }
                else
                {
                    lists.removeList.Add(viewModel.selectedGroupName);
                }

                SetSelectedAddRemoveGroupsList(lists.addList, lists.removeList);
                 

                if (viewModel.emailData != null)                                                                                            // Set new groupSelected state
                {
                    foreach (var email in viewModel.emailData)
                    {
                        bool newSelected = false;

                        foreach (var name in lists.addList)
                        {
                            if (email.IsInGroup(name))
                            {
                                newSelected = true;
                                break;
                            }
                        }

                        foreach (var name in lists.removeList)
                        {
                            if (email.IsInGroup(name))
                            {
                                newSelected = false;
                                break;
                            }
                        }

                        email.groupSelected = newSelected;
                    }
                }


                viewModel.selectedGroupName  = string.Empty;                                                                                // Clear view filter  
            }
        }

        private (List<string> addList, List<string> removeList) GetSelectedAddRemoveGroupsList()
        {
            var addList    = new List<string>();
            var removeList = new List<string>();

            if (! string.IsNullOrWhiteSpace(viewModel.selectedAddRemoveGroups))
            {
                var listItems = viewModel.selectedAddRemoveGroups.Split(' ');

                foreach (var listItem in listItems)
                {
                    if (! string.IsNullOrWhiteSpace(listItem))
                    {                                           
                        if (listItem[0] == '+')
                        {
                            addList.Add(listItem[1..]);
                        }
                        else if (listItem[0] == '-')
                        {
                            removeList.Add(listItem[1..]);
                        }
                        else
                        {
                            throw new ArgumentException("Internal error! GetSelectedAddRemoveGroupsList(): none '+', '-' !");
                        }
                    }
                }
            }

            return (addList, removeList);        
        }

        private void SetSelectedAddRemoveGroupsList(List<string> addList, List<string> removeList)
        {
            addList.Sort();
            removeList.Sort();

            var sb = new StringBuilder(1000);

            foreach (var addItem in addList)
            {
                sb.Append('+');
                sb.Append(addItem);
                sb.Append(' ');
            }

            foreach (var removeItem in removeList)
            {
                sb.Append('-');
                sb.Append(removeItem);
                sb.Append(' ');
            }

            viewModel.selectedAddRemoveGroups = sb.ToString();
        }
        #endregion

        
    }
}

#region WPF component links
/*
https://stackoverflow.com/questions/2853276/wpf-list-of-viewmodels-bound-to-list-of-model-objects           (ObservableCollection<Item>  --> ObservableCollection<ItemViewModel>)

https://www.nuget.org/packages/Smith.WPF.HtmlEditor/
https://github.com/emelhu/SmithHtmlEditor


*/
#endregion