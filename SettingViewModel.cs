#nullable enable   

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SendEmail2SelectedGroup
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        #region WPF GUI data
        #region load/save data
        public Profil       profil      { get; set; }
        public ProfilNames  profils     { get; set; }
        #endregion

        #region only state
        public bool         modified    { get; set; }

        public bool         exitEnable  => ! modified;
        public bool         prevEnable  => true;
        public bool         nextEnable  => true;
        #endregion
        #endregion

        public SettingViewModel()
        {
            profils = new ProfilNames();
            profil  = new Profil();
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

            modified = true;
        }
        #endregion
    }
}

#region sample code
/*
private int _progressValue;
public int ProgressValue
{
    get { return _progressValue; }
    set { _progressValue = value; OnPropertyChanged(); }
}
*/
#endregion