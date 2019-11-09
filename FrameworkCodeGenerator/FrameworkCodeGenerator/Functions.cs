using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
namespace TwincatCodeGenerator
{
    class Functions
    {
        public static bool SearchForItem(string itemPath, ref ITcSysManager sysManager, ref ITcSmTreeItem item)
        {
            try
            {
                item = sysManager.LookupTreeItem(itemPath);
                Console.WriteLine(itemPath + " item found.");
            }
            catch (Exception e)
            {
                Console.WriteLine(itemPath + " item not found. Exiting on Enter.");
                Console.Read();
                Environment.Exit(-1) ;
            }
            return true;
        }

        public static bool SearchForSubItem(string itemToLookFor,ref ITcSmTreeItem parentItem, ref ITcSmTreeItem childItem)
        {
            try
            {
                childItem = parentItem.LookupChild(itemToLookFor);
                Console.WriteLine(itemToLookFor +" item found.");
            }
            catch (Exception e)
            {
                Console.WriteLine(itemToLookFor +" item not found. Exiting on Enter.");
                Console.Read();
                Environment.Exit(-1);
            }
            return true;
        }

        public static string ResultForKey(string startKey, string endKey, string txt)
        {
            string result = null;
            try
            {
                int first = txt.IndexOf(startKey) + startKey.Length;
                int last = txt.LastIndexOf(endKey);
                result = txt.Substring(first, last - first);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.Read();
                Environment.Exit(-1);
            }
            return result.Trim();
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string LowercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }

        public static void ConfigFileDataRawAssignment(string nodeName, string nodeContent)
        {
            switch(nodeName)
            {
                case "VisualStudioVersion":
                    AssignedConfigFileData.visualStudioVersion = nodeContent;           
                    break;
                case "NameOfProject":
                    AssignedConfigFileData.nameOfProject = nodeContent;
                   
                    break;
                case "NameOfPLCProject":
                    AssignedConfigFileData.nameOfPlcProject = nodeContent;
                    
                    break;
                case "PathToSolution":
                    AssignedConfigFileData.pathToSolution = nodeContent;
                    break;
                case "PathToPOUsFolder":
                    AssignedConfigFileData.pathToPOUsFolder = nodeContent;
                    break;
                case "PathToDUTsFolder":
                    AssignedConfigFileData.pathToDUTsFolder = nodeContent;
                    break;
                case "PathToGVLsFolder":
                    AssignedConfigFileData.pathToGVLsFolder = nodeContent;
                    break;
                case "PathToTargetModule":
                    AssignedConfigFileData.pathToTargetModule = nodeContent;
                    break;
                case "PathToMAIN":
                    AssignedConfigFileData.pathToMAIN = nodeContent;
                    break;
                case "PathToEventManagersFolder":
                    AssignedConfigFileData.pathToEventManagerFolder = nodeContent;
                    break;
                case "Activity":
                    AssignedConfigFileData.activity = nodeContent;
                    break;

                case "InterfaceSequenceMethodDeclarationText":
                    AssignedConfigFileData.blueprintInterfaceSequenceMethodDeclaration = nodeContent;
                    break;

                case "MainDeclarationText":
                    AssignedConfigFileData.blueprintMainDeclaration = nodeContent;
                    break;
                case "MainImplementationText":
                    AssignedConfigFileData.blueprintMainImplementation = nodeContent;
                    break;
                case "MainSystemTimeDeclarationText":
                    AssignedConfigFileData.blueprintMainSystemTimeDeclaration = nodeContent;
                    break;
                case "MainSystemTimeImplementationText":
                    AssignedConfigFileData.blueprintMainSystemTimeImplementation = nodeContent;
                    break;

                case "EnumEventDeclarationText":
                    AssignedConfigFileData.blueprintEnumEventDeclaration = nodeContent;
                    break;
                case "EventRecordDeclarationText":
                    AssignedConfigFileData.blueprintEventRecordDeclaration = nodeContent;
                    break;
                case "EventManagerDeclarationText":
                    AssignedConfigFileData.blueprintEventManagerDeclaration = nodeContent;
                    break;
                case "EventManagerImplementationText":
                    AssignedConfigFileData.blueprintEventManagerImplementation = nodeContent;
                    break;
                case "TargetModuleImplementationText":
                    AssignedConfigFileData.blueprintModuleImplementation = nodeContent;
                    break;
                case "TargetModuleDeclarationText":
                    AssignedConfigFileData.blueprintModuleDeclaration = nodeContent;
                    break;
                case "ModuleEventManagerDeclarationText":
                    AssignedConfigFileData.blueprintModuleEventManagerDeclaration = nodeContent;
                    break;
                case "ModuleEventManagerImplementationText":
                    AssignedConfigFileData.blueprintModuleEventManagerImplementation = nodeContent;
                    break;
                case "ModuleTriggerEventsDeclarationText":
                    AssignedConfigFileData.blueprintModuleTriggerEventsDeclaration = nodeContent;
                    break;
                case "EventRaiserImplementationText":
                    AssignedConfigFileData.blueprintEventRaiserImplementation = nodeContent;
                    break;
                case "EventRaiserDeclarationText":
                    AssignedConfigFileData.blueprintEventRaiserDeclaration = nodeContent;
                    break;
                case "EventMethodDeclarationText":
                    AssignedConfigFileData.blueprintEventMethodDeclaration = nodeContent;
                    break;
                case "EventSenderMethodDeclarationText":
                    AssignedConfigFileData.blueprintSenderMethodDeclaration = nodeContent;
                    break;
                case "EventSenderMethodImplementationText":
                    AssignedConfigFileData.blueprintSenderMethodImplementation = nodeContent;
                    break;
                case "EventReceiverDeclarationText":
                    AssignedConfigFileData.blueprintReceiversDeclaration = nodeContent;
                    break;
                case "EventReceiverGetterImplementationText":
                    AssignedConfigFileData.blueprintReceiversGetterImplementation = nodeContent;
                    break;
                case "CheckForRaceConditionImplementationText":
                    AssignedConfigFileData.blueprintRaceConditionImplementation = nodeContent;
                    break;
                case "CheckForRaceConditionDeclarationText":
                    AssignedConfigFileData.blueprintRaceConditionDeclaration = nodeContent;
                    break;
                case "EventDispatcherImplementationText":
                    AssignedConfigFileData.blueprintEventDispatcherImplementation = nodeContent;
                    break;
                case "EventDispatcherDeclarationText":
                    AssignedConfigFileData.blueprintEventDispatcherDeclaration = nodeContent;
                    break;
                case "ExecuteHandlerDeclarationText":
                    AssignedConfigFileData.blueprintExecuteHandlerDeclaration = nodeContent;
                    break;
                case "ExecuteHandlerImplementationText":
                    AssignedConfigFileData.blueprintExecuteHandlerImplementation = nodeContent;
                    break;
                case "HandlerOperationText":
                    AssignedConfigFileData.blueprintHandlerOperation = nodeContent;
                    break;
                case "UpdateEventHistoryDeclarationText":
                    AssignedConfigFileData.blueprintUpdateEventHistoryDeclaration = nodeContent;
                    break;
                case "UpdateEventHistoryImplementationText":
                    AssignedConfigFileData.blueprintUpdateEventHistoryImplementation = nodeContent;
                    break;
                case "UpdateEventRecordDeclarationText":
                    AssignedConfigFileData.blueprintUpdateEventRecordDeclaration = nodeContent;
                    break;
                case "UpdateEventRecordImplementationText":
                    AssignedConfigFileData.blueprintUpdateEventRecordImplementation = nodeContent;
                    break;
                case "TriggerGvlDeclarationText":
                    AssignedConfigFileData.blueprintTriggerGvlDeclaration = nodeContent;
                    break;
                case "ModulesGvlDeclarationText":
                    AssignedConfigFileData.blueprintModulesGvlDeclaration = nodeContent;
                    break;
                case "ConstantsGvlDeclarationText":
                    AssignedConfigFileData.blueprintConstantsGvlDeclaration = nodeContent;
                    break;
                case "SystemGvlDeclarationText":
                    AssignedConfigFileData.blueprintSystemGvlDeclaration = nodeContent;
                    break;
                case "DisableEventManagerSignalDeclarationText":
                    AssignedConfigFileData.blueprintDisableEventManagerSignalDeclaration = nodeContent;
                    break;
                case "DisableEventManagerSignalSetterImplementationText":
                    AssignedConfigFileData.blueprintDisableEventManagerSignalSetterImplementation = nodeContent;
                    break;
                case "DisableEventManagerSignalGetterImplementationText":
                    AssignedConfigFileData.blueprintDisableEventManagerSignalGetterImplementation = nodeContent;
                    break;
                case "CallComponentsDeclarationText":
                    AssignedConfigFileData.blueprintCallComponentsDeclaration = nodeContent;
                    break;
                case "RegisterSequenceDeclarationText":
                    AssignedConfigFileData.blueprintRegisterSequenceDeclarationText = nodeContent;
                    break;
                case "RegisterSequenceImplementationText":
                    AssignedConfigFileData.blueprintRegisterSequenceImplementationText = nodeContent;
                    break;
                case "RunSequenceDeclarationText":
                    AssignedConfigFileData.blueprintRunSequenceDeclarationText = nodeContent;
                    break;
                case "RunSequenceImplementationText":
                    AssignedConfigFileData.blueprintRunSequenceImplementationText = nodeContent;
                    break;

                //--------------------------------------------------------------------------------------------------
                case "EventManagerFBInitDeclarationText":
                    AssignedConfigFileData.blueprintEventManagerFBInitDeclarationText = nodeContent;
                    break;
                case "EventManagerFBInitImplementationText":
                    AssignedConfigFileData.blueprintEventManagerFBInitImplementationText = nodeContent;
                    break;
                case "EventManagerOutStruct":
                    AssignedConfigFileData.blueprintEventManagerOutStruct = nodeContent;
                    break;
                case "EventManagerInStruct":
                    AssignedConfigFileData.blueprintEventManagerInStruct = nodeContent;
                    break;
                case "ModuleInStruct":
                    AssignedConfigFileData.blueprintModuleInStruct = nodeContent;
                    break;
                case "ModuleOutStruct":
                    AssignedConfigFileData.blueprintModuleOutStruct = nodeContent;
                    break;
            }
        }

    }
}
