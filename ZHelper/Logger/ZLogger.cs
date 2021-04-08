using System;
using UnityEngine;

namespace ZHelper.Logger
{
    public static class ZLogger
    {
        public const string prefixLOG = "[ZHelper [log] ";
        public const string prefixWARNING = "[ZHelper [Warning] ";
        public const string prefixERROR = "[ZHelper [Error] ";

        public static void Log(string message)
        {
            Console.WriteLine(prefixLOG + message);
        }

        public static void Log(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    Error(message);
                    break;

                case LogType.Warning:
                    Warning(message);
                    break;

                default:
                    Log(message);
                    break;
            }
        }

        public static void Log(string message, LogType type, params object[] args)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    Error(message, args);
                    break;

                case LogType.Warning:
                    Warning(message, args);
                    break;

                default:
                    Log(message, args);
                    break;
            }
        }

        public static void Log(string format, params object[] args)
        {
            Console.WriteLine(prefixLOG + string.Format(format, args));
        }

        public static void Warning(string message)
        {
            Console.WriteLine(prefixWARNING + message);
        }

        public static void Warning(string format, params object[] args)
        {
            Console.WriteLine(prefixWARNING + string.Format(format, args));
        }

        public static void Error(string message)
        {
            Console.WriteLine(prefixERROR + message);
        }

        public static void Error(string format, params object[] args)
        {
            Console.WriteLine(prefixERROR + string.Format(format, args));
        }
    }
}
