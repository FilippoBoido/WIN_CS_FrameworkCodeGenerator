using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//ToDo need eventNames and eventManagersNames
namespace TwincatCodeGenerator.HelperClasses
{
    class Assigner
    {
        Module[] modules = null;

        //List<EventManager[]> eventManagers;
        //List<String[]> eventNames;

        public Assigner()
        {
            modules = AssignedConfigFileData.modulesContainer.content;
            if (modules == null)
            {
                Console.WriteLine("modules == null");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            AssignModuleCoreData();
            AssignModuleCode();
            //AssignEventManagersCoreData();
        }

        public void AssignModuleCoreData()
        {
            AssignedConfigFileData.modulesNames = new String[modules.Length];
            AssignedConfigFileData.modulesPaths = new String[modules.Length];

            for (int i = 0; i < modules.Length; i++)
            {
                AssignedConfigFileData.modulesNames[i] = modules[i].getName;
                AssignedConfigFileData.modulesPaths[i] = modules[i].getPath;
            }

        }

        public void AssignModuleCode()
        {
            AssignedConfigFileData.modulesImplementations = new String[modules.Length];
            AssignedConfigFileData.modulesDeclarations = new String[modules.Length];
            
            for (int i = 0; i < modules.Length; i++)
            {
                AssignedConfigFileData.modulesDeclarations[i] = AssignedConfigFileData.blueprintModuleDeclaration;
                AssignedConfigFileData.modulesImplementations[i] = AssignedConfigFileData.blueprintModuleImplementation;         
            }

        }


        public static string ReplaceBlueprintValues(string bluePrint, String[] keys, String[] newValues)
        {
            string result = bluePrint;

            for(int i = 0; i<keys.Length; i++)
            {
                result = result.Replace(keys[i], newValues[i]);
            }
            return result;
        }
       
    }
}
