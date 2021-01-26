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
        public  string name      ;
        public  string dataFile  ;
        public  bool   textBody  ;
        public  string testEmail ;
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
            this.testEmail = "______@gmail.com";
        }

        public void SaveAsXML()
        {
            var filename = Path.Combine(xmlDirectoryChecked, $"Profil_{name}.xml");

            this.SaveAsXML<Profil>(filename);
        }

        public static Profil LoadFromXML(string profilName)
        {
            var filename = Path.Combine(xmlDirectoryChecked, $"Profil_{profilName}.xml");

            if (! File.Exists(filename))
            {
                return new Profil(profilName);                
            }

            return SerializationExtensions.LoadFromXML<Profil>(filename) ?? new Profil(profilName);
        }

        private string filenameSubjectAndBody => Path.Combine(xmlDirectoryChecked, $"Profil_{name}_Content.txt");

        public (string subject, string body) LoadSubjectAndBody()
        {
            if (File.Exists(filenameSubjectAndBody))
            {
                var content = File.ReadAllLines(filenameSubjectAndBody);

                if (content.Length >= 1)
                {
                    string line = content[0].Trim();
                    string body;
                    string subject;

                    if (line.StartsWith("<!--") && line.EndsWith("-->"))
                    {
                        subject = line[4..^3];

                        if (subject.StartsWith("subject:", StringComparison.OrdinalIgnoreCase))
                        {
                            subject = line[8..];                            
                        }
                        else if (subject.StartsWith("subject", StringComparison.OrdinalIgnoreCase))
                        {
                            subject = line[7..];                            
                        }

                        body = string.Join('\n', content[1..]);                                                                                 // https://blog.ndepend.com/c-index-and-range-operators-explained/
                    }
                    else
                    {
                        subject = String.Empty;
                        body = string.Join('\n', content);
                    }
                     
                    return (subject, body);
                }                
            }

            return (String.Empty, "Tisztelt _______ !" + Environment.NewLine + Environment.NewLine);
        }

        public void SaveSubjectAndBody(string subject, string body)
        {
            var sb = new StringBuilder(subject.Length + body.Length + 64);

            sb.Append("<!--"); 
            sb.Append("subject:");     
            sb.Append(subject);                
            sb.Append("-->");
            sb.Append(Environment.NewLine);
            sb.Append(body);

            File.WriteAllText(filenameSubjectAndBody, sb.ToString());
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
