using BZCommon;
using BZCommon.Helpers.RuntimeGUI;
using UnityEngine;

namespace ZHelper
{
    public class ZHelper_Command : ConsoleCommand
    {
        public GameObject guiRoot;

        public override bool AvailableInStartScreen => true;

        public override bool AvailableInGame => true;

        public override void RegisterCommand()
        {
            DevConsole.RegisterConsoleCommand(this, "zhelper", false, false);
        }

        public void OnConsoleCommand_zhelper(NotificationCenter.Notification n)
        {
            if (!guiRoot)
            {
                guiRoot =  new GameObject("ZHelper_IGUI", typeof(ZHelper));
            }
            else
            {
                guiRoot.GetComponent<GUI_ROOT>().GuiBase.ShowMainWindow();
            }            
        }        
    }
}
