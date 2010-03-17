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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Schema;


namespace Wayloop.Highlight
{
    public class Global
    {
        public static string CreateCssClassName(string definition, string pattern)
        {
            var cssClassName = definition
                .Replace("#", "sharp")
                .Replace("+", "plus")
                .Replace(".", "dot")
                .Replace("-", "");

            return String.Concat(cssClassName, pattern);
        }


        public static string CreatePatternStyle(Color foreColor, Color backColor, Font font)
        {
            var patternStyle = new StringBuilder();
            if (foreColor != Color.Empty) {
                patternStyle.Append("color: " + foreColor.Name + ";");
            }
            if (backColor != Color.Empty) {
                patternStyle.Append("background-color: " + backColor.Name + ";");
            }
            if (font.Name != null) {
                patternStyle.Append("font-family: " + font.Name + ";");
            }
            if (font.Size > 0f) {
                patternStyle.Append("font-size: " + font.Size + "px;");
            }
            if (font.Style == FontStyle.Regular) {
                patternStyle.Append("font-weight: normal;");
            }
            if (font.Style == FontStyle.Bold) {
                patternStyle.Append("font-weight: bold;");
            }

            return patternStyle.ToString();
        }


        public static string Escape(string str)
        {
            if (str != @"\n") {
                str = Regex.Escape(str);
            }

            return str;
        }


        public static string GetCleanInput(string input)
        {
            input = Regex.Replace(input, @"\<br\ ?\/?>", "\n", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\<p\>", "\n\n", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\<\/p\>", string.Empty, RegexOptions.IgnoreCase);
            input = input.Replace("\r\n", "\n");
            input = HttpUtility.HtmlDecode(input);
            input = input
                .TrimStart(new[] { '\n' })
                .TrimEnd(new[] { '\n' });

            return input;
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
                if (configurationFile == string.Empty) {
                    if (ConfigurationManager.AppSettings["configurationFile"] != null) {
                        configurationFile = ConfigurationManager.AppSettings["configurationFile"];
                    }
                    else {
                        configurationFile = string.Format(Path.Combine("{0}", "Definitions.xml"), ApplicationDirectory);
                    }
                }
                if (!File.Exists(configurationFile)) {
                    throw new FileNotFoundException(string.Format("The configuration file does not exist at location <i>{0}</i>", configurationFile), configurationFile);
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


        public static string HtmlDecode(string str)
        {
            str = Regex.Replace(str, "&amp;", "&");
            str = Regex.Replace(str, "&lt;", "<");
            str = Regex.Replace(str, "&gt;", ">");
            return str;
        }


        public static string HtmlEncode(string str)
        {
            str = Regex.Replace(str, "&", "&amp;");
            str = Regex.Replace(str, "<", "&lt;");
            str = Regex.Replace(str, ">", "&gt;");
            return str;
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
                return string.Format(@"{0}\Definitions.xml", ApplicationDirectory);
            }
        }


        public static bool IsWebForm
        {
            get { return HttpContext.Current != null; }
        }
    }
}