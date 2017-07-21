using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SM.Media.Core.Utility
{
    public static class ApplicationInformationFactory
    {
        private static readonly IApplicationInformation ApplicationInformation = ApplicationInformationFactory.Initialize();

        public static IApplicationInformation Default
        {
            get
            {
                return ApplicationInformationFactory.ApplicationInformation;
            }
        }

        private static IApplicationInformation Initialize()
        {
            try
            {
                string inputUri = "WMAppManifest.xml";
                XmlReaderSettings settings = new XmlReaderSettings()
                {
                    XmlResolver = (XmlResolver)new XmlXapResolver()
                };
                using (XmlReader xmlReader = XmlReader.Create(inputUri, settings))
                {
                    if (xmlReader.ReadToDescendant("App") && xmlReader.IsStartElement())
                        return (IApplicationInformation)new ApplicationInformation(xmlReader.GetAttribute("Title"), xmlReader.GetAttribute("Version"));
                    Debug.WriteLine("Cannot find <App> in WMAppManifest.xml");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ApplicationInformationFactory.Initialize() Unable to create application information: " + ex.Message);
            }
            return (IApplicationInformation)new Utility.ApplicationInformation((string)null, (string)null);
        }
    }
}
