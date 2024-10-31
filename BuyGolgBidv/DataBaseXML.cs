using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BuyGolgBidv
{
    public class DataBaseXML
    {
        public PageSJCV PageSJC { get; set; }
        public UserInfo UserInfo { get; set; }

        public async Task LoadFromXmlAsync(string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(DataBaseXmlWrapper));
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

                var dataWrapper = await Task.Run(() => (DataBaseXmlWrapper)serializer.Deserialize(stream));

                PageSJC = dataWrapper.PageSJC;
                UserInfo = dataWrapper.UserInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading XML file: " + ex.Message);
            }
        }
    }

    // Wrapper class to match the XML root element "Data"
    [XmlRoot("Data")]
    public class DataBaseXmlWrapper
    {
        public PageSJCV PageSJC { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
