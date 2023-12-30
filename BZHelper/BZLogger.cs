using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum BZLogType
    {
        LOG,
        WARNING,
        ERROR,
        DEBUG,
        TRACE
    }

    public enum PatchPriority
    {
        Last = 0,
        VeryLow = 100,
        Low = 200,
        LowerThanNormal = 300,
        Normal = 400,
        HigherThanNormal = 500,
        High = 600,
        VeryHigh = 700,
        First = 800
    }

    public static class BZLogger
    {
        public static readonly Dictionary<BZLogType, string> logTypeCache = new Dictionary<BZLogType, string>(4)
        {
            { BZLogType.LOG, "LOG" },
            { BZLogType.WARNING, "WARNING" },
            { BZLogType.ERROR, "ERROR" },
            { BZLogType.DEBUG, "DEBUG" },
            { BZLogType.TRACE, "TRACE" },
        };

        private static void WriteLog(BZLogType logType, string message)
        {            
            Console.WriteLine($"[{Assembly.GetCallingAssembly().GetName().Name}/{logTypeCache[logType]}] {message}");
        }

        public static void UnityLog(string message) => UnityEngine.Debug.Log(message);

        public static void Log(string message) => WriteLog(BZLogType.LOG, message);

        public static void Log(string format, params object[] args) => WriteLog(BZLogType.LOG, string.Format(format, args));

        public static void Warn(string moduleName, string message) => WriteLog(BZLogType.WARNING, message);

        public static void Warn(string format, params object[] args) => WriteLog(BZLogType.WARNING, string.Format(format, args));

        public static void Error(string message) => WriteLog(BZLogType.ERROR, message);

        public static void Error(string format, params object[] args) => WriteLog(BZLogType.ERROR, string.Format(format, args));

        [Conditional("DEBUG")]
        public static void Debug(string message) => WriteLog(BZLogType.DEBUG, message);

        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args) => WriteLog(BZLogType.DEBUG, string.Format(format, args));

        [Conditional("DEBUG")]
        internal static void DebugIL(CodeInstruction cin, string logPrefix, int ilRow = -1)
        {              
            WriteLog(BZLogType.DEBUG, $"{logPrefix} {((ilRow != -1)? $"IL row:  {ilRow}" : "")} OpCode: {cin.opcode.Name}, Operand: [{cin.operand}]");            
        }

        [Conditional("DEBUG")]
        internal static void DebugILs(IEnumerable<CodeInstruction> cins, string logPostfix)
        {
            WriteLog(BZLogType.DEBUG, $"DebugILs: listing {logPostfix} IL code...");

            foreach (CodeInstruction cin in cins)
            {               
                WriteLog(BZLogType.DEBUG, $"OpCode: {cin.opcode.Name}, Operand: [{cin.operand}]"); 
            }
        }

        [Conditional("DEBUG")]
        internal static void DebugHarmony(Type instance , string method, string patchTarget, HarmonyPatchType patchType, PatchPriority patchPriority = PatchPriority.Normal)
        { 
          WriteLog(BZLogType.DEBUG, $"DebugHarmony: class: {instance.Name},  method: {method},  target: {patchTarget},  type: {patchType},  priority: {patchPriority}");            
        }

        [Conditional("TRACE")]
        public static void Trace(string message) => WriteLog(BZLogType.TRACE, message);

        [Conditional("TRACE")]
        public static void Trace(string format, params object[] args) => WriteLog(BZLogType.TRACE, string.Format(format, args));
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
