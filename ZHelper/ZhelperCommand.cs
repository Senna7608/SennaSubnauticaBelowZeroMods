using BZCommon;
using BZCommon.Helpers.RuntimeGUI;
using UnityEngine;

namespace ZHelper
{
    public class ZHelperCommand : ConsoleCommand
    {
        public GameObject guiRoot;

        public override bool AvailableInStartScreen => true;

        public override bool AvailableInGame => false;

        public override void RegisterCommand()
        {
            DevConsole.RegisterConsoleCommand(this, "zhelper", false, false);
        }

        public void OnConsoleCommand_zhelper(NotificationCenter.Notification n)
        {
            if (!guiRoot)
            {
                guiRoot =  new GameObject("IGUI_Test", typeof(Zhelper));
            }
            else
            {
                guiRoot.GetComponent<GUI_ROOT>().GuiBase.ShowMainWindow();
            }            
        }        
    }
}
