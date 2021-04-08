#define DEBUG_GAMELOG
//#define DEBUGTOFILE
#define AUTOSCROLL
//#define MAXMESSAGE_INFINITY

using System.Collections.Generic;
using UnityEngine;
using CheatManagerZero.Configuration;
using BZCommon.Helpers.GUIHelper;

#if DEBUGTOFILE
using System.IO;
using System;
#endif

namespace CheatManagerZero
{
    public class CMZ_Console : MonoBehaviour
    {        
        private static GUIStyle logStyle;
        //private bool setStyle = false;

        private static Rect windowRect = new Rect(Screen.width - (Screen.width / CmZConfig.ASPECT), Screen.height - (Screen.height / 4), Screen.width / CmZConfig.ASPECT, Screen.height / 4);
        private static Rect buttonRect = new Rect(windowRect.x + 5, windowRect.y + windowRect.height - 27, windowRect.width - 10, 22);
        private Rect drawRect, scrollRect;
        private float scrollWidth;

#if DEBUGTOFILE
        private readonly string LogFilePath = Environment.CurrentDirectory + "/CheatManagerZero_log.txt";
#endif
        private Vector2 scrollPos = Vector2.zero;
        private float contentHeight = 0;
        private float drawingPos;
        private List<LOG> logMessage = new List<LOG>();
        private int messageCount = 0;

        private bool show = false;
        //private string inputField = string.Empty;

#if MAXMESSAGE_INFINITY
        private static readonly int MAXLOG = int.MaxValue;
#else
        private readonly int MAXLOG = 100; 
#endif
        //private static List<string> history = new List<string>();

        //private static int historyIndex = 0;
        
        private struct LOG
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }
        
        private readonly Dictionary<LogType, GuiColor> logTypeColors = new Dictionary<LogType, GuiColor>()
        {
            { LogType.Error, GuiColor.Magenta },
            { LogType.Assert, GuiColor.Blue },
            { LogType.Warning, GuiColor.Yellow },            
            { LogType.Log, GuiColor.Green },
            { LogType.Exception, GuiColor.Red },            

        };        
         
        public void Awake()
        {
#if DEBUG
            show = true;
#endif                        
            DontDestroyOnLoad(this);            
            useGUILayout = false;

            drawRect = SNWindow.InitWindowRect(windowRect);
            scrollRect = new Rect(drawRect.x, drawRect.y + 5, drawRect.width - 5, drawRect.height - 37);
            scrollWidth = scrollRect.width - 40;
#if DEBUGTOFILE

            if (!File.Exists(LogFilePath))
            {
                try
                {
                    File.Delete(LogFilePath);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
#endif
            Application.logMessageReceived += HandleLog;            
        }

#if DEBUGTOFILE
        public void WriteToFile(string message)
        {
            try
            {
                StreamWriter fileWriter = new StreamWriter(LogFilePath, true);

                fileWriter.WriteLine(message);
                fileWriter.Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
#endif




        public void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
            logMessage.Clear();
            Destroy(this);
        }

        void OnGUI()
        {             
            if (!show)            
                return;
            
            logStyle = SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, TextAnchor.MiddleLeft, wordWrap: true);

            SNWindow.CreateWindow(windowRect, $"CheatManagerZero Console (Press {CmZConfig.KEYBINDINGS["ToggleConsole"]} to toggle)", true, true);

            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, new Rect(scrollRect.x, scrollRect.y, scrollWidth, drawingPos - scrollRect.y));

            for (int i = 0; i < logMessage.Count; i++)
            {
                if (i == 0)
                {
                    drawingPos = scrollRect.y;
                }

                contentHeight = logStyle.CalcHeight(new GUIContent(logMessage[i].message), scrollWidth);

                logStyle.normal.textColor = SNStyles.GetGuiColor(logTypeColors[logMessage[i].type]);

                GUI.Label(new Rect(scrollRect.x + 5, drawingPos, 15, 21), "> ", logStyle);
                GUI.Label(new Rect(scrollRect.x + 20, drawingPos, scrollWidth, contentHeight), logMessage[i].message, logStyle);

                drawingPos += contentHeight + 1;

                if (logMessage[i].stackTrace != "")
                {
                    contentHeight = logStyle.CalcHeight(new GUIContent(logMessage[i].stackTrace), scrollWidth);
                    logStyle.normal.textColor = SNStyles.GetGuiColor(logTypeColors[logMessage[i].type]);
                    GUI.Label(new Rect(scrollRect.x + 20, drawingPos, scrollWidth, contentHeight), logMessage[i].stackTrace, logStyle);
                    drawingPos += contentHeight + 1;
                }
            }

#if AUTOSCROLL
            if (messageCount != logMessage.Count)
            {
                scrollPos.y += Mathf.Infinity;
                messageCount = logMessage.Count;
            }
#endif
            GUI.EndScrollView();

            /*
            if (Event.current.Equals(Event.KeyboardEvent("return")) && inputField != "")
            {
                history.Add(inputField);
                historyIndex = history.Count;
                Log(inputField);                
                DevConsole.SendConsoleCommand(inputField);
                inputField = "";
            }
            
            if (Event.current.Equals(Event.KeyboardEvent("up")))
            {
                if (history.Count > 0 && historyIndex >= 1)
                {
                    historyIndex--;
                    inputField = history[historyIndex];
                }
            }

            if (Event.current.Equals(Event.KeyboardEvent("down")))
            {
                if (history.Count > 0 && historyIndex < history.Count - 1)
                {
                    historyIndex++;
                    inputField = history[historyIndex];
                }
            }
            
            inputField = GUI.TextField(new Rect(scrollRect.x + 5, scrollRect.y + scrollRect.height + 5, 300, 22), inputField);
            */

            if (GUI.Button(buttonRect, "Clear Window"))
            {
                logMessage.Clear();
                drawingPos = scrollRect.y;
            }
        }            
            
        public void Update()
        {
            if (Input.GetKeyDown(CmZConfig.KEYBINDINGS["ToggleConsole"]))
            {
                show = !show;                
            }
        }        

        private void Write(string message)
        {
            logMessage.Add(new LOG()
            {
                message = message,
                stackTrace = "",
                type = LogType.Log,
            });
            
            if (logMessage.Count == MAXLOG)
            {
                logMessage.RemoveAt(0);
                messageCount--;
            }
        }

        private void Write(string message, LogType type)
        {
            logMessage.Add(new LOG()
            {
                message = message,
                stackTrace = "",
                type = type,
            });

            if (logMessage.Count == MAXLOG)
            {
                logMessage.RemoveAt(0);
                messageCount--;
            }
        }        

        private void Write(string message, LogType type, params object[] arg)
        {
            logMessage.Add(new LOG()
            {
                message = string.Format(message, arg),
                stackTrace = "",
                type = type,
            });

            if (logMessage.Count == MAXLOG)
            {
                logMessage.RemoveAt(0);
                messageCount--;
            }
        }

        private void Write(string message, string stacktrace, LogType type)
        {            
            if (stacktrace != "")
            {
                string temp;
                temp = "<<STACKTRACE>>\n" + stacktrace;
                stacktrace = temp;
            }

            logMessage.Add(new LOG()
            {
                message = message,
                stackTrace = stacktrace,
                type = type,
            });

            if (logMessage.Count == MAXLOG)
            {
                logMessage.RemoveAt(0);
                messageCount--;
            }

        }

        public void Log(string message) => Write(message);

        public void Log(string message, LogType type) => Write(message, type);

        public void Log(string message, LogType type, params object[] arg) => Write(message, type, arg);

        public void HandleLog(string message, string stacktrace, LogType type)
        {
            Write(message, stacktrace, type);

#if DEBUGTOFILE
            WriteToFile(message);
#endif
        }
    }
}