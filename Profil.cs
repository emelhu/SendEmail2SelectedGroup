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
        public string name    { get; set; }
        public string xmlfile { get; set; } 

        public Profil()
        {   // For design time
            #if DEBUG
            this.name     = "DesignTime";
            this.xmlfile  = "DesignTime.XML";
            #else
            this.name     = String.Empty;
            this.xmlfile  = String.Empty;
            #endif

        }

        public Profil(string name)
        {
            this.name     = name;
            this.xmlfile  = String.Empty;
        }

        public void SaveAsXML(string directory)
        {
            var filename = Path.Combine(directory, $"Profil_{name}.xml");

            this.SaveAsXML<Profil>(filename);
        }

        public static Profil LoadFromXML(string directory, string name)
        {
            var filename = Path.Combine(directory, $"Profil_{name}.xml");

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

        private const string xmlFileName = "ProfilNames.xml";
        private const string defaultName = "Alapertelmezett";

        public void SaveAsXML(string directory)
        {
            var filename = Path.Combine(directory, xmlFileName);

            this.SaveAsXML<ProfilNames>(filename);
        }

        public static ProfilNames LoadFromXML(string directory)
        {
            var filename = Path.Combine(directory, xmlFileName);

            if (! File.Exists(filename))
            {
                return new ProfilNames();
            }

            return SerializationExtensions.LoadFromXML<ProfilNames>(filename) ?? new ProfilNames();
        }
    }
}
