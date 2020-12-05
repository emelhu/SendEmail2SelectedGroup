#nullable enable   

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public  string          dataFile            { get { return _profil.dataFile; }              set { _profil.dataFile      = value; OnPropertyChanged(); OnPropertyChanged(nameof(dataFileStatusText)); } }
        #endregion

        #region ProfilNames fields
        public  List<string>    profilNames         => _profils.names;  
        public  string          profilNameLast      { get { return _profils.last; }                 set { _profils.last         = value; OnPropertyChanged(); } }
        #endregion

        private ObservableCollection<EmailData>?    _emailData;
        public  ObservableCollection<EmailData>?    emailData { get { return _emailData; }              set { _emailData            = value; OnPropertyChanged(); } } 

        #endregion

        #region only state
        private bool            _modified;
        public  bool            modified            { get { return _modified; }                     set { _modified             = value; OnPropertyChanged(); /*OnPropertyChanged(nameof(exitEnable)); */ } }                               

        //public bool           exitEnable          => ! modified;
        public  bool            prevEnable          => true;
        public  bool            nextEnable          => true;

        private string          _dataFileStatusText  = "az adatfile státusa";
        public  string          dataFileStatusText  { get { return _dataFileStatusText; }           set { _dataFileStatusText   = value; OnPropertyChanged(); } }
        #endregion
        #endregion

        public ProfilViewModel()                                                                                                // for design times
        {
            _profils    = new ProfilNames();
            _profil     = new Profil();
            _emailData  = null;
        }

        public ProfilViewModel(ProfilNames profils)
        {
            _profils    = profils;
            _profil     = Profil.LoadFromXML(profils.last);
            _emailData  = GetEmailData(_profil.dataFile);
        }

        private static ObservableCollection<EmailData> GetEmailData(string dataFile)
        {
            var list = new List<EmailData>();

            //TODO **************************************************************************************************************************************************************************

            return new ObservableCollection<EmailData>(list);
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

            if (propertyName == nameof(profilNameLast))
            {
                _profil  = Profil.LoadFromXML(profilNameLast);
            }

            if ((propertyName != nameof(modified)) /*(propertyName != nameof(exitEnable)) &&*/)
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