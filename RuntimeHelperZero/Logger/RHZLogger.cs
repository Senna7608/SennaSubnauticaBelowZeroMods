using System;
using UnityEngine;

namespace RuntimeHelperZero.Logger
{
    public static class RHZLogger
    {
        public const string prefixLOG = "[RhZero [log] ";
        public const string prefixWARNING = "[RhZero [Warning] ";
        public const string prefixERROR = "[RhZero [Error] ";

        public static void RHZ_Log(string message)
        {
            Console.WriteLine(prefixLOG + message);
        }

        public static void RHZ_Log(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    RHZ_Error(message);
                    break;

                case LogType.Warning:
                    RHZ_Warning(message);
                    break;

                default:
                    RHZ_Log(message);
                    break;
            }
        }

        public static void RHZ_Log(string message, LogType type, params object[] args)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    RHZ_Error(message, args);
                    break;

                case LogType.Warning:
                    RHZ_Warning(message, args);
                    break;

                default:
                    RHZ_Log(message, args);
                    break;
            }
        }

        public static void RHZ_Log(string format, params object[] args)
        {
            Console.WriteLine(prefixLOG + string.Format(format, args));
        }

        public static void RHZ_Warning(string message)
        {
            Console.WriteLine(prefixWARNING + message);
        }

        public static void RHZ_Warning(string format, params object[] args)
        {
            Console.WriteLine(prefixWARNING + string.Format(format, args));
        }

        public static void RHZ_Error(string message)
        {
            Console.WriteLine(prefixERROR + message);
        }

        public static void RHZ_Error(string format, params object[] args)
        {
            Console.WriteLine(prefixERROR + string.Format(format, args));
        }
    }
}
