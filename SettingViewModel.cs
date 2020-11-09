#nullable enable   

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SendEmail2SelectedGroup
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        public Profil       profil      { get; set; }
        public ProfilNames  profils     { get; set; }

        public SettingViewModel()
        {
            profils = new ProfilNames();
            profil  = new Profil();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Refresh()
        {
            OnPropertyChanged(string.Empty);
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
