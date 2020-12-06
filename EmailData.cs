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

        private string?[]?  _groups;
        public  string?[]   groups   => (_groups  ?? GroupsFill());

        private string?[]?  _datas;
        public  string?[]   datas    => (_datas   ?? DatasFill());

        private bool        _check;  
        public  bool        check   { get { return _check; }   set { _check = value; OnPropertyChanged(); } } 

        #endregion

        public EmailData()
        {
 
        }

        public readonly static char[] GroupSeparators = { ',', ' ', ';', '\t', '/', '|' };

        /// <summary>
        /// WARNING: Assumed you will read the 'groups' property only after you have filled group1,group2,group3,group4,group5 properties
        /// </summary>
        private string?[] GroupsFill()
        {
            var t = group1 + "," + group2 + "," + group3 + "," + group4 + "," + group5;

            _groups = t.Split(GroupSeparators, StringSplitOptions.RemoveEmptyEntries);
            
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

                    for (int i = 0; i < excelTable.rowCount; i++)
                    {
                        var rec = new EmailData();

                        rec.id            = (string)excelTable[i, DatafileColumnDefinition.id     ];

                        rec.name1         = (string)excelTable[i, DatafileColumnDefinition.name1  ];
                        rec.name2         = (string)excelTable[i, DatafileColumnDefinition.name2  ];
                        rec.name3         = (string)excelTable[i, DatafileColumnDefinition.name3  ];

                        rec.email         = (string)excelTable[i, DatafileColumnDefinition.email  ];
                        rec.subject       = (string)excelTable[i, DatafileColumnDefinition.subject];
                        rec.body          = (string)excelTable[i, DatafileColumnDefinition.body   ];

                        rec.attach1       = (string)excelTable[i, DatafileColumnDefinition.attach1];
                        rec.attach2       = (string)excelTable[i, DatafileColumnDefinition.attach2];
                        rec.attach3       = (string)excelTable[i, DatafileColumnDefinition.attach3];
                        rec.attach4       = (string)excelTable[i, DatafileColumnDefinition.attach4];
                        rec.attach5       = (string)excelTable[i, DatafileColumnDefinition.attach5];

                        rec.group1        = (string)excelTable[i, DatafileColumnDefinition.group1 ];
                        rec.group2        = (string)excelTable[i, DatafileColumnDefinition.group2 ];
                        rec.group3        = (string)excelTable[i, DatafileColumnDefinition.group3 ];
                        rec.group4        = (string)excelTable[i, DatafileColumnDefinition.group4 ];
                        rec.group5        = (string)excelTable[i, DatafileColumnDefinition.group5 ];

                        rec.data1        = (string)excelTable[i, DatafileColumnDefinition.data1   ];
                        rec.data2        = (string)excelTable[i, DatafileColumnDefinition.data2   ];
                        rec.data3        = (string)excelTable[i, DatafileColumnDefinition.data3   ];
                        rec.data4        = (string)excelTable[i, DatafileColumnDefinition.data4   ];
                        rec.data5        = (string)excelTable[i, DatafileColumnDefinition.data5   ];

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
