using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCatSysManagerLib;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using TwincatCodeGenerator.ActivityClasses.ModulesCreation;
using TwincatCodeGenerator.HelperClasses;

namespace TwincatCodeGenerator
{

    class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {



            //-----------------------------------------------------------------------------------------------------------------------
            //                                          Retrieve all relevant data from config file
            //-----------------------------------------------------------------------------------------------------------------------

            ConfigFileReader configFileReader = new ConfigFileReader(); 
            Assigner assigner = new Assigner();

            MessageFilter.Register();

            //-----------------------------------------------------------------------------------------------------------------------
            //                                          Attach to existing Dte
            //-----------------------------------------------------------------------------------------------------------------------

            

            AssignedConfigFileData.pathToSolution = Assigner.ReplaceBlueprintValues(
                                                        AssignedConfigFileData.pathToSolution,
                                                        new string[] { "@NameOfProject@" },
                                                        new string[] { AssignedConfigFileData.nameOfProject }
                                                    );

            Console.WriteLine("pathToSolution: {0}",AssignedConfigFileData.pathToSolution);

            AssignedConfigFileData.pathToPOUsFolder = Assigner.ReplaceBlueprintValues(
                                                          AssignedConfigFileData.pathToPOUsFolder,
                                                          new string[] { "@NameOfPLCProject@" },
                                                          new string[] { AssignedConfigFileData.nameOfPlcProject }
                                                      );

            Console.WriteLine("pathToPOUsFolder: {0}",AssignedConfigFileData.pathToPOUsFolder);

            AssignedConfigFileData.pathToDUTsFolder = Assigner.ReplaceBlueprintValues(
                                                          AssignedConfigFileData.pathToDUTsFolder,
                                                          new string[] { "@NameOfPLCProject@" },
                                                          new string[] { AssignedConfigFileData.nameOfPlcProject }
                                                      );

            Console.WriteLine("pathToDUTsFolder: {0}",AssignedConfigFileData.pathToDUTsFolder);

            AssignedConfigFileData.pathToGVLsFolder = Assigner.ReplaceBlueprintValues(
                                                          AssignedConfigFileData.pathToGVLsFolder,
                                                          new string[] { "@NameOfPLCProject@" },
                                                          new string[] { AssignedConfigFileData.nameOfPlcProject }
                                                      );

            Console.WriteLine("pathToGVLsFolder: {0}", AssignedConfigFileData.pathToGVLsFolder);

            AssignedConfigFileData.pathToMAIN = Assigner.ReplaceBlueprintValues(
                                                          AssignedConfigFileData.pathToMAIN,
                                                          new string[] { "@NameOfPLCProject@" },
                                                          new string[] { AssignedConfigFileData.nameOfPlcProject }
                                                      );

            Console.WriteLine("pathToMAIN: {0}", AssignedConfigFileData.pathToMAIN);

            List<Project> projectList = new List<Project>();
            Project plcProj = null ;
            bool projectFound = false;

            ITcPlcDeclaration decl = (ITcPlcDeclaration)null;
            ITcPlcImplementation impl = (ITcPlcImplementation)null;

            ITcSmTreeItem currentMethod = null;
            ITcSmTreeItem pou = null;
            ITcSmTreeItem dut = null;
            ITcSmTreeItem gvl = null;
            ITcSmTreeItem main = null;
            ITcSmTreeItem modules = null;
            ITcSmTreeItem constants = null;
            ITcSmTreeItem system = null;
            ITcSmTreeItem targetModule = null;
            ITcSmTreeItem triggers = null;
            ITcSmTreeItem eventManagers = null;
            ITcSmTreeItem interfaces = null;
            ITcSmTreeItem i_sequence = null;

            ITcSysManager sysManager = null;

            EnvDTE.DTE dte =
                DteAttacher.attachToExistingDte(
                    AssignedConfigFileData.pathToSolution,
                    AssignedConfigFileData.visualStudioVersion,
                    AssignedConfigFileData.nameOfProject

                );
            
            dynamic solution = dte.Solution;

            Console.WriteLine("Full solution name:");
            Console.WriteLine(solution.FullName);

            Projects prjs = solution.projects;
            Console.WriteLine("Projects in solution: {0}", prjs.Count);
            for (int i = 1; i <= prjs.Count; i++)
            {
                projectList.Add(prjs.Item(i));
                Console.WriteLine("Project found: {0}", prjs.Item(i).Name);
                Console.WriteLine("Searching for project: {0}", AssignedConfigFileData.nameOfProject);
                if (prjs.Item(i).Name.Equals(AssignedConfigFileData.nameOfProject))
                {          
                    plcProj = prjs.Item(i);
                    projectFound = true;
                }
            }

            //-----------------------------------------------------------------------------------------------------------------------
            //                                          Perform checks
            //-----------------------------------------------------------------------------------------------------------------------

            Console.WriteLine("Performing checks:");

            if (projectFound)
            {
                Console.WriteLine("Project found.");
                sysManager = plcProj.Object;
            }
            else
            {
                Console.WriteLine("Project not found. Exiting on Enter.");
                Console.Read();
                return;
            }

            //Is POUs item available?
            Functions.SearchForItem(AssignedConfigFileData.pathToPOUsFolder, ref sysManager, ref pou);

            //Is DUTs item available?
            Functions.SearchForItem(AssignedConfigFileData.pathToDUTsFolder, ref sysManager, ref dut);

            //Is GVLs item available?
            Functions.SearchForItem(AssignedConfigFileData.pathToGVLsFolder, ref sysManager, ref gvl);

            //Is Main item available?
            Functions.SearchForItem(AssignedConfigFileData.pathToMAIN, ref sysManager, ref main);

            if (AssignedConfigFileData.activity.Equals("SpecificEventsModuleCreator"))
            {

                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- MAIN implementation -----------------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                impl = (ITcPlcImplementation)main;
                impl.ImplementationText = AssignedConfigFileData.blueprintMainImplementation;

                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create methods for MAIN -------------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                currentMethod = main.CreateChild("systemTime", 609, "");
                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = AssignedConfigFileData.blueprintMainSystemTimeDeclaration;

                impl = (ITcPlcImplementation)currentMethod;
                impl.ImplementationText = AssignedConfigFileData.blueprintMainSystemTimeImplementation;

                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create the  ModulesGVL --------------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                modules = gvl.CreateChild("Modules", 615, "", AssignedConfigFileData.blueprintModulesGvlDeclaration);

                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create the  ConstantsGVL ------------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                constants = gvl.CreateChild("Constants", 615, "", AssignedConfigFileData.blueprintConstantsGvlDeclaration);
                
                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create the SystemGVL ----------------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                system = gvl.CreateChild("System", 615, "", AssignedConfigFileData.blueprintSystemGvlDeclaration);
                
                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create the EventManagers folder -----------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                eventManagers = pou.CreateChild("EventManagers", 601, "", "1");
               
                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create the Interfaces folder --------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                interfaces = pou.CreateChild("Interfaces", 601, "", "1");
               
                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create the I_Sequence interface -----------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                i_sequence = interfaces.CreateChild("I_Sequence", 618);

                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------------- Create the sequence interface method ------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                currentMethod = i_sequence.CreateChild("sequence", 610);
                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = AssignedConfigFileData.blueprintInterfaceSequenceMethodDeclaration;
                
            }
            else
            {
                //Is the Module GVL present?
                Functions.SearchForSubItem("Modules", ref gvl, ref modules);

                //Is the Triggers GVL present?
                //Functions.SearchForSubItem("Triggers", ref gvl, ref triggers);

                //Is the Eventmanagers folder present?
                Functions.SearchForSubItem("EventManagers", ref pou, ref eventManagers);

            }

            //Is TargetModule present?
            /*
            if (AssignedConfigFileData.activity.Equals("InjectNewEventManagerInTarget"))
            {
                Functions.SearchForItem(AssignedConfigFileData.pathToTargetModule, ref sysManager, ref targetModule);
            }
            */
            //Is the Interfaces folder present?
            //Functions.SearchForSubItem("Interfaces", ref pou, ref interfaces);

            //-----------------------------------------------------------------------------------------------------------------------
            //                                          Start activity
            //-----------------------------------------------------------------------------------------------------------------------

            Console.WriteLine("Executing program.");

            for (int i = 0; i < AssignedConfigFileData.modulesContainer.content.Length; i++)
            { 
                if (AssignedConfigFileData.activity.Equals("GeneralEventsModuleCreator"))
                {
                    new GeneralEventsModuleCreator(i, pou, dut, gvl, eventManagers, modules, triggers);
                }
                else if (AssignedConfigFileData.activity.Equals("SpecificEventsModuleCreator"))
                {
                    new SpecificEventsModuleCreator(i, pou, dut, gvl, eventManagers, modules, triggers);
                }
            
            }
            MessageFilter.Revoke();
            Console.WriteLine("Finished!");
            Console.Read();
            
        }
    
               
    }
    
}
