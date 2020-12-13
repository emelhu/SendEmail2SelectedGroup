﻿#nullable enable   

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

namespace SendEmail2SelectedGroup
{
    public class ProfilViewModel : INotifyPropertyChanged 
    {
        #region WPF GUI data
        #region load/save data
        private Profil          _profil             { get; set; }
        private ProfilNames     _profils            { get; set; }

        public  Profil          profil              { get { return _profil; }                    set { _profil          = value; Refresh(); } } 
        public  ProfilNames     profils             { get { return _profils; }                   set { _profils         = value; Refresh(); } }

        #region Profil fields
        public  string          name                { get { return _profil.name; }               set { _profil.name     = value; OnPropertyChanged(); } } 
        public  string          dataFile            { get { return _profil.dataFile; }           set { _profil.dataFile = value; OnPropertyChanged(); OnPropertyChanged(nameof(dataFileStatusText)); } }
        #endregion

        #region ProfilNames fields
        public  List<string>    profilNames         => _profils.names;  
        public  string          profilNameLast      { get { return _profils.last; }              set { _profils.last    = value; OnPropertyChanged(); } }
        #endregion

        private ObservableCollection<EmailData>?    _emailData;
        public  ObservableCollection<EmailData>?    emailData { get { return _emailData; }       set { _emailData       = value; SetAllGroupNames(); Refresh(); } }      

        private ObservableCollection<EmailData>?    _emailRawData;
        public  ObservableCollection<EmailData>?    emailRawData { get { return _emailRawData; } set { _emailRawData    = value; Refresh(); } } 

        #endregion

        #region only state
        private bool           _modified;
        public  bool            modified            { get { return _modified; }                     set { _modified             = value; Refresh(); } }                               

        //public bool             exitEnable          => ! modified;                                                                                        // Solved by "Converter={StaticResource boolNegation}"
        public  bool            prevEnable          => true;
        public  bool            nextEnable          => (emailData != null);

        private string          _dataFileStatusText = "az adatfile státusa";
        public  string          dataFileStatusText  { get { return _dataFileStatusText; }           set { _dataFileStatusText   = value; OnPropertyChanged(); } }       
        
        private string[]        saveNeedFields      = new string[] { nameof(name), nameof(dataFile), nameof(profilNameLast) };                              // Need save XML files

        #endregion

        public string[] emailDataOrderNames { get; }        = new string[] { String.Empty, "id", "name1", "name2", "name3", "email" };
        public string[] allGroupNames       { get; set; }   = new string[0];


        private string  _selectedOrderName   = string.Empty;
        private string  _selectedGroupName   = string.Empty;
        public  string  selectedOrderName    { get { return _selectedOrderName; }  set { _selectedOrderName    = value; FillEmailRawData(); OnPropertyChanged(); } }        
        public  string  selectedGroupName    { get { return _selectedGroupName; }  set { _selectedGroupName    = value; FillEmailRawData(); OnPropertyChanged(); } }
        #endregion

        public ProfilViewModel()                                                                                                // for design times
        {
            _profils        = new ProfilNames();
            _profil         = new Profil();
            _emailData      = null;
            _emailRawData   = null;
        }

        public ProfilViewModel(ProfilNames profils)
        {
            _profils        = profils;
            _profil         = Profil.LoadFromXML(profils.last);
            _emailData      = GetEmailData(_profil.dataFile);
            _emailRawData   = _emailData;
        }

        private static ObservableCollection<EmailData> GetEmailData(string dataFile)
        {
            var list = new List<EmailData>();

            //TODO **************************************************************************************************************************************************************************

            return new ObservableCollection<EmailData>(list);
        }

        public void TrimDataFileName()
        {
            _profil.dataFile = _profil.dataFile.Trim();
        }

        private void SetAllGroupNames()
        {
            var groups = new List<string>();

            if (emailData != null)
            {
                foreach (var data in emailData)
                {
                    groups.AddRange(data.groups);
                }
            }

            groups.Add(String.Empty);

            allGroupNames = groups.Distinct().ToArray();
        }

        private void FillEmailRawData()
        {
            bool order  = ! string.IsNullOrWhiteSpace(selectedOrderName);
            bool filter = ! string.IsNullOrWhiteSpace(selectedGroupName);

            if ((! order && ! filter) || (emailData == null))
            {
                emailRawData = emailData;
                return;
            }

            List<EmailData> data;

            if (filter)
            {
                data = emailData.Where(ed => ed.IsInGroup(selectedGroupName)).ToList();
            }
            else
            {
                data = new List<EmailData>(emailData);
            }

            if (order)
            {   // "id", "name1", "name2", "name3", "email"
                if (selectedOrderName == "id")
                {
                    data = data.OrderBy(ed => (ed.id ?? string.Empty)).ToList();
                }
                else if (selectedOrderName == "name1")
                {
                    data = data.OrderBy(ed => (ed.name1 ?? string.Empty)).ToList();
                }
                else if (selectedOrderName == "name2")
                {
                    data = data.OrderBy(ed => (ed.name2 ?? string.Empty)).ToList();
                }
                else if (selectedOrderName == "name3")
                {
                    data = data.OrderBy(ed => (ed.name3 ?? string.Empty)).ToList();
                }
                else if (selectedOrderName == "email")
                {
                    data = data.OrderBy(ed => (ed.email ?? string.Empty)).ToList();
                }
            }

            emailRawData = new ObservableCollection<EmailData>(data);
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

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                if (propertyName == nameof(profilNameLast))
                {
                    _profil = Profil.LoadFromXML(profilNameLast);
                }

                if ((propertyName != nameof(modified)) && saveNeedFields.Contains(propertyName))
                {
                    modified = true;
                }
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