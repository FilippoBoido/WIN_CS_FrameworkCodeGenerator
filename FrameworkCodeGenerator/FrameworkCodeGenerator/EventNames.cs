using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwincatCodeGenerator
{
    class EventNames
    {
        public String[] eventNames;

        public EventNames(int events)
        {
            eventNames = new String[events];
        }

        public int getArrayLength{
            get { return eventNames.Length; }
        }

        public String[] content
        {
            get { return eventNames; }
            set { eventNames = value;  }
        }
    }
}
