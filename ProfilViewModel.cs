#nullable enable   

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SendEmail2SelectedGroup
{
    public class ProfilViewModel : INotifyPropertyChanged 
    {
        #region WPF GUI data
        #region load/save data
        private Profil          _profil             { get; set; }
        private ProfilNames     _profils            { get; set; }

        public  Profil          profil              { get { return _profil; }                       set { _profil               = value; Refresh(); } } 
        public  ProfilNames     profils             { get { return _profils; }                      set { _profils              = value; Refresh(); } }

        #region Profil fields
        public  string          name                { get { return _profil.name; }                  set { _profil.name          = value; OnPropertyChanged(); } } 
        public  string          dataFile            { get { return _profil.dataFile; }              set { _profil.dataFile      = value; OnPropertyChanged(); } }
        #endregion

        #region ProfilNames fields
        public  List<string>    profilNames         => _profils.names;  
        public  string          profilNameLast      { get { return _profils.last; }                 set { _profils.last         = value; OnPropertyChanged(); } }
        #endregion

        #endregion

        #region only state
        private bool            _modified;
        public  bool            modified            { get { return _modified; }                   set { _modified               = value; OnPropertyChanged(); OnPropertyChanged(nameof(exitEnable)); } }                               

        public bool             exitEnable          => ! modified;
        public bool             prevEnable          => true;
        public bool             nextEnable          => true;
        #endregion
        #endregion

        public ProfilViewModel()                                                                                                // for design times
        {
            _profils = new ProfilNames();
            _profil  = new Profil();
        }

        public ProfilViewModel(ProfilNames profils)
        {
            _profils = profils;
            _profil  = new Profil(profils.last);
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

            if (!modified)
            {
                modified = true;
            }
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