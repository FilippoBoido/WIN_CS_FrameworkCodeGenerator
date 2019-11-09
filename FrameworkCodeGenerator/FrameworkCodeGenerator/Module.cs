using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwincatCodeGenerator.HelperClasses
{
    class Module
    {
        EventManager[] eventManagers;
        string plcProjectPath;
        string name;
        string path;

        public Module(int numberOfEventManagers, string name, string path)
        {
            eventManagers = new EventManager[numberOfEventManagers];
            this.name = name;
            this.path = path;
        }

        public EventManager[] content
        {
            get { return eventManagers; }
            set { eventManagers = value; }
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

        public string getPath
        {
            get { return path; }
        }

        public string setPath
        {
            set { path = value; }
        }
    }
}
