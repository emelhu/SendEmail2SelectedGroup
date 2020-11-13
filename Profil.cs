#nullable enable   

using System;
using System.Collections.Generic;
using System.Text;

namespace SendEmail2SelectedGroup
{
    using System.IO;
    using eMeL_Common;

    [Serializable]
    public class Profil
    {
        #region Stored to XML
        public  string name     ;
        public  string dataFile ;
        #endregion

        public static string? xmlDirectory;
        public static string  xmlDirectoryChecked => xmlDirectory ?? NullException();

        private static string NullException()
        {
            throw new ArgumentNullException("A 'Profil.xmlDirectory' kitöltése kötelező!");
        }

        public Profil() : this(ProfilNames.defaultName)
        {   // For design time           
        }

        public Profil(string name)
        {
            this.name      = name.Trim();
            this.dataFile  = "Profil_" + name + ".XLSX";
        }

        public void SaveAsXML()
        {
            var filename = Path.Combine(xmlDirectoryChecked, $"Profil_{name}.xml");

            this.SaveAsXML<Profil>(filename);
        }

        public static Profil LoadFromXML(string name)
        {
            var filename = Path.Combine(xmlDirectoryChecked, $"Profil_{name}.xml");

            if (! File.Exists(filename))
            {
                return new Profil(name);
                
            }

            return SerializationExtensions.LoadFromXML<Profil>(filename) ?? new Profil(name);
        }

    }

    //

    [Serializable]
    public class ProfilNames
    {
        public List<string> names   { get; private set; }
        public string       last    { get; set; }

        public ProfilNames()
        {   // Default and/or for design time
            this.last = defaultName;

            this.names = new List<string>();
            this.names.Add(defaultName);
        }

        public const string xmlFileName = "ProfilNames.xml";
        public const string defaultName = "Alapertelmezett";

        public void SaveAsXML()
        {
            var filename = Path.Combine(Profil.xmlDirectoryChecked, xmlFileName);

            this.SaveAsXML<ProfilNames>(filename);
        }

        public static ProfilNames LoadFromXML()
        {
            var filename = Path.Combine(Profil.xmlDirectoryChecked, xmlFileName);

            if (! File.Exists(filename))
            {
                return new ProfilNames();
            }

            return SerializationExtensions.LoadFromXML<ProfilNames>(filename) ?? new ProfilNames();
        }
    }
}
