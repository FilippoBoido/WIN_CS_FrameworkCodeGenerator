using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using TwincatCodeGenerator.HelperClasses;

namespace TwincatCodeGenerator.ActivityClasses.ModulesCreation
{
    class GeneralEventsModuleCreator
    {
        Module module;

        ITcPlcDeclaration decl = (ITcPlcDeclaration)null;
        ITcPlcImplementation impl = (ITcPlcImplementation)null;

        ITcSmTreeItem pou = null;
        ITcSmTreeItem dut = null;
        ITcSmTreeItem gvl = null;
        ITcSmTreeItem modules = null;
        ITcSmTreeItem currentMethod = null;
        ITcSmTreeItem currentReceiver = null;
        ITcSmTreeItem currentSignal = null;
        ITcSmTreeItem triggers = null;
        ITcSmTreeItem eventManagers = null;
        ITcSmTreeItem disableSignal = null;
        ITcSmTreeItem currentDut = null;
        ITcSmTreeItem moduleConditionsToFire = null;
        ITcSmTreeItem moduleEventTriggers = null;
        ITcSmTreeItem eventManagerEventTriggers = null;
        ITcSmTreeItem eventManagerEventReceivers = null;

        string[] methodDescription = { "1", "BOOL" };
        string[] propertyDescription = { "1", "BOOL" };
        
        string outConditionsToFire = null;
        string outEventManagerEvents = null;
        string eventManagerEventsStrings = null;
        string eventDispatcherImplementation = null;
        string handlerOperations = null;
        string eventRaiserImplementation = null;
        string modulesGvlImplementation = null;
        string triggersImplementation = null;
        string enumEventDeclaration = null;
        string moduleEventManagerDeclaration = null;
        string moduleEventManagerImplementation = null;
        string moduleTriggerEventDeclaration = null;

        List<String[]> eventNamesLists;
        List<String> eventManagerNames;

        string keyVarGlobal = "VAR_GLOBAL";
        string keyVarOutput = "VAR_OUTPUT";
        string keyVarStruct = "STRUCT";

        int index = 0;
        int targetIndex = 0;
        public
        GeneralEventsModuleCreator(
            int targetIndex,
            ITcSmTreeItem pou,
            ITcSmTreeItem dut,
            ITcSmTreeItem gvl,
            ITcSmTreeItem eventManagers,
            ITcSmTreeItem modules,
            ITcSmTreeItem triggers
            
        )
        {
            this.eventManagers = eventManagers;
            this.modules = modules;
            this.triggers = triggers;    
            this.pou = pou;
            this.dut = dut;
            this.gvl = gvl;          
            module = AssignedConfigFileData.modulesContainer.content[targetIndex];
            eventNamesLists = new List<String[]>();
            eventManagerNames = new List<String>();
            this.targetIndex = targetIndex;
            CreateModuleCode();
            Console.WriteLine("Code for module {0} created.", module.getName);
            CreateEventManagerCode();
           
        }

        private void CreateModuleCode()
        {
            String eventManagerName;
            //-----------------------------------------------------------------------------------------------------------------------
            //---------------------------------Create module In-Out Structs  --------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------------

            moduleConditionsToFire = dut.CreateChild(
                   module.getName + "ConditionsToFire",
                   606,
                   "",
                   Assigner.ReplaceBlueprintValues(
                       AssignedConfigFileData.blueprintModuleOutStruct,
                       new string[] { "@moduleName@" },
                       new string[] { module.getName }
                   )
               );

            //Structure that defines the Event-Triggers that will be used as inputs in the Eventmanager
            moduleEventTriggers = dut.CreateChild(
                               module.getName + "EventTriggers",
                               606,
                               "",
                               Assigner.ReplaceBlueprintValues(
                                   AssignedConfigFileData.blueprintModuleInStruct,
                                   new string[] { "@moduleName@" },
                                   new string[] { module.getName }
                               )
                            );

            
            //-----------------------------------------------------------------------------------------------------------------------
            //--------------------------------- Fill the  ModulesGVL ----------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------------

            modulesGvlImplementation = "\t_" + module.getName + ": " + module.getName + ";" + "\n";
            decl = (ITcPlcDeclaration)modules;
            index = decl.DeclarationText.IndexOf(keyVarGlobal);
            decl.DeclarationText =
                decl.DeclarationText.Insert(
                                        index + (keyVarGlobal.Length + 1),
                                        modulesGvlImplementation
                                      );

            //-----------------------------------------------------------------------------------------------------------------------
            //------------------------------------------------Create the FB_TargetModule---------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------------

            //604   TREEITEMTYPE_PLCPOUFB   POU Function Block
            ITcSmTreeItem targetModule = pou.CreateChild(module.getName, 604, "1");

            //-----------------------------------------------------------------------------------------------------------------------
            //--------------------------- Create the FB_TargetModule Methods --------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------------
            moduleEventManagerDeclaration = null;
            eventNamesLists.Clear();

            foreach (EventManager eventManager in module.content)
            {
                eventNamesLists.Add(eventManager.content.eventNames);
                eventManagerNames.Add(eventManager.getName);
                moduleEventManagerDeclaration += Assigner.ReplaceBlueprintValues(
                                                            "\t" + AssignedConfigFileData.blueprintModuleEventManagerDeclaration + "\n",
                                                            new string[] { "@eventManagerName@" },
                                                            new string[] { eventManager.getName }
                                                          );

                moduleEventManagerImplementation += Assigner.ReplaceBlueprintValues(
                                                                AssignedConfigFileData.blueprintModuleEventManagerImplementation + "\n",
                                                                new string[] { "@eventManagerName@" },
                                                                new string[] { eventManager.getName }
                                                             );
            }

            int numberOfEvents = 0;
            foreach (String[] eventNamesGroup in eventNamesLists)
            {
                numberOfEvents += eventNamesGroup.Length;
                eventManagerName = eventManagerNames.First();
                eventManagerNames.RemoveAt(0);
                for(int i = 1; i <= eventNamesGroup.Length; i++)
                {
                    //Create EventMethods
                    currentMethod = targetModule.CreateChild(eventManagerName + i + "Event", 609, "", methodDescription);

                    decl = (ITcPlcDeclaration)currentMethod;
                    decl.DeclarationText = Assigner.ReplaceBlueprintValues(
                                                        AssignedConfigFileData.blueprintEventMethodDeclaration,
                                                        new string[] { "@eventName@" },
                                                        new string[] { eventManagerName + i.ToString() }
                                                    );

                    //Create SenderMethods
                    currentMethod = targetModule.CreateChild("fire" + eventManagerName + i + "Event", 609, "", methodDescription);

                    decl = (ITcPlcDeclaration)currentMethod;

                    decl.DeclarationText = Assigner.ReplaceBlueprintValues(
                                                        AssignedConfigFileData.blueprintSenderMethodDeclaration,
                                                        new string[] { "@eventName@" },
                                                        new string[] { eventManagerName + i.ToString() }
                                                    );

                    impl = (ITcPlcImplementation)currentMethod;
                    impl.ImplementationText = Assigner.ReplaceBlueprintValues(
                                                            AssignedConfigFileData.blueprintSenderMethodImplementation,
                                                            new string[] { "@eventName@", "@eventManagerName@","@eventNumber@" },
                                                            new string[] { eventManagerName + i.ToString(), eventManagerName , i.ToString()}
                                                       );

                    outConditionsToFire += "\t" + eventManagerName + i + "ConditionsToFire : BOOL;\n";

                    eventRaiserImplementation += Assigner.ReplaceBlueprintValues(
                                                            AssignedConfigFileData.blueprintEventRaiserImplementation + "\n",
                                                            new string[] { "@eventName@" },
                                                            new string[] { eventManagerName + i.ToString() }
                                                          );
                    /*
                    moduleTriggerEventDeclaration += Assigner.ReplaceBlueprintValues(
                                                                "\t" + AssignedConfigFileData.blueprintModuleTriggerEventsDeclaration + "\n",
                                                                new string[] { "@eventName@" },
                                                                new string[] { i.ToString() }
                                                              );
                    */
                }

            }

            moduleTriggerEventDeclaration = Assigner.ReplaceBlueprintValues(
                                                        "\t" + AssignedConfigFileData.blueprintModuleTriggerEventsDeclaration + "\n",
                                                        new string[] { "@numberOfEvents@" },
                                                        new string[] { numberOfEvents.ToString() }
                                                    );
            //Fill the module structures with the relevant data
            //--------------------------------------------------------------------------------
            decl = (ITcPlcDeclaration)moduleEventTriggers;

            index = decl.DeclarationText.IndexOf(keyVarStruct);
            decl.DeclarationText = decl.DeclarationText.Insert(
                                            index + (keyVarStruct.Length + 1),
                                            moduleTriggerEventDeclaration
                                        );

            decl = (ITcPlcDeclaration)moduleConditionsToFire;

            index = decl.DeclarationText.IndexOf(keyVarStruct);
            decl.DeclarationText = decl.DeclarationText.Insert(
                                            index + (keyVarStruct.Length + 1),
                                            outConditionsToFire
                                        );
            //--------------------------------------------------------------------------------

            //eventRaiser method
            currentMethod = targetModule.CreateChild("eventRaiser", 609, "", methodDescription);

            
            decl = (ITcPlcDeclaration)currentMethod;
            decl.DeclarationText = AssignedConfigFileData.blueprintEventRaiserDeclaration;

            impl = (ITcPlcImplementation)currentMethod;
            impl.ImplementationText = eventRaiserImplementation;

            //callComponents method
            currentMethod = targetModule.CreateChild("callComponents", 609, "", methodDescription);

    
            decl = (ITcPlcDeclaration)currentMethod;
            decl.DeclarationText = AssignedConfigFileData.blueprintCallComponentsDeclaration;

            //registerSequence method
            currentMethod = targetModule.CreateChild("registerSequence", 609, "", methodDescription);

            
            decl = (ITcPlcDeclaration)currentMethod;
            decl.DeclarationText = AssignedConfigFileData.blueprintRegisterSequenceDeclarationText;

            impl = (ITcPlcImplementation)currentMethod;
            impl.ImplementationText = AssignedConfigFileData.blueprintRegisterSequenceImplementationText;

            //runSequence method
            currentMethod = targetModule.CreateChild("runSequence", 609, "", methodDescription);

            
            decl = (ITcPlcDeclaration)currentMethod;
            decl.DeclarationText = AssignedConfigFileData.blueprintRunSequenceDeclarationText;

            impl = (ITcPlcImplementation)currentMethod;
            impl.ImplementationText = AssignedConfigFileData.blueprintRunSequenceImplementationText;

            //-----------------------------------------------------------------------------------------------------------------------
            //-------------------------- Fill the declaration and implementation part of FB_TargetModule with code-------------------
            //-----------------------------------------------------------------------------------------------------------------------

            decl = (ITcPlcDeclaration)targetModule;
            impl = (ITcPlcImplementation)targetModule;

            decl.DeclarationText = Assigner.ReplaceBlueprintValues(
                                        AssignedConfigFileData.modulesDeclarations[targetIndex],
                                        new string[] {"@eventManagers", "@moduleName@"  },
                                        new string[] { moduleEventManagerDeclaration, module.getName }
                                    );

            impl.ImplementationText = Assigner.ReplaceBlueprintValues(
                                        AssignedConfigFileData.modulesImplementations[targetIndex],
                                        new string[] { "@eventManagers" },
                                        new string[] { moduleEventManagerImplementation }
                                    ); 
        }

        private void CreateEventManagerCode()
        {
            //-----------------------------------------------------------------------------------------------------------------------
            //------------------------------------------------Create the FB_EventManager---------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------------
            int numberOfEvents = 0;
            foreach (EventManager eventManager in module.content)
            {
                ITcSmTreeItem fbEventManager = eventManagers.CreateChild("FB_" + eventManager.getName + "EventManager", 604, "1");

                String[] eventNameGroup = eventManager.content.eventNames;
                
                outEventManagerEvents = null;
                eventManagerEventsStrings = null;
                eventDispatcherImplementation = null;
                handlerOperations = null;
                enumEventDeclaration = null;
                triggersImplementation = null;
                for (int i = 1; i <= eventNameGroup.Length; i++)
                {
                    outEventManagerEvents += "\t_" + i + "Event : BOOL;\n";

                    if(i != eventNameGroup.Length)
                        eventManagerEventsStrings += "\t\t_" + i + "Event',\n";
                    else
                        eventManagerEventsStrings += "\t\t_" + i + "Event'\n";

                    eventDispatcherImplementation += Assigner.ReplaceBlueprintValues(
                                                            "\n" + AssignedConfigFileData.blueprintEventDispatcherImplementation,
                                                            new string[] { "@eventManagerName@", "@eventName@" },
                                                            new string[] { eventManager.getName, i.ToString() }
                                                        );

                    handlerOperations += Assigner.ReplaceBlueprintValues(
                                                AssignedConfigFileData.blueprintHandlerOperation + "\n",
                                                new string[] { "@eventManagerName@", "@eventName@", "@moduleName@" , "@eventManagerEventName@"},
                                                new string[] { eventManager.getName, i.ToString(), module.getName, eventManager.getName + i.ToString() }
                                            );

                    enumEventDeclaration += "\t" + i + "Event := "+ i + numberOfEvents +",\n";

                    triggersImplementation += "\t" + i + "RTrigger : R_TRIG;" + "\n";
                    //-----------------------------------------------------------------------------------------------------------------------
                    //--------------------------- Create the FB_EventManager eventReceivers -------------------------------------------------
                    //-----------------------------------------------------------------------------------------------------------------------

                    //Implement receivers
                    currentReceiver = fbEventManager.CreateChild("_" + i + "Receiver", 611, "", propertyDescription);

                    decl = (ITcPlcDeclaration)currentReceiver;
                    decl.DeclarationText = Assigner.ReplaceBlueprintValues(
                                                    AssignedConfigFileData.blueprintReceiversDeclaration,
                                                    new string[] { "@eventName@" },
                                                    new string[] { i.ToString() }
                                                );

                    //Implement getters
                    currentReceiver = currentReceiver.CreateChild("", 613, "", "1");

                    impl = (ITcPlcImplementation)currentReceiver;
                    impl.ImplementationText = Assigner.ReplaceBlueprintValues(
                                                    AssignedConfigFileData.blueprintReceiversGetterImplementation,
                                                    new string[] { "@eventManagerName@", "@eventName@" },
                                                    new string[] { eventManager.getName, i.ToString() }
                                                );
                }
                numberOfEvents += eventNameGroup.Length;

                //-----------------------------------------------------------------------------------------------------------------------
                //-------------------------- Create the  E_<X>Event Enumaration ---------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------

                //605   TREEITEMTYPE_PLCDUTENUM     DUT enum data type
                dut.CreateChild(
                    eventManager.getName + "Event", 
                    605, "", 
                    Assigner.ReplaceBlueprintValues(
                        AssignedConfigFileData.blueprintEnumEventDeclaration,
                        new string[] { "@eventManagerName@", "@eventNames" },
                        new string[] { eventManager.getName, enumEventDeclaration }
                    )
                );

                //-----------------------------------------------------------------------------------------------------------------------
                //-------------------------- Create the  EventManager-Structures ----------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
           
                dut.CreateChild(
                    eventManager.getName + "EventRecord",
                    606,
                    "",
                    Assigner.ReplaceBlueprintValues(
                        AssignedConfigFileData.blueprintEventRecordDeclaration,
                        new string[] { "@eventManagerName@" },
                        new string[] { eventManager.getName }
                    )
                );

                //Structure that defines the Event-Triggers that will be used as inputs in the Eventmanager
                eventManagerEventTriggers = dut.CreateChild(
                                               eventManager.getName + "EventTriggers",
                                               606,
                                               "",
                                               Assigner.ReplaceBlueprintValues(
                                                   AssignedConfigFileData.blueprintEventManagerInStruct,
                                                   new string[] { "@eventManagerName@" },
                                                   new string[] { eventManager.getName }
                                               )
                                            );

                decl = (ITcPlcDeclaration)eventManagerEventTriggers;
  
                index = decl.DeclarationText.IndexOf(keyVarStruct);
                decl.DeclarationText = decl.DeclarationText.Insert(
                                                index + (keyVarStruct.Length + 1),
                                                triggersImplementation
                                            );

                eventManagerEventReceivers =    dut.CreateChild(
                                                    eventManager.getName + "EventReceiver",
                                                    606,
                                                    "",
                                                    Assigner.ReplaceBlueprintValues(
                                                        AssignedConfigFileData.blueprintEventManagerOutStruct,
                                                        new string[] { "@eventManagerName@" },
                                                        new string[] { eventManager.getName }
                                                    )
                                                );

                decl = (ITcPlcDeclaration)eventManagerEventReceivers;
                index = decl.DeclarationText.IndexOf(keyVarStruct);
                decl.DeclarationText = decl.DeclarationText.Insert(
                                                index + (keyVarStruct.Length + 1),
                                                outEventManagerEvents
                                            );
                //-----------------------------------------------------------------------------------------------------------------------
                //-------------------------- Fill the declaration and implementation part of FB_EventManager with code-------------------
                //-----------------------------------------------------------------------------------------------------------------------
                decl = (ITcPlcDeclaration)fbEventManager;
                impl = (ITcPlcImplementation)fbEventManager;

                decl.DeclarationText =  Assigner.ReplaceBlueprintValues(
                                            AssignedConfigFileData.blueprintEventManagerDeclaration,
                                            new string[] { "@eventManagerName@", "@eventStrings","@moduleName@" },
                                            new string[] { eventManager.getName,  eventManagerEventsStrings,module.getName }
                                        );

                
                impl.ImplementationText = AssignedConfigFileData.blueprintEventManagerImplementation;

                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------- Create the FB_EventManager Methods --------------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------

                //M_CheckForRaceCondition
                currentMethod = fbEventManager.CreateChild("checkForRaceCondition", 609, "", methodDescription);

                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = AssignedConfigFileData.blueprintRaceConditionDeclaration;

                impl = (ITcPlcImplementation)currentMethod;
                impl.ImplementationText =   Assigner.ReplaceBlueprintValues(
                                                AssignedConfigFileData.blueprintRaceConditionImplementation,
                                                new string[] { "@eventManagerName@" },
                                                new string[] { eventManager.getName }
                                            );

                //M_EventDispatcher
                currentMethod = fbEventManager.CreateChild("eventDispatcher", 609, "", methodDescription);

                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = AssignedConfigFileData.blueprintEventDispatcherDeclaration;

                impl = (ITcPlcImplementation)currentMethod;
                impl.ImplementationText = eventDispatcherImplementation;

                //M_ExecuteHandler
                currentMethod = fbEventManager.CreateChild("executeHandler", 609, "", methodDescription);

                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = AssignedConfigFileData.blueprintExecuteHandlerDeclaration;

                impl = (ITcPlcImplementation)currentMethod;
                impl.ImplementationText = Assigner.ReplaceBlueprintValues(
                                                AssignedConfigFileData.blueprintExecuteHandlerImplementation,
                                                new string[] { "@handlerOperations" },
                                                new string[] { handlerOperations }
                                          );
                
                //M_UpdateEventHistory
                currentMethod = fbEventManager.CreateChild("updateEventHistory", 609, "", methodDescription);

                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = AssignedConfigFileData.blueprintUpdateEventHistoryDeclaration;
                
                
                impl = (ITcPlcImplementation)currentMethod;

                impl.ImplementationText = AssignedConfigFileData.blueprintUpdateEventHistoryImplementation;
               
                //M_UpdateEventRecord
                currentMethod = fbEventManager.CreateChild("updateEventRecord", 609, "", methodDescription);

                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = AssignedConfigFileData.blueprintUpdateEventRecordDeclaration;

                impl = (ITcPlcImplementation)currentMethod;
                impl.ImplementationText = Assigner.ReplaceBlueprintValues(
                                                AssignedConfigFileData.blueprintUpdateEventRecordImplementation,
                                                new string[] { "@unequal" },
                                                new string[] { "<>" }
                                          );
                //FB_Init
                currentMethod = fbEventManager.CreateChild("FB_init", 609, "", methodDescription);
                decl = (ITcPlcDeclaration)currentMethod;
                decl.DeclarationText = Assigner.ReplaceBlueprintValues(
                                                AssignedConfigFileData.blueprintEventManagerFBInitDeclarationText,
                                                new string[] { "@moduleName@" },
                                                new string[] { module.getName }
                                          );
                impl = (ITcPlcImplementation)currentMethod;
                impl.ImplementationText = Assigner.ReplaceBlueprintValues(
                                                AssignedConfigFileData.blueprintEventManagerFBInitImplementationText,
                                                new string[] { "@moduleName@" },
                                                new string[] { module.getName }
                                          );
                //-----------------------------------------------------------------------------------------------------------------------
                //--------------------------- Create the FB_EventManager disable signal -------------------------------------------------
                //-----------------------------------------------------------------------------------------------------------------------
                disableSignal =
                    fbEventManager.
                        CreateChild("disableEventManagerSignal", 611, "", propertyDescription);

                decl = (ITcPlcDeclaration)disableSignal;
                decl.DeclarationText = AssignedConfigFileData.blueprintDisableEventManagerSignalDeclaration;

                //Implement getter
                currentSignal = disableSignal.CreateChild("", 613, "", "1");
                impl = (ITcPlcImplementation)currentSignal;
                impl.ImplementationText = AssignedConfigFileData.blueprintDisableEventManagerSignalGetterImplementation;

                //Implement setter
                currentSignal = disableSignal.CreateChild("", 614, "", "1");
                impl = (ITcPlcImplementation)currentSignal;
                impl.ImplementationText = AssignedConfigFileData.blueprintDisableEventManagerSignalSetterImplementation;

                Console.WriteLine("Code for " + eventManager.getName + "EventManager" + " created.");       
            
            }
            
        }
    }
}
