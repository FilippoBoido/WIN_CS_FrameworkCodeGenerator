using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;

namespace TwincatCodeGenerator
{
    class DteAttacher
    {
        [DllImport("ole32.dll")]
        private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern void GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        public static Hashtable GetRunningObjectTable()
        {
            Hashtable result = new Hashtable();
            System.IntPtr numFetched = (System.IntPtr)0;
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];
            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();
            while (monikerEnumerator.Next(1, monikers, numFetched) == 0)
            {
                IBindCtx ctx;
                CreateBindCtx(0, out ctx);
                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);
                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);
                result[runningObjectName] = runningObjectVal;
            }
            return result;
        }

        public static Hashtable GetIDEInstances(bool openSolutionsOnly, string progId)
        {
            Hashtable runningIDEInstances = new Hashtable();
            Hashtable runningObjects = GetRunningObjectTable();
            IDictionaryEnumerator rotEnumerator = runningObjects.GetEnumerator();
            while (rotEnumerator.MoveNext())
            {
                string candidateName = (string)rotEnumerator.Key;
                if (!candidateName.StartsWith("!" + progId))
                    continue;
                EnvDTE.DTE ide = rotEnumerator.Value as EnvDTE.DTE;
                if (ide == null)
                    continue;
                if (openSolutionsOnly)
                {
                    try
                    {
                        string solutionFile = ide.Solution.FullName;
                        if (solutionFile != String.Empty)
                            runningIDEInstances[candidateName] = ide;
                    }
                    catch { }
                }
                else
                    runningIDEInstances[candidateName] = ide;
            }
            return runningIDEInstances;
        }

        public static EnvDTE.DTE attachToExistingDte(string solutionPath, string progId, string nameOfProject)
        {
            Console.WriteLine("@attachToExistingDte: solution path: {0}, progId: {1}", solutionPath, progId);
            EnvDTE.DTE dte = null;
            Hashtable dteInstances = GetIDEInstances(false, progId);
            IDictionaryEnumerator hashtableEnumerator = dteInstances.GetEnumerator();
            String solutionName;
            while (hashtableEnumerator.MoveNext())
            {
                EnvDTE.DTE dteTemp = hashtableEnumerator.Value as EnvDTE.DTE;
                solutionName = dteTemp.Solution.FullName.ToLower();
                solutionPath = solutionPath.ToLower();
                if (solutionPath.Equals(solutionName) )
                {
                    Console.WriteLine("Found solution in list of all open DTE objects. " + dteTemp.Name);
                    dte = dteTemp;
                }

            }
            if (dte == null)
            {
                Console.WriteLine("No solution found with following solution path: {0}", solutionPath);
                Console.Read();
                Environment.Exit(0);
            }
            return dte;
        }
    }
}
