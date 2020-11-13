#nullable enable   

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

namespace SendEmail2SelectedGroup
{
    /// <summary>
    /// Interaction logic for NewProfilWindow.xaml
    /// </summary>
    public partial class NewProfilWindow : Window, INotifyPropertyChanged
    {
        #region GUI interface data

        private string                          _newName         = string.Empty;
        private ObservableCollection<string>    _existNames;
        public  string                          newName                 { get { return _newName; }          set { _newName    = value; OnPropertyChanged(); } } 
        public  ObservableCollection<string>    existNames              { get { return _existNames; }       set { _existNames = value; OnPropertyChanged(); } } 

        #region only state
        private string?                         _newNameErrorText = null;
        public  string?                         newNameErrorText        { get { return _newNameErrorText; } set { _newNameErrorText = value; Refresh(); } }             // OnPropertyChanged for newNameError & newNameErrorVisibility too
        public  bool                            newNameError            => ! string.IsNullOrWhiteSpace(newNameErrorText);
        public  Visibility                      newNameErrorVisibility  => newNameError ? Visibility.Visible : Visibility.Collapsed;

        public bool                             saveButtonEnabled       => ! newNameError;
        #endregion
        #endregion

        public NewProfilWindow(IEnumerable<string> existNames)  
        {
            InitializeComponent();

            this._existNames = new ObservableCollection<string>(existNames);

            DataContext = this;
        }       

        private void ProfilName_TextChanged(object sender, RoutedEventArgs e)
        {
            CheckProfilNewName();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckProfilNewName())
            {
                this.DialogResult = true;
            }           
        }

        private bool CheckProfilNewName()
        {
            newNameErrorText = null;

            if (String.IsNullOrWhiteSpace(newName) || newName.Trim().Contains(' '))
            {
                newNameErrorText = "Az új profil neve nem lehet üres ill. ne tartalmazzon szóközt.";
                newName          = newName.Trim();
                Refresh();
            }
            else
            {
                char[] invalidFileChars = Path.GetInvalidFileNameChars();

                foreach (char chr in newName)
                {
                    if (invalidFileChars.Contains(chr))
                    {
                        newNameErrorText = $"Az új profil neve meg kell feleljen a filenevek előírásának, ezért nem tartalmathatja a '{chr}' karaktert!";
                        Refresh();
                        break;
                    }
                }
            }

            if (! newNameError)                                                                                 // 'newNameError' was defined as lambda expression with check 'newNameErrorText'
            {
                var caseInsensitiveComparer = new CaseInsensitiveComparer();

                foreach (var name in existNames)
                {
                    if (String.Equals(name, newName, StringComparison.CurrentCultureIgnoreCase))
                    { 
                        newNameErrorText = $"Az új profil neve nem lehet azonos (vagy összetéveszthető) a már létező '{name}' profil névvel!";
                        Refresh();
                    }
                }                
            }

            return ! newNameError;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Refresh()
        {
            OnPropertyChanged(string.Empty);
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
