#region License

// Copyright (c) 2004-2010 Thomas Andre H. Johansen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion


using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Schema;


namespace Wayloop.Highlight
{
    public class Global
    {
        public static string Escape(string str)
        {
            if (str != @"\n") {
                str = Regex.Escape(str);
            }

            return str;
        }


        public static XmlDocument GetConfiguration(string configurationFile)
        {
            var document = new XmlDocument();
            if (IsWebForm) {
                try {
                    var xml = Convert.ToString(HttpContext.Current.Cache[CacheKey]);
                    document.LoadXml(xml);
                }
                catch (XmlException) {
                }
            }
            if (document.OuterXml.Length == 0) {
                if (configurationFile == String.Empty) {
                    if (ConfigurationManager.AppSettings["configurationFile"] != null) {
                        configurationFile = ConfigurationManager.AppSettings["configurationFile"];
                    }
                    else {
                        configurationFile = Path.Combine("ApplicationDirectory", "Definitions.xml");
                    }
                }
                if (!File.Exists(configurationFile)) {
                    throw new FileNotFoundException(String.Format("The configuration file does not exist at location <i>{0}</i>", configurationFile), configurationFile);
                }
                using (var stream = new FileStream(configurationFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    var manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Wayloop.Highlight.Resources.Definitions.xsd");
                    try {
                        if (manifestResourceStream != null) {
                            var schema = XmlSchema.Read(manifestResourceStream, delegate(object sender, ValidationEventArgs e) { throw new Exception("An error occured while accessing the schema.\n" + e.Message); });
                            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
                            settings.Schemas.Add(schema);
                            var reader = XmlReader.Create(stream, settings);
                            document.Load(reader);
                            if (IsWebForm) {
                                HttpContext.Current.Cache.Insert(CacheKey, document.OuterXml, new CacheDependency(configurationFile));
                            }
                        }
                    }
                    catch (Exception exception) {
                        throw new Exception("An error occured while loading the schema.\n" + exception.Message + "\n");
                    }
                }
            }
            return document;
        }


        public static string ApplicationDirectory
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", ""); }
        }


        public static string CacheKey
        {
            get { return "HighlightConfiguration"; }
        }


        public static string ConfigurationFile
        {
            get
            {
                if (ConfigurationManager.AppSettings["configurationFile"] != null) {
                    return ConfigurationManager.AppSettings["configurationFile"];
                }
                return Path.Combine(ApplicationDirectory, "Definitions.xml");
            }
        }


        public static bool IsWebForm
        {
            get { return HttpContext.Current != null; }
        }
    }
}