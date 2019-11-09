using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwincatCodeGenerator.HelperClasses
{
    class ModulesContainer
    {
        Module[] modules;
        public ModulesContainer(int numberOfModules)
        {
            modules = new Module[numberOfModules];
        }

        public Module[] content
        {
            get { return modules;}
            set { modules = value;}
        }
    }
}
