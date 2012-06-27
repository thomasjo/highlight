using System.Xml.Linq;

namespace Highlight.Configuration
{
    public class DefaultConfiguration : XmlConfiguration
    {
        public DefaultConfiguration()
        {
            XmlDocument = XDocument.Parse(Resources.DefaultDefinitions);
        }
    }
}