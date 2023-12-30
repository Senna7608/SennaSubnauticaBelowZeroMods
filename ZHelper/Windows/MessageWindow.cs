using BZCommon.Helpers.RuntimeGUI;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ZHelper.Logger;

namespace ZHelper
{
    public enum MessageType
    {
        Log,
        Warning,
        Error,
        Debug
    }

    public partial class ZHelper // Message Window
    {
        private Rect messageWindowRect = new Rect(0, 860, 600, 220);

        private List<MSG> messages = new List<MSG>();

        private List<GUI_content> MessageScrollContents = new List<GUI_content>();

        private readonly int MAXMSG = 100;

        private int messageNum = 0;

        private class MSG
        {
            public string message;
            public MessageType type;
        }

        private readonly Dictionary<MessageType, GUI_Color> msgTypeColors = new Dictionary<MessageType, GUI_Color>()
        {
            { MessageType.Log, GUI_Color.Green },
            { MessageType.Warning, GUI_Color.Red },
            { MessageType.Error, GUI_Color.Red },
            { MessageType.Debug, GUI_Color.Yellow }
        };

        private void MessageWindow_Start()
        {
            GuiBase.GetGroupByID(3, 5).SetAutoScroll(true);
        }

        private void UpdateWindow()
        {
            if (messages.Count != MessageScrollContents.Count)
            {
                messageNum++;
                MessageScrollContents.Add(new GUI_content(messages.Count, contentType: GUI_Item_Type.LABEL, $"[{messageNum}] > {messages.GetLast().message}", null, new GUI_textColor(normal: msgTypeColors[messages.GetLast().type]), textAlign: TextAnchor.MiddleLeft));
            }            

            GuiBase.RefreshGroup(3, 5);
        }

        private void AddMessage(string message, MessageType type, params object[] arg)
        {
            messages.Add(new MSG()
            {
                message = string.Format(message, arg),
                type = type
            });

            if (messages.Count == MAXMSG)
            {
                messages.RemoveAt(0);
                MessageScrollContents.RemoveAt(0);                
            }

            UpdateWindow();

            ZLogger.Log(message, type, arg);
        }        

        public void Message(string message) => AddMessage(message, MessageType.Log);

        public void Message(string message, MessageType type) => AddMessage(message, type);

        public void Message(string message, params object[] arg) => AddMessage(message, MessageType.Log, arg);

        public void Message(string message, MessageType type, params object[] arg) => AddMessage(message, type, arg);

        [Conditional("DEBUG")]
        public void DebugMessage(string message, params object[] arg) => AddMessage("[DEBUG] " + message , MessageType.Debug, arg);
    }

}