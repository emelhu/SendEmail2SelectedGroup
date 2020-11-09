﻿#nullable enable   

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

        SettingViewModel    setting;        
        #endregion

        static MainWindow()
        {
            appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
            Directory.CreateDirectory(appDir);
        }

        public MainWindow()
        {
            InitializeComponent();

            setting = new SettingViewModel();

            setting.profils = ProfilNames.LoadFromXML(appDir);
            setting.profil  = Profil.LoadFromXML(appDir, setting.profils.last);

            DataContext = setting;
        }

        private void SettingSelectProfil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedName = (sender as ComboBox)?.SelectedItem as string;                                                                              // string text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

            Debug.Assert(selectedName != null);

            if ((selectedName != null) && (selectedName != setting.profils.last))
            {
                setting.profil       = Profil.LoadFromXML(appDir, selectedName);
                setting.profils.last = selectedName;

                setting.Refresh();
            }
        }

        private void SettingNewProfil_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Jelenleg nem adható hozzá új profil.", "Csoportos levélküldő");
        }

        private void SettingSaveProfil_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void SettingFindXlsx_Click(object sender, RoutedEventArgs e)
        {
            //TODO

            setting.profil.xmlfile = "betöltött";

            setting.Refresh();
        }

        private void SettingCreateSampleXlsx_Click(object sender, RoutedEventArgs e)
        {
            //TODO

            setting.profil.xmlfile = "létrehozott";

            setting.Refresh();
        }

        private void ProfilNextButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void ProfilExitButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}