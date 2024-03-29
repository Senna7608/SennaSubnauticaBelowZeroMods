﻿using System.Collections.Generic;
using UnityEngine;
using CheatManagerZero.Configuration;
using BZCommon.Helpers.GUIHelper;

namespace CheatManagerZero
{
    public class CMZ_Logger : MonoBehaviour
    {        
        private static GUIStyle logStyle;        

        private static Rect windowRect = new Rect(Screen.width - (Screen.width / CMZ_Config.ASPECT), Screen.height - (Screen.height / 4), Screen.width / CMZ_Config.ASPECT, Screen.height / 4);
        private static Rect buttonRect = new Rect(windowRect.x + 5, windowRect.y + windowRect.height - 27, windowRect.width - 10, 22);
        private Rect drawRect, scrollRect;
        private float scrollWidth;

        private Vector2 scrollPos = Vector2.zero;
        private float contentHeight = 0;
        private float drawingPos;
        private List<LOG> logMessage = new List<LOG>();
        private int messageCount = 0;

        private bool show = false;

        private readonly int MAXLOG = 100;         
        
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
            DontDestroyOnLoad(this);            
            useGUILayout = false;

            drawRect = SNWindow.InitWindowRect(windowRect);
            scrollRect = new Rect(drawRect.x, drawRect.y + 5, drawRect.width - 5, drawRect.height - 37);
            scrollWidth = scrollRect.width - 40;

            Application.logMessageReceived += HandleLog;
            show = CMZ_Config.isConsoleEnabled;
        }

        public void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
            logMessage.Clear();
            Destroy(this);
        }

        void OnGUI()
        {
            if (!CMZ_Config.isConsoleEnabled)
            {
                return;
            }

            if (!show)            
                return;
            
            logStyle = SNStyles.GetGuiItemStyle(GuiItemType.LABEL, GuiColor.Green, TextAnchor.MiddleLeft, wordWrap: true);

            SNWindow.CreateWindow(windowRect, $"CheatManagerZero Console (Press {CMZ_Config.KEYBINDINGS["ToggleConsole"]} to toggle)", true, true);

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

            if (messageCount != logMessage.Count)
            {
                scrollPos.y += Mathf.Infinity;
                messageCount = logMessage.Count;
            }

            GUI.EndScrollView();

            if (GUI.Button(buttonRect, "Clear Window"))
            {
                logMessage.Clear();
                drawingPos = scrollRect.y;
            }
        }            
            
        public void Update()
        {
            if (!CMZ_Config.isConsoleEnabled)
            {
                return;
            }

            if (Input.GetKeyDown(CMZ_Config.KEYBINDINGS["ToggleConsole"]))
            {
                show = !show;                
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

        public void HandleLog(string message, string stacktrace, LogType type)
        {
            Write(message, stacktrace, type);
        }
    }
}