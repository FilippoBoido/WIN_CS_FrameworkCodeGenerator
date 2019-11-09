using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwincatCodeGenerator.HelperClasses
{
    class EventManager
    {
        EventNames eventNames;
        string plcProjectPath;
        string name;

        public EventManager(int numberOfEvents, string name)
        {
            eventNames = new EventNames(numberOfEvents);
            this.name = name;
        }

        public EventNames content
        {
            get { return eventNames; }
            set { eventNames = value; }
        }

        public string getPlcProjectPath
        {
            get { return plcProjectPath; }
        }

        public string setPlcProjectPath
        {
            set { plcProjectPath = value; }
        }

        public string getName
        {
            get { return name; }
        }

        public string setName
        {
            set { name = value; }
        }
    }
}
