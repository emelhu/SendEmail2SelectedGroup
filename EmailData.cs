#nullable enable   

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

using EPPlus.SimpleTable;

using OfficeOpenXml;

namespace SendEmail2SelectedGroup
{
    public class EmailData : INotifyPropertyChanged
    {
        #region data
        public  string?     id      { get; set; }

        public  string?     name1   { get; set; }
        public  string?     name2   { get; set; }
        public  string?     name3   { get; set; }

        public  string?     email   { get; set; }
        public  string?     subject { get; set; }
        public  string?     body    { get; set; }

        public  string?     attach1 { get; set; }
        public  string?     attach2 { get; set; }
        public  string?     attach3 { get; set; }
        public  string?     attach4 { get; set; }
        public  string?     attach5 { get; set; }

        public  string?     group1  { get; set; }
        public  string?     group2  { get; set; }
        public  string?     group3  { get; set; }
        public  string?     group4  { get; set; }
        public  string?     group5  { get; set; }

        public  string?     data1   { get; set; }
        public  string?     data2   { get; set; }
        public  string?     data3   { get; set; }
        public  string?     data4   { get; set; }
        public  string?     data5   { get; set; }

        private string?[]?  _attachs;
        public  string?[]   attachs  => (_attachs ?? AttachsFill());

        private string[]?   _groups;
        public  string[]    groups   => (_groups  ?? GroupsFill());

        public  string      groupsText => string.Join(",", groups);

        private string?[]?  _datas;
        public  string?[]   datas    => (_datas   ?? DatasFill());

        private bool        _userSelected;  
        public  bool        userSelected     { get { return _userSelected; }    set { _userSelected = value;  OnPropertyChanged(); OnPropertyChanged(nameof(selected)); } } 

        private bool        _groupSelected;  
        public  bool        groupSelected    { get { return _groupSelected; }   set { _groupSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(selected)); } } 

        public  bool        selected         => userSelected || groupSelected;

        #endregion

        public EmailData()
        {
 
        }

        public readonly static char[] GroupSeparators = { ',', ' ', ';', '\t', '/', '|' };

        /// <summary>
        /// WARNING: Assumed you will read the 'groups' property only after you have filled group1,group2,group3,group4,group5 properties
        /// </summary>
        private string[] GroupsFill()
        {
            var t = group1 + "," + group2 + "," + group3 + "," + group4 + "," + group5;

            _groups = t.Split(GroupSeparators, StringSplitOptions.RemoveEmptyEntries).Distinct().OrderBy(s => s).ToArray();
            
            return _groups;
        }

        /// <summary>
        /// WARNING: Assumed you will read the 'attachs' property only after you have filled group1,group2,group3,group4,group5 properties
        /// </summary>
        private string?[] AttachsFill()
        {
            _attachs = new string?[5] { attach1,attach2,attach3,attach4,attach5 };
            
            return _attachs;
        }

        /// <summary>
        /// WARNING: Assumed you will read the 'datas' property only after you have filled group1,group2,group3,group4,group5 properties
        /// </summary>
        private string?[] DatasFill()                                                                                                                                        // OK, 'datas' is grammatically incorrect but uniform
        {
            _datas = new string?[5] { data1, data2, data3, data4, data5};
            
            return _datas;
        }

        public bool IsInGroup(string groupName)
        {
            return groups.Contains(groupName);
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

    public static class EmailDataManager
    {
        static public ObservableCollection<EmailData> Get(string filename)
        {
            string  ext     = Path.GetExtension(filename).ToUpperInvariant();

            if (ext == ".XLSX")
            {
                return GetXLSX(filename);
            }
            else if (ext == ".CSV")
            {
                return GetCSV(filename);
            }
            else
            {
                throw new Exception($"The extension of data's filename must be one of XLSX or CSV ! [{filename}]");
            }
        }

        static private ObservableCollection<EmailData> GetXLSX(string filename)
        {
            using (var excelTable = new SimpleExcelTable<DatafileColumnDefinition>(filename, null))
            {         
                excelTable.writeAtDispose = false;                                                                                                              // ReadOnly mode

                if ((excelTable.rowCount < 1) || (excelTable.colCount < 1))
                {
                    throw new Exception($"The content of datafile is invalid (empty) ! [{filename}]");
                }
                else
                {
                    var maxValue = Enum.GetValues(typeof(DatafileColumnDefinition)).Cast<int>().Max();

                    if (excelTable.colCount < maxValue)
                    {
                        throw new Exception($"The content of datafile is invalid (column count < {maxValue}) ! [{filename}]");
                    }

                    var ret = new ObservableCollection<EmailData>();

                    var startRow = excelTable.hasHeaderRow ? 2 : 1;                                                                                             // Skip header if any

                    for (int i = startRow; i < excelTable.rowCount; i++)
                    {
                        var rec = new EmailData();

                        rec.id           = (excelTable[i, DatafileColumnDefinition.id     ] ?? string.Empty).ToString();
                                           
                        rec.name1        = (excelTable[i, DatafileColumnDefinition.name1  ] ?? string.Empty).ToString();
                        rec.name2        = (excelTable[i, DatafileColumnDefinition.name2  ] ?? string.Empty).ToString();
                        rec.name3        = (excelTable[i, DatafileColumnDefinition.name3  ] ?? string.Empty).ToString();
                                           
                        rec.email        = (excelTable[i, DatafileColumnDefinition.email  ] ?? string.Empty).ToString();
                        rec.subject      = (excelTable[i, DatafileColumnDefinition.subject] ?? string.Empty).ToString();
                        rec.body         = (excelTable[i, DatafileColumnDefinition.body   ] ?? string.Empty).ToString();
                                           
                        rec.attach1      = (excelTable[i, DatafileColumnDefinition.attach1] ?? string.Empty).ToString();
                        rec.attach2      = (excelTable[i, DatafileColumnDefinition.attach2] ?? string.Empty).ToString();
                        rec.attach3      = (excelTable[i, DatafileColumnDefinition.attach3] ?? string.Empty).ToString();
                        rec.attach4      = (excelTable[i, DatafileColumnDefinition.attach4] ?? string.Empty).ToString();
                        rec.attach5      = (excelTable[i, DatafileColumnDefinition.attach5] ?? string.Empty).ToString();
                                           
                        rec.group1       = (excelTable[i, DatafileColumnDefinition.group1 ] ?? string.Empty).ToString();
                        rec.group2       = (excelTable[i, DatafileColumnDefinition.group2 ] ?? string.Empty).ToString();
                        rec.group3       = (excelTable[i, DatafileColumnDefinition.group3 ] ?? string.Empty).ToString();
                        rec.group4       = (excelTable[i, DatafileColumnDefinition.group4 ] ?? string.Empty).ToString();
                        rec.group5       = (excelTable[i, DatafileColumnDefinition.group5 ] ?? string.Empty).ToString();
                                           
                        rec.data1        = (excelTable[i, DatafileColumnDefinition.data1  ] ?? string.Empty).ToString();
                        rec.data2        = (excelTable[i, DatafileColumnDefinition.data2  ] ?? string.Empty).ToString();
                        rec.data3        = (excelTable[i, DatafileColumnDefinition.data3  ] ?? string.Empty).ToString();
                        rec.data4        = (excelTable[i, DatafileColumnDefinition.data4  ] ?? string.Empty).ToString();
                        rec.data5        = (excelTable[i, DatafileColumnDefinition.data5  ] ?? string.Empty).ToString();

                        ret.Add(rec);
                    }

                    return ret;
                }
            }
        }

        static private ObservableCollection<EmailData> GetCSV(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
