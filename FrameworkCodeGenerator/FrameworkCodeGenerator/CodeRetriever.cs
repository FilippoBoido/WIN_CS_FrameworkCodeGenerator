using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TwincatCodeGenerator.HelperClasses;
namespace TwincatCodeGenerator
{
    class CodeRetriever
    {
        string pathToXmlFile = null;
        string moduleName = null;
        string modulePath = null;
        string eventManagerName = null;

        int modulesQuantity = 0;
        int eventManagersQuantity = 0;
        int eventsQuantity = 0;
        int modulesCounter = 0;
        int eventManagerCounter = 0;
        int eventsCounter = 0;

        ModulesContainer modulesContainer = null;
        Module currentModule = null;
        EventManager currentEventManager = null;

        public CodeRetriever(String pathToXmlFile)
        {
            this.pathToXmlFile = pathToXmlFile;
            Run();
        }

        public void Run()
        {

            Console.WriteLine("Retrieving text from {0}", pathToXmlFile);
            string nodeName = null;
            
            XmlTextReader xmlReader = new XmlTextReader(pathToXmlFile);
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:

                        nodeName = xmlReader.Name;
                        if (nodeName.Equals("Modules"))
                        {
                            RetrieveModulesCode(xmlReader.ReadSubtree());
                            return;
                        }
                        break;

                    case XmlNodeType.Text:

                        Functions.ConfigFileDataRawAssignment(nodeName, xmlReader.Value.Trim());
                        break;
                }

            }

            Console.WriteLine("Text retrieved..");

        }

        public void RetrieveModulesCode(XmlReader subTreeReader)
        {
            while (subTreeReader.Read())
            {
                switch (subTreeReader.Name)
                {
                        
                    case "Modules":

                        if (subTreeReader.NodeType == XmlNodeType.Element)
                        {

                            Console.WriteLine("Modules element.");
                            modulesQuantity = Convert.ToInt32(subTreeReader["modulesQuantity"]);
                            
                            AssignedConfigFileData.modulesContainer = new ModulesContainer(modulesQuantity);
                            modulesContainer =  AssignedConfigFileData.modulesContainer;
                        }
                        /*
                        else if (subTreeReader.NodeType == XmlNodeType.EndElement)
                        {
                            AssignedConfigFileData.modulesContainer = modulesContainer;
                        }
                        */
                        break;

                    case "ModuleTarget":

                        if (subTreeReader.NodeType == XmlNodeType.Element)
                        {                           
                                               
                            eventManagerCounter = 0;

                            eventManagersQuantity = Convert.ToInt32(subTreeReader["eventManagersQuantity"]);

                            moduleName = subTreeReader["name"];
                            modulePath = subTreeReader["path"];

                            modulesContainer.content[modulesCounter] = new Module(eventManagersQuantity, moduleName, modulePath);
                            currentModule = modulesContainer.content[modulesCounter];
                            modulesCounter += 1;

                            Console.WriteLine("ModuleTarget element: {0}", moduleName);
                        }


                        break;

                    case "EventManagerTargetName":
                        if (subTreeReader.NodeType == XmlNodeType.Element)
                        {                          
                            eventsCounter = 0;
                            eventsQuantity = Convert.ToInt32(subTreeReader["eventsQuantity"]);
                            eventManagerName = subTreeReader["name"];
                            currentModule.content[eventManagerCounter] = new EventManager(eventsQuantity, eventManagerName);
                            currentEventManager = currentModule.content[eventManagerCounter];
                            eventManagerCounter += 1;
                            Console.WriteLine("eventManagersNames element: {0}", eventManagerName);
                        }
                        break;

                    case "EventName":
                        
                        if(subTreeReader.NodeType == XmlNodeType.Element)
                        {
                            Console.WriteLine("EventName element.");
                            // Next read will contain text.
                            if (subTreeReader.Read())
                            {
                                Console.WriteLine("Text node: " + subTreeReader.Value.Trim());
                             
                                currentEventManager.content.eventNames[eventsCounter] = subTreeReader.Value.Trim();
                                eventsCounter += 1;

                            }
                        }

                        break;
                }
            }

            Console.WriteLine("Code retrieved..");
        }

    }
}
