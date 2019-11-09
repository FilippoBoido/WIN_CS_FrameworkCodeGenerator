using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TwincatCodeGenerator
{
    class ConfigFileReader
    {
        public ConfigFileReader()
        {
            Console.WriteLine("Reading config file..");

            XmlTextReader xmlReader = new XmlTextReader("CodeGeneratorConfig.xml");

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {

                    case XmlNodeType.Text:
                        new CodeRetriever(xmlReader.Value.Trim());
                        break;
                }

            }

            Console.WriteLine("Values retrieved and assigned..");
        }

    }
}