using UnityEngine;
using BZCommon;

namespace CheatManagerZero.Configuration
{
    public class CMZConfigCommand : MonoBehaviour
    {
        CMZConfigCommand Instance;

        public void Awake()
        {            
            DevConsole.RegisterConsoleCommand(this, "cmzconfig", false, false);
            BZLogger.Log($"[{Config.PROGRAM_NAME}] Information: Enter 'cmzconfig' command for configuration window.");
        }

        private void OnConsoleCommand_cmzconfig(NotificationCenter.Notification n)
        {
            new CMZConfigUI();
        }

        public CMZConfigCommand()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(CMZConfigCommand)) as CMZConfigCommand;

                if (Instance == null)
                {
                    GameObject cmzconfig_command = new GameObject("cmzconfig_command");
                    Instance = cmzconfig_command.AddComponent<CMZConfigCommand>();
                }
            }
            else
            {
                Instance.Awake();
            }            
        }
    }
}
