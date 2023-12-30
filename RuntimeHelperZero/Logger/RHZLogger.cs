using System;
using System.Diagnostics;
using UnityEngine;

namespace RuntimeHelperZero.Logger
{
    public enum RHZLogType
    {
        Log = 0,        
        Warning = 2,
        Error = 3,
        Debug = 4
    }

    public static class RHZLogger
    {
        public const string prefixLOG = "[RhZero [Log] ";
        public const string prefixWARNING = "[RhZero [Warning] ";
        public const string prefixERROR = "[RhZero [Error] ";
        public const string prefixDEBUG = "[RhZero [Debug] ";

        public static void Log(string message)
        {
            //UnityEngine.Debug.Log(prefixLOG + message);
            Console.WriteLine(prefixLOG + message);
        }

        public static void Log(string message, RHZLogType type)
        {
            switch (type)
            {
                case RHZLogType.Warning:
                    Warning(message);
                    break;

                case RHZLogType.Error:                
                    Error(message);
                    break;

                case RHZLogType.Debug:
                    Debug(message);
                    break;

                default:
                    Log(message);
                    break;
            }
        }

        public static void Log(string message, RHZLogType type, params object[] args)
        {
            switch (type)
            {
                case RHZLogType.Warning:
                    Warning(message, args);
                    break;

                case RHZLogType.Error:
                    Error(message, args);
                    break;                

                case RHZLogType.Debug:
                    Debug(message, args);
                    break;

                default:
                    Log(message, args);
                    break;
            }
        }

        public static void Log(string format, params object[] args)
        {
            UnityEngine.Debug.Log(prefixLOG + string.Format(format, args));
            //Console.WriteLine(prefixLOG + string.Format(format, args));
        }

        public static void Warning(string message)
        {
            UnityEngine.Debug.Log(prefixWARNING + message);
            //Console.WriteLine(prefixWARNING + message);
        }

        public static void Warning(string format, params object[] args)
        {
            UnityEngine.Debug.Log(prefixWARNING + string.Format(format, args));
            //Console.WriteLine(prefixWARNING + string.Format(format, args));
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.Log(prefixERROR + message);
            //Console.WriteLine(prefixERROR + message);
        }

        public static void Error(string format, params object[] args)
        {
            UnityEngine.Debug.Log(prefixERROR + string.Format(format, args));
            //Console.WriteLine(prefixERROR + string.Format(format, args));
        }

        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            UnityEngine.Debug.Log(prefixDEBUG + string.Format(format, args));
            //Console.WriteLine(prefixDEBUG + string.Format(format, args));
        }
    }
}
