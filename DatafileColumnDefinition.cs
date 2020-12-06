using System;
using System.Collections.Generic;
using System.Text;

using EPPlus.SimpleTable;

namespace SendEmail2SelectedGroup
{
    /// <summary>
    /// List of HEAD columns for 'excel datafile'
    /// Values must starts from one and increments by one; because enum value can index column of worksheet table as EPPlus's worksheet.Cells[] do it.
    /// </summary>
    public enum DatafileColumnDefinition
    {
        [ColumnType(typeof(string), minLen :  0,  maxLen :   20)]       id          = 1,
        [ColumnType(typeof(string), minLen : 10,  maxLen :   60)]       name1       ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :   60)]       name2       ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :   60)]       name3       ,
        [ColumnType(typeof(string), minLen :  8,  maxLen :  100)]       email       ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :  120)]       subject     ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 3000)]       body        ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :  250)]       attach1     ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :  250)]       attach2     ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :  250)]       attach3     ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :  250)]       attach4     ,
        [ColumnType(typeof(string), minLen :  0,  maxLen :  250)]       attach5     ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 1000)]       group1      ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 1000)]       group2      ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 1000)]       group3      ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 1000)]       group4      ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 1000)]       group5      ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 3000)]       data1       ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 3000)]       data2       ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 3000)]       data3       ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 3000)]       data4       ,
        [ColumnType(typeof(string), minLen :  0,  maxLen : 3000)]       data5
    }
}
