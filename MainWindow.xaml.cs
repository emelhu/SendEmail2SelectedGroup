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

        private void SettingSelectProfil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedName = (sender as ComboBox)?.SelectedItem as string;                                                                              // string text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

            Debug.Assert(selectedName != null);
            Debug.Assert(profilViewModel.profils.last == selectedName);

            profilViewModel.profil = Profil.LoadFromXML(selectedName);
        }

        private void SettingNewProfil_Click(object sender, RoutedEventArgs e)
        {
            //

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

        private void SettingFindXlsx_Click(object sender, RoutedEventArgs e)
        {
            //TODO

            profilViewModel.profil.dataFile = "betöltött";

            profilViewModel.Refresh();
        }

        private void SettingCreateSampleXlsx_Click(object sender, RoutedEventArgs e)
        {
            //TODO

            profilViewModel.profil.dataFile = "létrehozott";

            profilViewModel.Refresh();
        }

        private void ProfilPrevNextButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void ProfilExitButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }       
    }
}
